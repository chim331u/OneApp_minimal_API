using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Interfaces;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace OneApp_minimalApi.Services;

public class DockerCommandService : IDockerCommandService
{
    private readonly ApplicationContext _context; // Database context
    private readonly ILogger<DockerCommandService> _logger; // Logger for logging information and error
    private readonly IUtilityServices _utilityServices; // Utility services for encryption and other utilities

    public DockerCommandService(ApplicationContext context, ILogger<DockerCommandService> logger,
        IUtilityServices utilityServices)
    {
        _context = context;
        _logger = logger;
        _utilityServices = utilityServices;
    }

    public async Task<DockerCommandResponse<string>> SendSSHCommand(int deployId, string _command)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        
        using (var client = new SshClient(dockerConfig.Host, dockerConfig.User, await _utilityServices.DecryptString(dockerConfig.Password)))
        {
            client.Connect();

            _logger.LogInformation($"SSH Command {_command} is sent...");

            SshCommand cmd = client.RunCommand(_command);
            if (!string.IsNullOrEmpty(cmd.Error))
            {
                // if (cmd.Error.Contains(
                //         "Error build images: DEPRECATED: The legacy builder is deprecated and will be removed in a future release.\r\n            Install the buildx component to build images with BuildKit:\r\n            https://docs.docker.com/go/buildx/"))
                // {
                //     _logger.LogWarning(
                //         "SSH Error build images: DEPRECATED: The legacy builder is deprecated and will be removed in a future release.\r\n            Install the buildx component to build images with BuildKit:\r\n            https://docs.docker.com/go/buildx/");
                //
                //     var error = cmd.Error.Replace(
                //         "Error build images: DEPRECATED: The legacy builder is deprecated and will be removed in a future release.\r\n            Install the buildx component to build images with BuildKit:\r\n            https://docs.docker.com/go/buildx/",
                //         string.Empty).Trim();
                //
                // }

                _logger.LogError($"SSH Error command '{_command}' - {cmd.Error}");
                return new DockerCommandResponse<string>(_command, cmd.Error, false);
            }

            _logger.LogInformation($"SSH Command result: {cmd.Result} ");
            return new DockerCommandResponse<string>(_command, cmd.Result, true);

        }
    }
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
                command = "dir"; // comando equivalente su Windows
            }
            else
            {
                shell = "/bin/bash";
                shellArgsPrefix = "-c";
                // command = "ls -la";
            }

            using (var process = new Process())
            {
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
                    return new DockerCommandResponse<string>(command, output, true);
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    _logger.LogError($"Error: {error}");
                    return new DockerCommandResponse<string>(command, error, false);
                }

                _logger.LogWarning("No output or error received.");

                return new DockerCommandResponse<string>(command, "No output or error received.", false);
            }
        }

        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new DockerCommandResponse<string>(command, ex.Message, false);
        }
    }

    public async Task<DockerCommandResponse<string>> CreateDockerFile(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return null!;
        }

        try
        {
            using(StreamWriter dockerFile = new StreamWriter(dockerConfig.DockerFileName, false, Encoding.UTF8))
            {
                dockerFile.WriteLineAsync($"FROM mcr.microsoft.com/dotnet/sdk:{dockerConfig.SkdVersion} as SOURCE");
                dockerFile.WriteLineAsync($"RUN apt-get update && apt-get install -y \\	");
                dockerFile.WriteLineAsync($"	curl \\	");
                dockerFile.WriteLineAsync($"	git \\	");
                dockerFile.WriteLineAsync($"	unzip \\		");
                dockerFile.WriteLineAsync($"	--no-install-recommends");
                dockerFile.WriteLineAsync($"WORKDIR /build");
                if (!string.IsNullOrEmpty(dockerConfig.Branch))
                {
                    dockerConfig.Branch = $"-b {dockerConfig.Branch}";
                }
                dockerFile.WriteLineAsync($"RUN git clone {dockerConfig.Branch} {dockerConfig.SolutionRepository} src");
                dockerFile.WriteLineAsync($"FROM mcr.microsoft.com/dotnet/sdk:{dockerConfig.SkdVersion} as BUILDER");
                dockerFile.WriteLineAsync($"COPY --from=SOURCE /build/src/ /build");
                dockerFile.WriteLineAsync($"WORKDIR /build {dockerConfig.SolutionFolder}");
                dockerFile.WriteLineAsync($"RUN cd /build {dockerConfig.SolutionFolder} ; dotnet build {dockerConfig.BuildProject}");
                dockerFile.WriteLineAsync($"RUN cd /build {dockerConfig.SolutionFolder} ; dotnet publish {dockerConfig.BuildProject} -c release -o /build/publish");
                dockerFile.WriteLineAsync($"RUN cd /build/publish; ls");
                dockerFile.WriteLineAsync($"FROM mcr.microsoft.com/dotnet/aspnet:{dockerConfig.SkdVersion}");
                dockerFile.WriteLineAsync($"COPY --from=BUILDER /build/publish/ /app");
                dockerFile.WriteLineAsync($"WORKDIR /app");
                dockerFile.WriteLineAsync($"ENTRYPOINT [\"dotnet\", \"{dockerConfig.AppEntryName}\"]");
            }
            _logger.LogInformation($"Dockerfile created at {dockerConfig.DockerFileName}");
            var dockerFileText = await new StreamReader(dockerConfig.DockerFileName).ReadToEndAsync();
            _logger.LogInformation($"Dockerfile content: {dockerFileText}");
            
            return new DockerCommandResponse<string>(dockerConfig.DockerFileName, dockerFileText, true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating Dockerfile: {ex.Message}");
            return new DockerCommandResponse<string>(dockerConfig.DockerFileName, ex.Message, false);
        }


        
    }

    public async Task<DockerCommandResponse<string>> BuildCommand(int deployId)
    {
        var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
        if (dockerConfig == null)
        {
            _logger.LogWarning("No configuration found");
            return null!;
        }
        //todo image version ++
        var imageName = $"{dockerConfig.AppName}_image:{dockerConfig.ImageVersion}";
        _logger.LogInformation($"Image name: {imageName}");
        
        var command = $" build -t {dockerConfig.AppName}:latest -t {imageName} -f {dockerConfig.DockerFileName} .";
        _logger.LogInformation($"Docker build command: {command}");
        return new DockerCommandResponse<string>(dockerConfig.DockerFileName, command, true);
        
    }
}