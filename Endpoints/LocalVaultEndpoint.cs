using Hangfire;
using OneApp_minimalApi.Contracts.Vault;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

public static class LocalVaultEndpoint
{
    public static IEndpointRouteBuilder MapLocalVaultEndPoint(this IEndpointRouteBuilder app)
    {
        
        app.MapGet("/CreateDb", async (ILocalVaultService _service) =>
        {
            var result = await _service.CreateDatabase();

            if (!result.Data)
            {
                return Results.BadRequest(result.Message);
            }
            
            return Results.Ok(result.Message);
        });
        
        
        /// <summary>
        /// Endpoint to mangage secrets in a local valut.
        /// </summary>
        app.MapGet("/GetSecret", async (string Key, ILocalVaultService _service) =>
        {
            var result = await _service.GetSecret(Key);

            if (result.Data == null)
            {
                return Results.NotFound(result.Message);
            }
            
            return Results.Ok(result.Data);
        });
        
        /// <summary>
        /// Endpoint to mangage secrets in a local valut.
        /// </summary>
        app.MapGet("/GetSecretList", async (ILocalVaultService _service) =>
        {
            var result = await _service.GetListSecrets();

            if (result.Data == null)
            {
                return Results.NotFound(result.Message);
            }
            
            return Results.Ok(result.Data);
        });

        app.MapPost("/AddSecret", async (SecretRequestDTO secret, ILocalVaultService _service) =>
        {
            var result = await _service.StoreSecret(secret);
            
            if (result.Data == null)
            {
                return Results.BadRequest(result.Message);
            }
            
            return Results.Ok(result.Data);

        });
        
        app.MapPut("/UpdateSecret/{id:int}", async (int id, SecretRequestDTO secret, ILocalVaultService _service) =>
        {
            var result = await _service.UpdateSecret(id, secret);
            
            if (result.Data == null)
            {
                return Results.BadRequest(result.Message);
            }
            
            return Results.Ok(result.Data);

        });
        
        app.MapPut("/ChangeSecretPsw/{id:int}", async (int id, SecretRequestDTO secret, ILocalVaultService _service) =>
        {
            var result = await _service.UpdateSecret(id, secret, true);
            
            if (result.Data == null)
            {
                return Results.BadRequest(result.Message);
            }
            
            return Results.Ok(result.Data);

        });
        
        app.MapDelete("/DeleteSecret/{id:int}", async (int id, ILocalVaultService _service) =>
        {
            var result = await _service.DeleteSecret(id);
            
            if (!result.Data)
            {
                return Results.NotFound(result.Message);
            }
            
            return Results.NoContent();
        });

        /// <summary>
        /// Endpoint to mangage secrets in a local valut.
        /// </summary>
        app.MapGet("/GetHistoricalSecretsList", async (ILocalVaultService _service) =>
        {
            var result = await _service.GetHistorySecretList();

            if (result.Data == null)
            {
                return Results.NotFound(result.Message);
            }
            
            return Results.Ok(result.Data);
        });
        return app;
    }
}
