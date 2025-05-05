using Hangfire;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

/// <summary>
/// Provides extension methods to map Docker command-related endpoints.
/// </summary>
public static class DockerCommandEnpoint
{
    /// <summary>
    /// Maps the Docker command-related endpoints to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoints to.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> with the mapped endpoints.</returns>
    public static IEndpointRouteBuilder MapDockerCommandEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints

        /// <summary>
        /// Endpoint to send an SSH command to a Docker container.
        /// </summary>
        /// <param name="id">The unique identifier of the Docker container.</param>
        /// <param name="command">The SSH command to execute.</param>
        /// <param name="_dockerCommandService">The service to process the command.</param>
        app.MapPost("/SendSSHCommand/{id:int}", async (int id, string command, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.SendSSHCommand(id, command);
            return Results.Ok(commandResult);
        });

        /// <summary>
        /// Endpoint to send a localhost command to a Docker container.
        /// </summary>
        /// <param name="id">The unique identifier of the Docker container.</param>
        /// <param name="command">The localhost command to execute.</param>
        /// <param name="_dockerCommandService">The service to process the command.</param>
        app.MapPost("/SendLocalhostCommand/{id:int}", async (int id, string command, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.SendLocalhostCommand(id, command);
            return Results.Ok(commandResult);
        });

        /// <summary>
        /// Endpoint to create a Docker file for a specific container.
        /// </summary>
        /// <param name="id">The unique identifier of the Docker container.</param>
        /// <param name="_dockerCommandService">The service to process the request.</param>
        app.MapGet("/CreateDockerFile/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.CreateDockerFile(id);
            return Results.Ok(commandResult);
        });

        app.MapGet("/GetRunningContainersCommand/", async (IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetRunningContainersCommand();
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/GetRemoteRunningContainers/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetRemoteRunningContainers(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/RemoveRemoteRunningContainers/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.RemoveRemoteRunningContainers(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/GetImageListCommand/", async (IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetImageListCommand();
            return Results.Ok(commandResult);
        });

        app.MapGet("/GetRemoteImageList/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetRemoteImageList(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/RemoveRemoteImagesList/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.RemoveRemoteImagesList(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/GetBuildCommand/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetBuildCommand(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/BuildImage/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            //todo: move to hangfire
            var commandResult = await _dockerCommandService.BuildImage(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/GetRunContainerCommand/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetRunContainerCommand(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/RunContainer/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.RunContainer(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/ExecuteFullDeploy/{id:int}", async (int id, IHangFireJobService jobService) =>
        {
            var jobId = BackgroundJob.Enqueue<IHangFireJobService>(job => job.ExecuteFullDeploy(id, CancellationToken.None));

            return string.IsNullOrEmpty(jobId) ? Results.BadRequest("Failed to start job") : Results.Ok(jobId);
        });
        
        app.MapGet("/GetPushImageCommand/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.GetPushImageCommand(id);
            return Results.Ok(commandResult);
        });
        
        app.MapGet("/PushImage/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.PushImage(id);
            return Results.Ok(commandResult);
        });
        
        return app;
    }
}