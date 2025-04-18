using fc_minimalApi.Contracts.Configs;
using fc_minimalApi.Interfaces;

namespace fc_minimalApi.Endpoints;

public static class ConfigsEndpoint
{
    public static IEndpointRouteBuilder MapConfigsEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints
            
        // Endpoint to get a list of configs
        app.MapGet("/GetConfigList", async (IConfigsService configsService) =>
        {
            var result = await configsService.GetConfigList();
            return Results.Ok(result);
        });
        
        //endpoint to get a config by ID
        app.MapGet("/GetConfig/{id:int}", async (int id, IConfigsService configsService) =>
        {
            var result = await configsService.GetConfig(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
        
        // Endpoint to add a new config
        app.MapPost("/AddConfig", async (ConfigsDto configsDto, IConfigsService configsService) =>
        {
            var result = await configsService.AddConfig(configsDto);
            return Results.Created($"/GetConfig/{result.Id}", result); 
        });
        
        // Endpoint to update a config
        app.MapPut("/UpdateConfig/{id:int}", async (int id, ConfigsDto configsDto, IConfigsService configsService) =>
        {
            var result = await configsService.UpdateConfig(id, configsDto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
        
        // Endpoint to delete a config
        app.MapDelete("/DeleteConfig/{id:int}", async (int id, IConfigsService configsService) =>
        {
            var result = await configsService.DeleteConfig(id);
            return result ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}