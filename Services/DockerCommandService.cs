using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;
using Renci.SshNet;

namespace OneApp_minimalApi.Services;

/// <summary>
/// Provides services for executing Docker-related commands.
/// </summary>
public class DockerCommandService : IDockerCommandService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<DockerCommandService> _logger; // Logger for logging information and errors
    private readonly IUtilityServices _utilityServices; // Utility services for encryption and other utilities

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerCommandService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="utilityServices">The utility services for encryption and other utilities.</param>
    public DockerCommandService(ApplicationContext context, ILogger<DockerCommandService> logger,
        IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
    }

    /// <summary>
    /// Sends an SSH command to a Docker container.
    /// </summary>
    /// <param name="deployId">The unique identifier of the Docker deployment.</param>
    /// <param name="command">The SSH command to execute.</param>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the result of the command.</returns>
    public async Task<DockerCommandResponse<string>> SendSSHCommand(int deployId, string command)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);

        if (dockerConfig == null) return null!;

        using var client = new SshClient(dockerConfig.Host, dockerConfig.User,
            await _utilityServices.DecryptString(dockerConfig.Password));
        client.Connect();

        _logger.LogInformation($"SSH Command '{command}' is sent...");

        var cmd = client.RunCommand(command);
        if (!string.IsNullOrEmpty(cmd.Error))
        {
            _logger.LogError($"SSH Error command '{command}' - {cmd.Error}");
            return new DockerCommandResponse<string>(cmd.Error, command, false);
        }

        _logger.LogInformation($"SSH Command result: {cmd.Result}");
        return new DockerCommandResponse<string>(cmd.Result, command, true);
    }

    /// <summary>
    /// Sends a localhost command to the Docker container.
    /// </summary>
    /// <param name="deployId">The unique identifier of the Docker deployment.</param>
    /// <param name="command">The command to execute locally.</param>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the result of the command.</returns>
    public async Task<DockerCommandResponse<string>> SendLocalhostCommand(int deployId, string command)
    {
        try
        {
            string shell;
            string shellArgsPrefix;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shell = "cmd.exe";
                shellArgsPrefix = "/c";
            }
            else
            {
                shell = "/bin/bash";
                shellArgsPrefix = "-c";
            }

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = shell,
                Arguments = $"{shellArgsPrefix} \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(output))
            {
                _logger.LogInformation($"Output: {output}");
                return new DockerCommandResponse<string>(output, command, true);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                _logger.LogError($"Error: {error}");
                return new DockerCommandResponse<string>(error, command, false);
            }

            _logger.LogWarning("No output or error received.");
            return new DockerCommandResponse<string>("No output or error received.", command, false);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new DockerCommandResponse<string>(ex.Message, command, false);
        }
    }

    /// <summary>
    /// Creates a Dockerfile for a specific deployment.
    /// </summary>
    /// <param name="deployId">The unique identifier of the Docker deployment.</param>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the Dockerfile content.</returns>
    public async Task<DockerCommandResponse<string>> CreateDockerFile(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        try
        {
            await using (var dockerFile = new StreamWriter(dockerConfig.DockerFileName, false, Encoding.UTF8))
            {
                await dockerFile.WriteLineAsync(
                    $"FROM mcr.microsoft.com/dotnet/sdk:{dockerConfig.SkdVersion} as SOURCE");
                await dockerFile.WriteLineAsync($"RUN apt-get update && apt-get install -y \\");
                await dockerFile.WriteLineAsync($"	curl \\");
                await dockerFile.WriteLineAsync($"	git \\");
                await dockerFile.WriteLineAsync($"	unzip \\");
                await dockerFile.WriteLineAsync($"	--no-install-recommends");
                await dockerFile.WriteLineAsync($"WORKDIR /build");
                if (!string.IsNullOrEmpty(dockerConfig.Branch))
                {
                    dockerConfig.Branch = $"-b {dockerConfig.Branch}";
                }

                await dockerFile.WriteLineAsync(
                    $"RUN git clone {dockerConfig.Branch} {dockerConfig.SolutionRepository} src");
                await dockerFile.WriteLineAsync(
                    $"FROM mcr.microsoft.com/dotnet/sdk:{dockerConfig.SkdVersion} as BUILDER");
                await dockerFile.WriteLineAsync($"COPY --from=SOURCE /build/src/ /build");
                await dockerFile.WriteLineAsync($"WORKDIR /build {dockerConfig.SolutionFolder}");
                await dockerFile.WriteLineAsync(
                    $"RUN cd /build {dockerConfig.SolutionFolder} ; dotnet build {dockerConfig.BuildProject}");
                await dockerFile.WriteLineAsync(
                    $"RUN cd /build {dockerConfig.SolutionFolder} ; dotnet publish {dockerConfig.BuildProject} -c release -o /build/publish");
                await dockerFile.WriteLineAsync($"RUN cd /build/publish; ls");
                await dockerFile.WriteLineAsync($"FROM mcr.microsoft.com/dotnet/aspnet:{dockerConfig.SkdVersion}");
                await dockerFile.WriteLineAsync($"COPY --from=BUILDER /build/publish/ /app");
                await dockerFile.WriteLineAsync($"WORKDIR /app");
                //todo: check if wasm
                await dockerFile.WriteLineAsync($"ENTRYPOINT [\"dotnet\", \"{dockerConfig.AppEntryName}\"]");
            }

            _logger.LogInformation($"Dockerfile created at {dockerConfig.DockerFileName}");
            var dockerFileText = await new StreamReader(dockerConfig.DockerFileName).ReadToEndAsync();
            _logger.LogInformation($"Dockerfile content: {dockerFileText}");

            return new DockerCommandResponse<string>(dockerFileText, dockerConfig.DockerFileName, true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating Dockerfile: {ex.Message}");
            return new DockerCommandResponse<string>(ex.Message, dockerConfig.DockerFileName, false);
        }
    }

    /// <summary>
    /// Builds a Docker image for a specific deployment.
    /// </summary>
    /// <param name="deployId">The unique identifier of the Docker deployment.</param>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the build command.</returns>
    public async Task<DockerCommandResponse<string>> BuildCommand(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var imageName = $"{dockerConfig.AppName}_image:{dockerConfig.ImageVersion}";
        _logger.LogInformation($"Image name: {imageName}");

        var command = $" build -t {dockerConfig.AppName}:latest -t {imageName} -f {dockerConfig.DockerFileName} .";
        _logger.LogInformation($"Docker build command: {command}");
        return new DockerCommandResponse<string>(command, dockerConfig.DockerFileName, true);
    }

    /// <summary>
    /// Retrieves the list of running Docker containers.
    /// </summary>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the command to list running containers.</returns>
    public async Task<DockerCommandResponse<string>> GetRunningContainersCommand()
    {
        var runningContainerCommand = $"docker container ls --format='{{{{json .}}}}'";

        _logger.LogInformation($"Running Containers command =  {runningContainerCommand}");

        return new DockerCommandResponse<string>(runningContainerCommand, "RunningContainerCommand", true);
    }


    /// <summary>
    /// Retrieves the list of Docker images in json format.
    /// </summary>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the command to list docker images.</returns>
    public async Task<DockerCommandResponse<string>> GetImageListCommand()
    {
        var getImagesCommand = $"docker images --format='{{{{json .}}}}'";

        _logger.LogInformation($"Docker images command =  {getImagesCommand}");

        return new DockerCommandResponse<string>(getImagesCommand, "GetImageListCommand", true);
    }

    public async Task<DockerCommandResponse<string>> GetRemoteRunningContainers(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var containersCommandResult = await GetRunningContainersCommand();

        if (!containersCommandResult.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrive Docker container command");
            return new DockerCommandResponse<string>("Unable to retrive Docker container command",
                "GetRunningContainersCommand", false);
        }

        if (containersCommandResult.Data.Contains("docker"))
        {
            containersCommandResult.Data = containersCommandResult.Data.Replace("docker", dockerConfig.DockerCommand);
        }

        _logger.LogInformation($"Docker running containers get command =  {containersCommandResult.Data}");
        var containerList = await SendSSHCommand(deployId, containersCommandResult.Data);

        return new DockerCommandResponse<string>(containerList.Data, "GetRemoteRunningContainers", true);
    }

    public async Task<DockerCommandResponse<string>> GetRemoteImageList(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var imageCommandResult = await GetImageListCommand();
        if (!imageCommandResult.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrive Docker images list ");
            return new DockerCommandResponse<string>("Unable to retrive Docker image list command",
                "GetImageListCommand", false);
        }

        if (imageCommandResult.Data.Contains("docker"))
        {
            imageCommandResult.Data = imageCommandResult.Data.Replace("docker", dockerConfig.DockerCommand);
        }

        _logger.LogInformation($"Docker images list =  {imageCommandResult.Data}");
        var imageList = await SendSSHCommand(deployId, imageCommandResult.Data);

        return new DockerCommandResponse<string>(imageList.Data, "GetRemoteImageList ", true);
    }

    public async Task<DockerCommandResponse<string>> RemoveRemoteRunningContainers(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var runningContainer = await GetRemoteRunningContainers(deployId);

        if (!runningContainer.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrive remote running container");
            return new DockerCommandResponse<string>("Unable to retrive remote running container",
                "GetRemoteRunningContainers", false);
        }

        if (string.IsNullOrEmpty(runningContainer.Data))
        {
            _logger.LogWarning($"No containers are running");
            return new DockerCommandResponse<string>("No containers are running",
                "GetRemoteRunningContainers", false);
        }

        var containers = new List<ContainerModel>();

        runningContainer.Data.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList()
            .ForEach(c => containers.Add(JsonConvert.DeserializeObject<ContainerModel>(c)));

        var runningContainerToDelete = containers.FirstOrDefault(c => c.Names == dockerConfig.AppName);

        if (runningContainerToDelete == null)
        {
            runningContainer = null;
            _logger.LogWarning("No Active Container to remove");
            return new DockerCommandResponse<string>("No Active Container to remove", "GetRemoteRunningContainers",
                true);
        }

        _logger.LogInformation(
            $"Stopping and remove container {runningContainerToDelete.Id} - {runningContainerToDelete.Names}");

        var containerRemoveResponse = await SendSSHCommand(deployId,
            $"{dockerConfig.DockerCommand} rm --force {runningContainerToDelete.Id}");

        if (!containerRemoveResponse.IsSuccess)
        {
            runningContainer = null;
            _logger.LogError($"Error removing container: {containerRemoveResponse.Data}");
            return new DockerCommandResponse<string>(containerRemoveResponse.Data, $"Error removing container", false);
        }

        runningContainer = null;
        _logger.LogInformation($"Container removed: {runningContainerToDelete.Id} - {runningContainerToDelete.Names}");
        return new DockerCommandResponse<string>(containerRemoveResponse.Data,
            $"Container removed: {runningContainerToDelete.Id} - {runningContainerToDelete.Names}", true);
    }

    public async Task<DockerCommandResponse<string>> RemoveRemoteImagesList(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }
        var imagesList = await GetRemoteImageList(deployId);

        if (!imagesList.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrive remote images list");
            return new DockerCommandResponse<string>("Unable to retrive remote images list",
                "RemoveRemoteImagesList", false);
        }

        if (string.IsNullOrEmpty(imagesList.Data))
        {
            _logger.LogWarning($"No images found");
            return new DockerCommandResponse<string>("No images found",
                "RemoveRemoteImagesList", false);
        }
        
        try
        {
            //TODO: manage the lastet image version?
            var imageName = $"{dockerConfig.AppName}_image:{dockerConfig.ImageVersion}";
            var imageModels = new List<ImageModel>();

            imagesList.Data.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList()
                .ForEach(c => imageModels.Add(JsonConvert.DeserializeObject<ImageModel>(c)));

            var logtoreturn = "";

            foreach (var imageModel in imageModels.Where(imageModel => imageModel.Repository == "<none>" || imageModel.Repository == imageName))
            {
                logtoreturn += ($"Removing image {imageModel.Id} - {imageModel.Repository} - {imageModel.Tag} {Environment.NewLine}");

                //var response = ssh.RunCommand($@"{variables.DockerCommand} rmi {imageModel.Id} --force");
                var removeImageResponse = await SendSSHCommand(deployId, $@"{dockerConfig.DockerCommand} rmi {imageModel.Id} --force");

                if (!removeImageResponse.IsSuccess)
                {
                    logtoreturn += ($"Error removing image {imageModel.Id}: {removeImageResponse.Data} {Environment.NewLine}");
                }
                else
                {
                    logtoreturn += ($"Image removed: {imageModel.Id} - {imageModel.Repository} {Environment.NewLine}");
                }
            }
            return new DockerCommandResponse<string>(logtoreturn, "RemoveRemoteImagesList", true);

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in deleting images: {ex.Message}");
            return new DockerCommandResponse<string>(ex.Message, "RemoveRemoteImagesList", false);
        }
    }

}