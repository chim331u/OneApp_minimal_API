using OneApp_minimalApi.Contracts.Vault;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

public static class HashicorpVaultEndPoint
{
    
    public static IEndpointRouteBuilder MapHashiCorpVaultEndPoint(this IEndpointRouteBuilder app)
    {

        /// <summary>
        /// Endpoint to mangage secrets in a server valut.
        /// </summary>
        app.MapGet("/GetSecret", async (string Key, string path, string mountPoint, IHashicorpVaultService _service) =>
        {
            var result = await _service.GetSecret(Key, path, mountPoint);

            if (result.Data == null)
            {
                return Results.NotFound(result.Message);
            }
            
            return Results.Ok(result.Data);
        });
        
        /// <summary>
        /// Endpoint to mangage secrets in a server valut.
        /// </summary>
        app.MapGet("/GetSecretList", async (string path, string mountPoint, IHashicorpVaultService _service) =>
        {
            var result = await _service.GetListSecretsKeys( path, mountPoint);
        
            if (result.Data == null)
            {
                return Results.NotFound(result);
            }
            
            return Results.Ok(result);
        });
        
        app.MapPost("/AddSecret", async (SecretRequestDTO secret, IHashicorpVaultService _service) =>
        {
            var result = await _service.CreateSecret(secret);
            
            if (result.Data == null)
            {
                return Results.BadRequest(result.Message);
            }
            
            return Results.Ok(result.Data);
        
        });
        
        app.MapPut("/UpdateSecret/", async (SecretRequestDTO secret, IHashicorpVaultService _service) =>
        {
            var result = await _service.UpdateSecret(secret);
            
            if (result.Data == null)
            {
                return Results.BadRequest(result.Message);
            }
            
            return Results.Ok(result.Data);
        
        });
        
        // app.MapPut("/ChangeSecretPsw/{id:int}", async (int id, SecretRequestDTO secret, IHashicorpVaultService _service) =>
        // {
        //     var result = await _service.UpdateSecret(id, secret, true);
        //     
        //     if (result.Data == null)
        //     {
        //         return Results.BadRequest(result.Message);
        //     }
        //     
        //     return Results.Ok(result.Data);
        //
        // });
        
        // app.MapDelete("/DeleteSecret/{id:int}", async (int id, IHashicorpVaultService _service) =>
        // {
        //     var result = await _service.DeleteSecret(id);
        //     
        //     if (!result.Data)
        //     {
        //         return Results.NotFound(result.Message);
        //     }
        //     
        //     return Results.NoContent();
        // });
        
        // /// <summary>
        // /// Endpoint to mangage secrets in a local valut.
        // /// </summary>
        // app.MapGet("/GetHistoricalSecretsList", async (IHashicorpVaultService _service) =>
        // {
        //     var result = await _service.GetHistorySecretList();
        //
        //     if (result.Data == null)
        //     {
        //         return Results.NotFound(result.Message);
        //     }
        //     
        //     return Results.Ok(result.Data);
        // });
        return app;
    }
}