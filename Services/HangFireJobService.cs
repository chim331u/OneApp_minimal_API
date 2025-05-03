using Microsoft.AspNetCore.SignalR;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Contracts.Enum;
using OneApp_minimalApi.Contracts.FilesDetail;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models;
using Serilog;

namespace OneApp_minimalApi.Services
{
    public class HangFireJobService : IHangFireJobService
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<HangFireJobService> _logger;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IFilesDetailService _serviceData;
        private readonly IConfigsService _configsService;
        private readonly IUtilityServices _utility;
        private readonly IMachineLearningService _machineLearningService;
        private readonly IDockerCommandService _deployActions;

        public HangFireJobService(ILogger<HangFireJobService> logger, ApplicationContext context, IConfiguration config,
            IHubContext<NotificationHub> hubContext, IFilesDetailService serviceData, IUtilityServices utility,
            IConfigsService configsService, IMachineLearningService machineLearningService,
            IDockerCommandService deployActions)

        {
            _context = context;
            _config = config;
            _logger = logger;
            _notificationHub = hubContext;
            _serviceData = serviceData;
            _utility = utility;
            _configsService = configsService;
            _machineLearningService = machineLearningService;
            _deployActions = deployActions;
        }

        /// This method is called by Hangfire to test signlalR
        public async Task ExecuteAsync(string fileName, string destinationFolder, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting background job");
            await _notificationHub.Clients.All.SendAsync("notifications", "Starting background job", 0);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            await _notificationHub.Clients.All.SendAsync("notifications", $"fileName = {fileName}", 0);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            await _notificationHub.Clients.All.SendAsync("notifications", $"destinationFolder = {destinationFolder}",
                0);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

            _logger.LogInformation("Completed background job");

            await _notificationHub.Clients.All.SendAsync("notifications", "Completed processing job", 1);
        }

        public async Task MoveFilesJob(List<FileMoveDto> filesToMove, CancellationToken cancellationToken)
        {
            var jobStartTime = DateTime.Now;
            int fileMoved = 0;

            var _originDir = await _configsService.GetConfigValue("ORIGINDIR");
            var _destDir = await _configsService.GetConfigValue("DESTDIR");

            foreach (var file in filesToMove)
            {
                try
                {
                    var _file = await _context.FilesDetail.FindAsync(file.Id);

                    if (_file == null)
                    {
                        _logger.LogWarning($"File with id {file.Id} is not present.");
                        await _notificationHub.Clients.All.SendAsync("moveFilesNotifications", file.FileCategory,
                            $"fileName with id {file.Id} not present", MoveFilesResults.IdNotPresent);
                        continue;
                    }

                    var fileOrigin = Path.Combine(_originDir, _file.Name);
                    var folderDest = Path.Combine(_destDir, file.FileCategory);
                    var fileDest = Path.Combine(folderDest, _file.Name);

                    // Determine whether the directory exists.
                    if (!Directory.Exists(folderDest))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(folderDest);
                        _logger.LogInformation($"Destination folder {folderDest} created");
                    }

                    // Move the file.
                    File.Move(fileOrigin, fileDest);
                    _logger.LogInformation($"{fileOrigin} moved to {fileDest}.");
                    fileMoved++;
                    await _notificationHub.Clients.All.SendAsync("moveFilesNotifications", _file.Id,
                        $"fileName = {_file.Name} moved", MoveFilesResults.Moved);
                    //update db
                    _file.FileCategory = file.FileCategory;

                    _file.IsToCategorize = false;
                    _file.IsNew = false;
                    _file.IsNotToMove = false;

                    var result = await _serviceData.UpdateFilesDetail(_file);
                    _logger.LogInformation($"Db updated: File {_file.Name}");

                    //add train data
                    using (StreamWriter sw = File.AppendText(Path.Combine(
                               _configsService.GetConfigValue("TRAINDATAPATH").Result,
                               _configsService.GetConfigValue("TRAINDATANAME").Result)))
                    {
                        sw.WriteLine(_file.Id + ";" + _file.FileCategory + ";" + _file.Name);
                    }

                    _logger.LogInformation($"File: {_file.Name} added to train model file");
                    await _notificationHub.Clients.All.SendAsync("moveFilesNotifications", _file.Id,
                        $"fileName = {_file.Name} completed", MoveFilesResults.Completed);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Move File process failed: {e.Message}");
                    await _notificationHub.Clients.All.SendAsync("moveFilesNotifications", file.Id,
                        $"Error in moving fileName with id {file.Id}: {e.Message}", MoveFilesResults.Failed);
                }
            }

