using fc_minimalApi.Contracts.FilesDetail;
using fc_minimalApi.Interfaces;
using Hangfire;

namespace fc_minimalApi.Endpoints;

public static class UtilitiesEndpoint
{
    
    public static IEndpointRouteBuilder MapUtilitiesEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints
            
        // Endpoint to crypt a string
        app.MapPost("/EncryptString", async (string plainText, IUtilityServices utilityServices) =>
        {
            string cryptedtext  = await utilityServices.EncryptString(plainText);
            
            return Results.Ok(cryptedtext);
        });
        
        // Endpoint to decrypt a string
        app.MapPost("/DencryptString", async (string cryptedText, IUtilityServices utilityServices) =>
        {
            string decryptedtext  = await utilityServices.DecryptString(cryptedText);
            
            return Results.Ok(decryptedtext);
        });
        
        // Endpoint to get hash of a string
        app.MapPost("/Hash256String", async (string text, IUtilityServices utilityServices) =>
        {
            string hashedtext  = await utilityServices.HashString_SHA256(text);
            
            return Results.Ok(hashedtext);
        });
        
        // Endpoint to verify a string with an existing hash
        app.MapPost("/VerifyHash", async (string text, string hashedText, IUtilityServices utilityServices) =>
        {
            bool areEquals  = await utilityServices.VerifyHash_SHA256(text, hashedText);
            
            return Results.Ok(areEquals);
        });
        
        return app;
    }
}