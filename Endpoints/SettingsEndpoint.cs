using OneApp_minimalApi.Contracts.Configs;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

public static class SettingsEndpoint
{
    public static IEndpointRouteBuilder MapSettingsEndPoint(this IEndpointRouteBuilder app)
    {
        /// <summary>
        /// Endpoint to get a list of Setting.
        /// </summary>
        app.MapGet("/GetSettingList", async (ISettingsService settingsService) =>
        {
            var result = await settingsService.GetSettingsList();
            return Results.Ok(result);
        });

        /// <summary>
        /// Endpoint to get the full list of Setting.
        /// </summary>
        app.MapGet("/GetSettingFullList", async (ISettingsService settingsService) =>
        {
            var result = await settingsService.GetSettingsFullList();
            return Results.Ok(result);
        });
        /// <summary>
        /// Endpoint to get a Setting by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the Setting.</param>
        app.MapGet("/GetSetting/{id:int}", async (int id, ISettingsService settingsService) =>
        {
            var result = await settingsService.GetSetting(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to add a new Setting.
        /// </summary>
        /// <param name="configsDto">The Setting data to add.</param>
        app.MapPost("/AddSetting", async (SettingsDto settingDto, ISettingsService settingsService) =>
        {
            var result = await settingsService.AddSetting(settingDto);
            return Results.Created($"/GetSetting/{result.Data.Id}", result);
        });

        /// <summary>
        /// Endpoint to update an existing Setting.
        /// </summary>
        /// <param name="id">The unique identifier of the Setting to update.</param>
        /// <param name="configsDto">The updated Setting data.</param>
        app.MapPut("/UpdateSetting/{id:int}",
            async (int id, SettingsDto settingDto, ISettingsService settingsService) =>
            {
                var result = await settingsService.UpdateSetting(id, settingDto);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });

        /// <summary>
        /// Endpoint to delete a Setting by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the Setting to delete.</param>
        app.MapDelete("/DeleteSetting/{id:int}", async (int id, ISettingsService settingsService) =>
        {
            var result = await settingsService.DeleteSetting(id);
            return result.Data ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}