            var jobExecutionTime = _utility.TimeDiff(jobStartTime, DateTime.Now);
            _logger.LogInformation($"Job Completed: [{jobExecutionTime}]");
            await _notificationHub.Clients.All.SendAsync("jobNotifications",
                $"Completed Job in [{jobExecutionTime}] - Moved {fileMoved} files", MoveFilesResults.Completed);
        }

        public async Task RefreshFiles(CancellationToken cancellationToken)
        {
            var jobStartTime = DateTime.Now;
            await _notificationHub.Clients.All.SendAsync("refreshFilesNotifications", $"Started refresh Files", 0);

            _logger.LogInformation("Start RefreshFile process");

            var _origDir = await _configsService.GetConfigValue("ORIGINDIR");

            int totalFilesInFolder = 0;
            int fileAdded = 0;

            DirectoryInfo dirFilesInfo = new DirectoryInfo(_origDir);

            var allFilesInOrigDir = dirFilesInfo.GetFileSystemInfos();

            var filesInOrigDir = allFilesInOrigDir.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));


            try
            {
                var filesToAddToDb = new List<FilesDetail>();
                //adding new files
                foreach (FileInfo file in filesInOrigDir)
                {
                    //TODO manage folder

                    totalFilesInFolder += 1;

                    if (!_serviceData.FileNameIsPresent(file.Name).Result)
                    {
                        //file in orig folder is not present in DB
                        _logger.LogInformation($"file {file.Name} not present in db");

                        filesToAddToDb.Add(new FilesDetail
                        {
                            Name = file.Name,
                            FileSize = file.Length,
                            LastUpdateFile = file.LastWriteTime,
                            Path = file.Directory.FullName,
                            IsToCategorize = true,
                            IsNew = true
                        });
                    }
                }


                if (filesToAddToDb != null && filesToAddToDb.Count > 0)
                {
                    //calculate category
                    _logger.LogInformation("Start Prediction process");
                    var _categorizationStartTime = DateTime.Now;
                    var _categorizedFiles = _machineLearningService.PredictFileCategorization(filesToAddToDb);

                    _logger.LogInformation(
                        $"End Prediction process: [{_utility.TimeDiff(_categorizationStartTime, DateTime.Now)}]");

                    //add to db
                    _logger.LogInformation("Start add process");
                    var _addStartTime = DateTime.Now;
                    fileAdded = await _serviceData.AddFileDetailList(_categorizedFiles);

                    _logger.LogInformation($"End adding process: [{_utility.TimeDiff(_addStartTime, DateTime.Now)}]");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error in refresh files :{ex.Message}");
                await _notificationHub.Clients.All.SendAsync("refreshFilesNotifications",
                    $"Error in refresh files :{ex.Message}", 0);
            }

            _logger.LogInformation($"End RefreshFile process: [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");

            var jobExecutionTime = _utility.TimeDiff(jobStartTime, DateTime.Now);
            _logger.LogInformation($"Refresh Files Job Completed: [{jobExecutionTime}]");
            await _notificationHub.Clients.All.SendAsync("jobNotifications",
                $"Refresh Files job Completed in [{jobExecutionTime}] - Added {fileAdded} files, total files in folder: {totalFilesInFolder}",
                0);
        }

        public async Task ExecuteFullDeploy(int dockerConfigId,
            CancellationToken cancellationToken)
        {
            var logPath = await GetLogPath(dockerConfigId);
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logPath.Data).CreateLogger();
            
            Thread.Sleep(1000);

            var jobStartTime = DateTime.Now;


            WriteLogInfo($"Start deploy id: {dockerConfigId} at {jobStartTime}");

            //Create Docker File
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"Creating dockerfile...",
                JobResult.InProgress);
            var dockerFileCreated = await _deployActions.CreateDockerFile(dockerConfigId);

            if (!dockerFileCreated.IsSuccess)
            {
                WriteLogWarning(dockerFileCreated.Data);
                _logger.LogWarning(
                    $"Job Completed with error {dockerFileCreated.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");
                await _notificationHub.Clients.All.SendAsync("jobNotifications",
                    $"Job Completed with error {dockerFileCreated.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]",
                    JobResult.Failed);

                await Log.CloseAndFlushAsync();
            }

            //Upload Docker File

            WriteLogInfo(dockerFileCreated.Data);
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"{dockerFileCreated.Data}",
                JobResult.InProgress);
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"Creating dockerfile...",
                JobResult.InProgress);
            var uploadDockerFile = await _deployActions.UploadDockerFile(dockerConfigId);

            if (!uploadDockerFile.IsSuccess)
            {
                WriteLogWarning(uploadDockerFile.Data);
                _logger.LogWarning(
                    $"Job Completed with error {uploadDockerFile.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");
                await _notificationHub.Clients.All.SendAsync("jobNotifications",
                    $"Job Completed with error {uploadDockerFile.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]",
                    JobResult.Failed);

                await Log.CloseAndFlushAsync();
            }

            //Remove running containers
            WriteLogInfo(uploadDockerFile.Data);
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"{uploadDockerFile.Data}",
                JobResult.InProgress);

            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"Remove running containers ...",
                JobResult.InProgress);
            var removeRunningContainers = await _deployActions.RemoveRemoteRunningContainers(dockerConfigId);
            await _notificationHub.Clients.All.SendAsync("jobNotifications",
                $"{removeRunningContainers.Data}", JobResult.InProgress);
            WriteLogInfo(removeRunningContainers.Data);

            //Remove images
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"Deleting images...",
                JobResult.InProgress);
            var deleteImages = await _deployActions.RemoveRemoteImagesList(dockerConfigId);
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"{deleteImages.Data}",
                JobResult.InProgress);
            WriteLogInfo(deleteImages.Data);


            //Build image
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"Building image...",
                JobResult.InProgress);
            WriteLogInfo($"Building image...");
            //
            var buildImage = await _deployActions.BuildImage(dockerConfigId);

            if (!buildImage.IsSuccess)
            {
                WriteLogWarning(buildImage.Data);
                _logger.LogWarning(
                    $"Job Completed with error {buildImage.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");
                await _notificationHub.Clients.All.SendAsync("jobNotifications",
                    $"Job Completed with error {buildImage.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]",
                    JobResult.Failed);

                await Log.CloseAndFlushAsync();
            }

            //Run container
            WriteLogInfo(buildImage.Data);
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"{buildImage.Data}",
                JobResult.InProgress);

            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"Running container...",
                JobResult.InProgress);
            var runContainer = await _deployActions.RunContainer(dockerConfigId);

            if (!runContainer.IsSuccess)
            {
                WriteLogWarning(runContainer.Data);
                _logger.LogWarning(
                    $"Job Completed with error {runContainer.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");
                await _notificationHub.Clients.All.SendAsync("jobNotifications",
                    $"Job Completed with error {runContainer.Data} in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]",
                    JobResult.Failed);

                await Log.CloseAndFlushAsync();
            }

            WriteLogInfo(runContainer.Data);
            await _notificationHub.Clients.All.SendAsync("jobNotifications", $"{runContainer.Data}",
                JobResult.InProgress);


            //close deploy
            _logger.LogInformation($"Job Completed: [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");
            WriteLogInfo($"Job Completed: [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]");
            await _notificationHub.Clients.All.SendAsync("jobNotifications",
                $"Completed Job in [{_utility.TimeDiff(jobStartTime, DateTime.Now)}]", JobResult.Completed);

            await Log.CloseAndFlushAsync();
        }

        #region Log

        public async Task<DockerCommandResponse<string>> GetLogPath(int deployId)
        {
            var dockerConfig = await _context.DockerConfig.FindAsync(deployId);
            if (dockerConfig == null)
            {
                _logger.LogWarning("No configuration found");
                return new DockerCommandResponse<string>($"No configuration found for id {deployId}",
                    "FindDockerConfig", false);
            }
            
            var logPath = $"{dockerConfig.FolderContainer1}/Logs/{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";

            //for debug only
            if (_config.GetSection("IsDev").Value != null)
            {
                logPath = $"{_config.GetSection("deployLogPath").Value} {DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
            }

            if(string.IsNullOrEmpty(logPath))
            {return new DockerCommandResponse<string>("Log path is empty", "GetLogPath", false);}

            try
            {
                var deployDetail = new DeployDetail
                {
                    DockerConfig = dockerConfig,
                    CreatedDate = DateTime.Now,
                    DeployStart = DateTime.Now,
                    IsActive = true,
                    LogFilePath = logPath, Id = 0, LastUpdatedDate= DateTime.Now

                };
            
                var _deployDetail = await _context.DeployDetails.AddAsync(deployDetail);
                await _context.SaveChangesAsync();
                
                return new DockerCommandResponse<string>(logPath, "GetLogPath", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetLogPath: {ex.Message}");
                return new DockerCommandResponse<string>($"Error in GetLogPath: {ex.Message}", "GetLogPath", false);
            }
            
        }
        
        public string WriteLogInfo(string message)
        {
            Log.Information(message);

            return DateTime.Now + " - " + message + System.Environment.NewLine;
        }

        public string WriteLogError(string message)
        {
            Log.Error(message);

            return DateTime.Now + " - " + message + System.Environment.NewLine;
        }

        public string WriteLogWarning(string message)
        {
            Log.Warning(message);

            return DateTime.Now + " - " + message + System.Environment.NewLine;
        }

        public string GetSerilogPath(DockerConfig variables)
        {
            var logPath = variables.FolderContainer1 + "/Logs/" + (DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");

            if (_config.GetSection("LocalPath").Value != null)
            {
                logPath = _config.GetSection("LocalPath").Value + "Logs\\" +
                          (DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
            }

            return logPath;
        }

        #endregion
    }
}