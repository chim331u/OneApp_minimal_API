using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;
// ReSharper disable InconsistentNaming

namespace OneApp_minimalApi.Endpoints;

/// <summary>
/// Docker Config Endpoints
/// </summary>
public static class DockerConfigsEndpoint
{
    /// <summary>
    /// Docker Config Endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapDockerConfigsEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints

        // Endpoint to get a list of docker configs
        app.MapGet("/GetDockerConfigList", async (IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.GetDockerConfigList();
            return Results.Ok(result);
        });
        
        // Endpoint to get the docker config by ID
        app.MapGet("/GetDockerConfig/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.GetDockerConfig(id);
            return Results.Ok(result);
        });
        
        // Endpoint to add a new Docker Config
        app.MapPost("/AddDockerConfig", async (DockerConfigsDto dockerConfigsDto, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.AddDockerConfig(dockerConfigsDto);
            return Results.Created($"/GetDockerConfig/{result!.Id}", result); 
        });

        // Endpoint to update a Docker Config
        app.MapPut("/UpdateDockerConfig/{id:int}", async (int id, DockerConfigsDto dockerConfigsDto, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.UpdateDockerConfig(id, dockerConfigsDto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
            
            
        // Endpoint to delete a Docker Config
        app.MapDelete("/DeleteDockerConfig/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.DeleteDockerConfig(id);
            return result ? Results.NoContent() : Results.NotFound();
        });
        
        
        return app;
    }
}