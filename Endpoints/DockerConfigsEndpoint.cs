using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;

// ReSharper disable InconsistentNaming

namespace OneApp_minimalApi.Endpoints;

/// <summary>
/// Provides extension methods to map Docker configuration-related endpoints.
/// </summary>
public static class DockerConfigsEndpoint
{
    /// <summary>
    /// Maps the Docker configuration-related endpoints to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoints to.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> with the mapped endpoints.</returns>
    public static IEndpointRouteBuilder MapDockerConfigsEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints

        /// <summary>
        /// Endpoint to get a list of Docker configurations.
        /// </summary>
        /// <param name="_dockerConfigsService">The service to retrieve the Docker configurations.</param>
        app.MapGet("/GetDockerConfigList", async (IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.GetDockerConfigList();
            return Results.Ok(result);
        });

        /// <summary>
        /// Endpoint to get a Docker configuration by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the Docker configuration.</param>
        /// <param name="_dockerConfigsService">The service to retrieve the Docker configuration.</param>
        app.MapGet("/GetDockerConfig/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.GetDockerConfig(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to add a new Docker configuration.
        /// </summary>
        /// <param name="dockerConfigsDto">The Docker configuration data to add.</param>
        /// <param name="_dockerConfigsService">The service to process the request.</param>
        app.MapPost("/AddDockerConfig",
            async (DockerConfigsDto dockerConfigsDto, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.AddDockerConfig(dockerConfigsDto);
                return Results.Created($"/GetDockerConfig/{result!.Id}", result);
            });

        /// <summary>
        /// Endpoint to update an existing Docker configuration.
        /// </summary>
        /// <param name="id">The unique identifier of the Docker configuration to update.</param>
        /// <param name="dockerConfigsDto">The updated Docker configuration data.</param>
        /// <param name="_dockerConfigsService">The service to process the request.</param>
        app.MapPut("/UpdateDockerConfig/{id:int}",
            async (int id, DockerConfigsDto dockerConfigsDto, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.UpdateDockerConfig(id, dockerConfigsDto);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });

        /// <summary>
        /// Endpoint to delete a Docker configuration by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the Docker configuration to delete.</param>
        /// <param name="_dockerConfigsService">The service to process the request.</param>
        app.MapDelete("/DeleteDockerConfig/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.DeleteDockerConfig(id);
            return result ? Results.NoContent() : Results.NotFound();
        });


        app.MapGet("/GetDockerParameterList/{id:int}",
            async (int id, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.GetDockerParametersList(id);
                return Results.Ok(result);
            });

        app.MapGet("/GetDockerParameter/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.GetDockerParameter(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        app.MapPost("/AddDockerParameter",
            async (DockerParameterDto dockerParamDto, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.AddDockerParameter(dockerParamDto);
                return Results.Created($"/GetDockerParameter/{result!.Id}", result);
            });

        app.MapPut("/UpdateDockerParameter/{id:int}",
            async (int id, DockerParameterDto dockerParamDto, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.UpdateDockerParameter(id, dockerParamDto);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });
        app.MapDelete("/DeleteDockerParameter/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.DeleteDockerParameter(id);
            return result ? Results.NoContent() : Results.NotFound();
        });
        app.MapGet("/GetDockerFolderMountsList/{id:int}",
            async (int DockerConfigId, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.GetDockerFolderMountsList(DockerConfigId);
                return Results.Ok(result);
            });
        app.MapGet("/GetDockerFolderMounts/{id:int}", async (int id, IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.GetDockerFolderMounts(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        app.MapPost("/AddDockerFolderMounts",
            async (DockerFolderMountsDto dockerFolderMountsDto, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.AddDockerFolderMounts(dockerFolderMountsDto);
                return Results.Created($"/GetDockerFolderMounts/{result!.Id}", result);
            });
        app.MapPut("/UpdateDockerFolderMounts/{id:int}", async (int id, DockerFolderMountsDto dockerFolderMountsDto,
            IDockerConfigsService _dockerConfigsService) =>
        {
            var result = await _dockerConfigsService.UpdateDockerFolderMounts(id, dockerFolderMountsDto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
        app.MapDelete("/DeleteDockerFolderMounts/{id:int}",
            async (int id, IDockerConfigsService _dockerConfigsService) =>
            {
                var result = await _dockerConfigsService.DeleteDockerFolderMounts(id);
                return result ? Results.NoContent() : Results.NotFound();
            });
        
        
        return app;
    }
}