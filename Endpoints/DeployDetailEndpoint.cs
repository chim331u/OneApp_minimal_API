using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

/// <summary>
/// Provides extension methods to map Deploy Detail-related endpoints.
/// </summary>
public static class DeployDetailEndpoint
{
    /// <summary>
    /// Maps the Deploy Detail-related endpoints to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoints to.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> with the mapped endpoints.</returns>
    public static IEndpointRouteBuilder MapDeployDetailEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints

        /// <summary>
        /// Endpoint to get a list of deploy details by Docker configuration ID.
        /// </summary>
        /// <param name="id">The Docker configuration ID.</param>
        /// <param name="_deopDeployDetailService">The service to retrieve deploy details.</param>
        app.MapGet("/GetDeployDetailList/{id:int}", async (int id, IDeployDetailService deployDetailService) =>
        {
            var result = await deployDetailService.GetDeployDetails(id);
            return Results.Ok(result);
        });

        /// <summary>
        /// Endpoint to get a deploy detail by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the deploy detail.</param>
        /// <param name="_deopDeployDetailService">The service to retrieve the deploy detail.</param>
        app.MapGet("/GetDeployDetail/{id:int}", async (int id, IDeployDetailService deployDetailService) =>
        {
            var result = await deployDetailService.GetDeployDetailById(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        }); 
        
        app.MapGet("/GetDeployDetailResult/{id:int}", async (int id, IDeployDetailService deployDetailService) =>
        {
            var result = await deployDetailService.GetDeployDetailResult(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to add a new deploy detail.
        /// </summary>
        /// <param name="deployDetailDto">The deploy detail data to add.</param>
        /// <param name="_deopDeployDetailService">The service to process the request.</param>
        app.MapPost("/AddDeployDetail", async (DeployDetailDto deployDetailDto, IDeployDetailService deployDetailService) =>
        {
            var result = await deployDetailService.AddDeployDetail(deployDetailDto);
            return Results.Created($"/GetDeployDetail/{result!.Id}", result);
        });

        /// <summary>
        /// Endpoint to update an existing deploy detail.
        /// </summary>
        /// <param name="id">The unique identifier of the deploy detail to update.</param>
        /// <param name="deployDetailDto">The updated deploy detail data.</param>
        /// <param name="_deopDeployDetailService">The service to process the request.</param>
        app.MapPut("/UpdateDeployDetail/{id:int}", async (int id, DeployDetailDto deployDetailDto, IDeployDetailService _deployDetailService) =>
        {
            var result = await _deployDetailService.UpdateDeployDetail(id, deployDetailDto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
        
        app.MapGet("/UpdateDeployDetailResult/{id:int}/{result}", async (int id, string result, IDeployDetailService _deployDetailService) =>
        {
            var resultUpdate = await _deployDetailService.UpdateDeployDetailResult(id, result);
            return resultUpdate != null ? Results.Ok(resultUpdate) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to delete a deploy detail by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the deploy detail to delete.</param>
        /// <param name="_deopDeployDetailService">The service to process the request.</param>
        app.MapDelete("/DeleteDeployDetail/{id:int}", async (int id, IDeployDetailService _deployDetailService) =>
        {
            var result = await _deployDetailService.DeleteDeployDetail(id);
            return result ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}