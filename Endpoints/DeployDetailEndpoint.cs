using OneApp_minimalApi.Contracts.DockerDeployer;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

public static class DeployDetailEndpoint
{
    public static IEndpointRouteBuilder MapDeployDetailEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints

        // Endpoint to get a list of detail list by docker config id
        app.MapGet("/GetDeployDetailList/{id:int}", async (int id,IDeployDetailService _deopDeployDetailService) =>
        {
            var result = await _deopDeployDetailService.GetDeployDetails(id);
            return Results.Ok(result);
        });
        
        // Endpoint to get the deploy detail by ID
        app.MapGet("/GetDeployDetail/{id:int}", async (int id, IDeployDetailService _deopDeployDetailService) =>
        {
            var result = await _deopDeployDetailService.GetDeployDetailById(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
        
        // Endpoint to add a new Deploy Detail
        app.MapPost("/AddDeployDetail", async (DeployDetailDto deployDetailDto, IDeployDetailService _deopDeployDetailService) =>
        {
            var result = await _deopDeployDetailService.AddDeployDetail(deployDetailDto);
            return Results.Created($"/GetDeployDetail/{result!.Id}", result); 
        });
        
        // Endpoint to update a Deploy Detail
        app.MapPut("/UpdateDeployDetail/{id:int}", async (int id, DeployDetailDto deployDetailDto, IDeployDetailService _deopDeployDetailService) =>
        {
            var result = await _deopDeployDetailService.UpdateDeployDetail(id, deployDetailDto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
        
        // Endpoint to delete a Deploy Detail
        app.MapDelete("/DeleteDeployDetail/{id:int}", async (int id, IDeployDetailService _deopDeployDetailService) =>
        {
            var result = await _deopDeployDetailService.DeleteDeployDetail(id);
            return result ? Results.NoContent() : Results.NotFound();
        });
        
        return app;
    }
}