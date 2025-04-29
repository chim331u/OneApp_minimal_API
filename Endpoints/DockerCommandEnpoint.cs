using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

public static class DockerCommandEnpoint
{
    public static IEndpointRouteBuilder MapDockerCommandEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints
        // Endpoint to send SSH command
        app.MapPost("/SendSSHCommand/{id:int}", async (int id, string command, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.SendSSHCommand(id, command);
            
            return Results.Ok(commandResult);
        });

        // Endpoint to send localhost command
        app.MapPost("/SendLocalhostCommand/{id:int}", async (int id, string command, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.SendLocalhostCommand(id, command);
            
            return Results.Ok(commandResult);
        });

        // Endpoint to create Docker file
        app.MapPost("/CreateDockerFile/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.CreateDockerFile(id);
            
            return Results.Ok(commandResult);
        });
        
        app.MapPost("/BuildCommand/{id:int}", async (int id, IDockerCommandService _dockerCommandService) =>
        {
            var commandResult = await _dockerCommandService.BuildCommand(id);
            
            return Results.Ok(commandResult);
        });
        return app;
    }
}