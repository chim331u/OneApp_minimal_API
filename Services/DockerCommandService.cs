using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
    private readonly ILocalVaultService _localVaultService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerCommandService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    /// <param name="utilityServices">The utility services for encryption and other utilities.</param>
    public DockerCommandService(ApplicationContext context, ILogger<DockerCommandService> logger,
        IUtilityServices utilityServices, ILocalVaultService localVaultService)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
        _localVaultService = localVaultService;
    }

    #region Commands

    /// <summary>
    /// Builds a Docker image for a specific deployment.
    /// </summary>
    /// <param name="deployId">The unique identifier of the Docker deployment.</param>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the build command.</returns>
    public async Task<DockerCommandResponse<string>> GetBuildCommand(int deployId)
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

        string noCache = string.Empty;
        if (dockerConfig.noCache)
        {
            noCache = " --no-cache";
        }

        var command =
            $"docker build{noCache} -t {dockerConfig.AppName}_image:latest -t {imageName} -f {dockerConfig.DockerFileName} .";
        _logger.LogInformation($"Docker build command: {command}");
        return new DockerCommandResponse<string>(command, dockerConfig.DockerFileName, true);
    }

    /// <summary>
    /// Retrieves the list of running Docker containers.
    /// </summary>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the command to list running containers.</returns>
    public async Task<DockerCommandResponse<string>> GetRunningContainersCommand()
    {
        // The command to list running Docker containers in JSON format
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

    public async Task<DockerCommandResponse<string>> GetPushImageCommand(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        if (dockerConfig.DockerRepositorySettings == null)
        {
            _logger.LogWarning("No Docker repository settings found");
            return new DockerCommandResponse<string>($"No Docker repository settings found for id {deployId}",
                "FindDockerConfig", false);
        }

        var myDockerRepoUsername = dockerConfig.DockerRepositorySettings.UserName;

        var pushImagesCommand =
            $"docker push {myDockerRepoUsername}/{dockerConfig.AppName}_image:{dockerConfig.ImageVersion}";

        _logger.LogInformation($"Docker push command =  {pushImagesCommand}");

        return new DockerCommandResponse<string>(pushImagesCommand, "GetPushImageCommand", true);
    }

    public async Task<DockerCommandResponse<string>> GetLoginDockerRegistryCommand(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        if (dockerConfig.DockerRepositorySettings == null)
        {
            _logger.LogWarning("No Docker repository settings found");
            return new DockerCommandResponse<string>($"No Docker repository settings found for id {deployId}",
                "FindDockerConfig", false);
        }

        var myDockerRepoUsername = dockerConfig.DockerRepositorySettings.UserName;
        var myDockerRepoPassword =
            _localVaultService.GetSecret(dockerConfig.DockerRepositorySettings.Password).Result.Data.Value;
            //await _utilityServices.DecryptString(dockerConfig.DockerRepositorySettings.Password);

        var dockingRegistryLogin = $"docker login -u {myDockerRepoUsername} -p {myDockerRepoPassword} docker.io";

        _logger.LogInformation($"Docker registry login command =  {dockingRegistryLogin}");

        return new DockerCommandResponse<string>(dockingRegistryLogin, "GetPushImageCommand", true);
    }

    public async Task<DockerCommandResponse<string>> GetTagImageCommand(int deployId)
    {
        var dockerConfig = await _context.DockerConfig
            .Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }


        if (dockerConfig.DockerRepositorySettings == null)
        {
            _logger.LogWarning("No Docker repository settings found");
            return new DockerCommandResponse<string>($"No Docker repository settings found for id {deployId}",
                "FindDockerConfig", false);
        }

        var myDockerRepoUsername = dockerConfig.DockerRepositorySettings.UserName;

        var dockingTagImage =
            $"docker tag {dockerConfig.AppName}_image {myDockerRepoUsername}/{dockerConfig.AppName}_image:{dockerConfig.ImageVersion}";

        _logger.LogInformation($"Tag Image command =  {dockingTagImage}");

        return new DockerCommandResponse<string>(dockingTagImage, "GetPushImageCommand", true);
    }

    public async Task<DockerCommandResponse<string>> GetRunContainerCommand(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var folderCommand1 = string.Empty;
        var folderCommand2 = string.Empty;
        var folderCommand3 = string.Empty;

        if (!string.IsNullOrEmpty(dockerConfig.FolderFrom1))
        {
            if (string.IsNullOrEmpty(dockerConfig.FolderContainer1))
            {
                _logger.LogInformation($"Folder Container 1 is empty: load default configuration (/data)");
                dockerConfig.FolderContainer1 = @"/data";
            }

            folderCommand1 = string.Concat(@" -v ", dockerConfig.FolderFrom1, ":", dockerConfig.FolderContainer1);
        }

        if (!string.IsNullOrEmpty(dockerConfig.FolderFrom2))
        {
            if (string.IsNullOrEmpty(dockerConfig.FolderContainer2))
            {
                _logger.LogInformation($"Folder Container 2 is empty: load default configuration (/data)");
                dockerConfig.FolderContainer2 = @"/data";
            }

            folderCommand2 = string.Concat(@" -v ", dockerConfig.FolderFrom2, ":", dockerConfig.FolderContainer2);
        }

        if (!string.IsNullOrEmpty(dockerConfig.FolderFrom3))
        {
            if (string.IsNullOrEmpty(dockerConfig.FolderContainer3))
            {
                _logger.LogInformation($"Folder Container 3 is empty: load default configuration (/data)");

                dockerConfig.FolderContainer3 = @"/data";
            }

            folderCommand3 = string.Concat(@" -v ", dockerConfig.FolderFrom3, ":", dockerConfig.FolderContainer3);
        }

        var imageName = $"{dockerConfig.AppName}_image:latest";

        string dockerCommand =
            $"docker run --restart always --name {dockerConfig.AppName} -d -p " +
            dockerConfig.PortAddress + folderCommand1 + folderCommand2 + folderCommand3 + " " + imageName + "";

        _logger.LogInformation($"Run command= {dockerCommand}");

        return new DockerCommandResponse<string>(dockerCommand, "RunImageCommand", true);
    }

    #endregion


    /// <summary>
    /// Sends an SSH command to a Docker container.
    /// </summary>
    /// <param name="deployId">The unique identifier of the Docker deployment.</param>
    /// <param name="command">The SSH command to execute.</param>
    /// <returns>A <see cref="DockerCommandResponse{T}"/> containing the result of the command.</returns>
    public async Task<DockerCommandResponse<string>> SendSSHCommand(int deployId, string command)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.NasSettings).Where(x => x.Id == deployId)
            .FirstOrDefaultAsync();

        if (dockerConfig == null) return null!;

        if (dockerConfig.NasSettings != null)
        {
            if (command.Contains("docker"))
            {
                command = command.Replace("docker", dockerConfig.NasSettings.DockerCommandPath);
            }

            using var client = new SshClient(dockerConfig.NasSettings.Address, dockerConfig.NasSettings.UserName,
                 _localVaultService.GetSecret(dockerConfig.NasSettings.Password).Result.Data.Value);
                //await _utilityServices.DecryptString(dockerConfig.NasSettings.Password));
            client.Connect();

            _logger.LogInformation($"SSH Command '{command}' is sent...");

            var cmd = client.RunCommand(command);

            if (!string.IsNullOrEmpty(cmd.Error))
            {
                var error = string.Empty;
                if (cmd.Error.Contains(
                        "Error build images: DEPRECATED: The legacy builder is deprecated and will be removed in a future release.\r\n            Install the buildx component to build images with BuildKit:\r\n            https://docs.docker.com/go/buildx/"))
                {
                    error = cmd.Error.Replace(
                        "Error build images: DEPRECATED: The legacy builder is deprecated and will be removed in a future release.\r\n            Install the buildx component to build images with BuildKit:\r\n            https://docs.docker.com/go/buildx/",
                        string.Empty).Trim();
                }

                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogError($"SSH Error command '{command}' - {cmd.Error}");
                    return new DockerCommandResponse<string>(cmd.Error, command, false);
                }
            }

            _logger.LogInformation($"SSH Command result: {cmd.Result}");
            return new DockerCommandResponse<string>(cmd.Result, command, true);
        }

        return new DockerCommandResponse<string>($"No Docker repository settings found for id {deployId}",
            "FindDockerConfig", false);
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
                    $"FROM mcr.microsoft.com/dotnet/aspnet:{dockerConfig.SkdVersion} AS base");
                await dockerFile.WriteLineAsync($"USER $APP_UID");
                await dockerFile.WriteLineAsync($"WORKDIR /app");
                await dockerFile.WriteLineAsync($"EXPOSE 8080");
                await dockerFile.WriteLineAsync($"EXPOSE 8081");
                await dockerFile.WriteLineAsync();

                await dockerFile.WriteLineAsync(
                    $"FROM mcr.microsoft.com/dotnet/sdk:{dockerConfig.SkdVersion} AS source");
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
                await dockerFile.WriteLineAsync();

                await dockerFile.WriteLineAsync(
                    $"FROM mcr.microsoft.com/dotnet/sdk:{dockerConfig.SkdVersion} AS builder");
                await dockerFile.WriteLineAsync($"COPY --from=source /build/src/ /build");
                if (!string.IsNullOrEmpty(dockerConfig.SolutionFolder))
                {
                    dockerConfig.SolutionFolder = $"/{dockerConfig.SolutionFolder}";
                }

                await dockerFile.WriteLineAsync($"WORKDIR /build{dockerConfig.SolutionFolder}");
                await dockerFile.WriteLineAsync(
                    $"RUN dotnet build {dockerConfig.BuildProject} -c release -o /app/build");
                await dockerFile.WriteLineAsync();

                await dockerFile.WriteLineAsync($"FROM builder AS publish");
                await dockerFile.WriteLineAsync($"RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false");
                await dockerFile.WriteLineAsync();


                if (dockerConfig.AppEntryName == "WASM")
                {
                    await dockerFile.WriteLineAsync($"FROM nginx:alpine AS final");
                    await dockerFile.WriteLineAsync($"WORKDIR /usr/share/nginx/html");
                    await dockerFile.WriteLineAsync($"COPY --from=publish /app/publish/wwwroot .");
                }
                else
                {
                    await dockerFile.WriteLineAsync($"FROM base AS final");
                    await dockerFile.WriteLineAsync($"WORKDIR /app");
                    await dockerFile.WriteLineAsync($"COPY --from=publish /app/publish .");
                    await dockerFile.WriteLineAsync($"ENTRYPOINT [\"dotnet\", \"{dockerConfig.AppEntryName}\"]");
                }
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
    /// Upload docker file to NAS.  
    /// </summary>
    /// <param name="deployId"></param>
    /// <returns>DockerCommand Response</returns>
    public async Task<DockerCommandResponse<string>> UploadDockerFile(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Include(x => x.NasSettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        try
        {
            if (dockerConfig.NasSettings != null)
            {
                using var client = new SftpClient(dockerConfig.NasSettings.Address, dockerConfig.NasSettings.UserName,
                    _localVaultService.GetSecret(dockerConfig.NasSettings.Password).Result.Data.Value);
                    //await _utilityServices.DecryptString(dockerConfig.NasSettings.Password));
                client.Connect();
                _logger.LogInformation($"Client connected to NAS");

                await using FileStream fs = File.OpenRead(dockerConfig.DockerFileName);
                var nasPath = $"{dockerConfig.NasSettings.DockerFilePath}{dockerConfig.DockerFileName}";
                client.UploadFile(fs, nasPath);
            }

            _logger.LogInformation($"Docker file uploaded to NAS ");
            return new DockerCommandResponse<string>("Connected to NAS, Docker File uploaded", "UploadDockerFile",
                true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error upload docker file: {ex.Message}");
            return new DockerCommandResponse<string>(ex.Message, "UploadDockerFile", false);
        }
    }

    public async Task<DockerCommandResponse<string>> GetRemoteRunningContainers(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId)
            .FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var containersCommandResult = await GetRunningContainersCommand();

        if (!containersCommandResult.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrieve Docker container command");
            return new DockerCommandResponse<string>("Unable to retrieve Docker container command",
                "GetRunningContainersCommand", false);
        }

        _logger.LogInformation($"Docker running containers get command =  {containersCommandResult.Data}");
        var containerList = await SendSSHCommand(deployId, containersCommandResult.Data);

        return new DockerCommandResponse<string>(containerList.Data, "GetRemoteRunningContainers", true);
    }

    public async Task<DockerCommandResponse<string>> GetRemoteImageList(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();


        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var imageCommandResult = await GetImageListCommand();
        if (!imageCommandResult.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrieve Docker images list ");
            return new DockerCommandResponse<string>("Unable to retrieve Docker image list command",
                "GetImageListCommand", false);
        }

        _logger.LogInformation($"Docker images list =  {imageCommandResult.Data}");
        var imageList = await SendSSHCommand(deployId, imageCommandResult.Data);

        return new DockerCommandResponse<string>(imageList.Data, "GetRemoteImageList ", true);
    }

    public async Task<DockerCommandResponse<string>> RemoveRemoteRunningContainers(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Include(x => x.NasSettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var runningContainer = await GetRemoteRunningContainers(deployId);

        if (!runningContainer.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrieve remote running container");
            return new DockerCommandResponse<string>("Unable to retrieve remote running container",
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
            _logger.LogWarning("No Active Container to remove");
            return new DockerCommandResponse<string>("No Active Container to remove", "GetRemoteRunningContainers",
                true);
        }

        _logger.LogInformation(
            $"Stopping and remove container {runningContainerToDelete.Id} - {runningContainerToDelete.Names}");

        if (dockerConfig.NasSettings != null)
        {
            var containerRemoveResponse = await SendSSHCommand(deployId,
                $"docker rm --force {runningContainerToDelete.Id}");

            if (!containerRemoveResponse.IsSuccess)
            {
                _logger.LogError($"Error removing container: {containerRemoveResponse.Data}");
                return new DockerCommandResponse<string>(containerRemoveResponse.Data, $"Error removing container",
                    false);
            }

            _logger.LogInformation(
                $"Container removed: {runningContainerToDelete.Id} - {runningContainerToDelete.Names}");
            return new DockerCommandResponse<string>(containerRemoveResponse.Data,
                $"Container removed: {runningContainerToDelete.Id} - {runningContainerToDelete.Names}", true);
        }

        _logger.LogWarning("No Docker repository settings found");
        return new DockerCommandResponse<string>($"No Docker repository settings found for id {deployId}",
            "FindDockerConfig", false);
    }

    public async Task<DockerCommandResponse<string>> RemoveRemoteImagesList(int deployId)
    {
        var dockerConfig = await _context.DockerConfig
            .Include(x => x.DockerRepositorySettings)
            .Include(x => x.NasSettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var imagesList = await GetRemoteImageList(deployId);

        if (!imagesList.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrieve remote images list");
            return new DockerCommandResponse<string>("Unable to retrieve remote images list",
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

            foreach (var imageModel in imageModels.Where(imageModel =>
                         imageModel.Repository == "<none>" || imageModel.Repository == imageName))
            {
                logtoreturn +=
                    ($"Removing image {imageModel.Id} - {imageModel.Repository} - {imageModel.Tag} {Environment.NewLine}");


                if (dockerConfig.NasSettings == null) continue;

                var removeImageResponse =
                    await SendSSHCommand(deployId,
                        $@"docker rmi {imageModel.Id} --force");

                if (!removeImageResponse.IsSuccess)
                {
                    logtoreturn +=
                        ($"Error removing image {imageModel.Id}: {removeImageResponse.Data} {Environment.NewLine}");
                }
                else
                {
                    logtoreturn +=
                        ($"Image removed: {imageModel.Id} - {imageModel.Repository} {Environment.NewLine}");
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

    public async Task<DockerCommandResponse<string>> BuildImage(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var buildCommand = await GetBuildCommand(deployId);
        if (!buildCommand.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrive build command");
            return new DockerCommandResponse<string>("Unable to retrieve build command",
                "BuildCommand", false);
        }

        _logger.LogWarning($"Build command: {buildCommand.Data} sent: please wait for the result");

        var buildResult = await SendSSHCommand(deployId, buildCommand.Data);

        if (!buildResult.IsSuccess)
        {
            _logger.LogError($"Unable to build image");
            return new DockerCommandResponse<string>(buildResult.Data,
                "Unable to Build Image", false);
        }

        return new DockerCommandResponse<string>(buildResult.Data, "BuildImage", true);
    }

    public async Task<DockerCommandResponse<string>> RunContainer(int deployId)
    {
        var runCommand = await GetRunContainerCommand(deployId);
        if (!runCommand.IsSuccess)
        {
            _logger.LogWarning($"Unable to retrieve run command");
            return new DockerCommandResponse<string>("Unable to retrieve build command",
                "RunCommand", false);
        }

        _logger.LogWarning($"Run command: {runCommand.Data} sent: Container is starting, please wait for the result");

        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();

        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var runResult = await SendSSHCommand(deployId, runCommand.Data);

        if (!runResult.IsSuccess)
        {
            _logger.LogError($"Unable to run container");
            return new DockerCommandResponse<string>(runResult.Data,
                "Unable to Run Container", false);
        }

        return new DockerCommandResponse<string>(runResult.Data, "RunImage", true);
    }

    public async Task<DockerCommandResponse<string>> PushImage(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.Include(x => x.DockerRepositorySettings)
            .Where(x => x.Id == deployId).FirstOrDefaultAsync();


        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                "FindDockerConfig", false);
        }

        var loginCommand = await GetLoginDockerRegistryCommand(deployId);

        _logger.LogInformation($"Login registry command = {loginCommand.Data} ");

        var loginResult = await SendSSHCommand(deployId, loginCommand.Data);
        if (!loginResult.IsSuccess)
        {
            _logger.LogError($"Unable to login to docker registry");
            return new DockerCommandResponse<string>(loginResult.Data,
                "Unable to Login to Docker Registry", false);
        }

        _logger.LogInformation($"Login to docker registry: {loginResult.Data} ");
        var tagCommand = await GetTagImageCommand(deployId);

        _logger.LogInformation($"Tag command = {tagCommand.Data} ");
        var tagResult = await SendSSHCommand(deployId, tagCommand.Data);
        if (!tagResult.IsSuccess)
        {
            _logger.LogError($"Unable to tag image");
            return new DockerCommandResponse<string>(tagResult.Data,
                "Unable to Tag image", false);
        }

        _logger.LogInformation($"Image tagged: {tagResult.Data} ");

        var pushCommand = await GetPushImageCommand(deployId);

        _logger.LogInformation($"Push command = {pushCommand.Data} ");

        var pushResult = await SendSSHCommand(deployId, pushCommand.Data);

        if (!pushResult.IsSuccess)
        {
            _logger.LogError($"Unable to push image");
            return new DockerCommandResponse<string>(pushResult.Data,
                "Unable to Push image", false);
        }

        _logger.LogInformation($"Image Pushed : {pushCommand.Data} ");
        return new DockerCommandResponse<string>(pushResult.Data, "PushImage", true);
    }
    
    // public async Task<List<DownloadFile>> AddEd2kLink(string ed2kLink)
    // {
    //     var param = new Dictionary<string, string>();
    //     param.Add("Submit", "Download link");
    //     param.Add("ed2klink", ed2kLink);
    //     param.Add("selectcat", "all");
    //
    //     var result = await _networkHelperServices.PostRequest(_footer, param);
    //
    //     if (!string.IsNullOrEmpty(result))
    //     {
    //         return ParseDownloading(result);
    //     }
    //
    //     return null;
    //
    // }
    //
    //
    // public async Task<string> PostRequest(string page, Dictionary<string, string> parameters )
    // {
    //     try
    //     {
    //         var url = _utilityServices.ApiUrl + page;
    //         using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(parameters) };
    //         var result = await httpClient.SendAsync(req);
    //
    //         if (result.IsSuccessStatusCode)
    //         {
    //             return await result.Content.ReadAsStringAsync();
    //         }
    //
    //         _logger.LogWarning($"Post command response: {result.StatusCode}");
    //         return string.Empty;
    //
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError($"Error post command: {ex.Message}");
    //         return string.Empty;
    //
    //     }
    //
    // }
}