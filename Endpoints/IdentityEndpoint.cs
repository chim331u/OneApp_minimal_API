using System.Text.Json;
using OneApp_minimalApi.Contracts.Identity;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Models.Identity;

namespace OneApp_minimalApi.Endpoints;

public static class IdentityEndpoint
{
    public static IEndpointRouteBuilder MapIdentityEndPoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/Signup", async (SignupModelDto model, IIdentityService identityService) =>
        {
            var signupResult = await identityService.Signup(model);
            
            if (signupResult == null)
            {
                return Results.BadRequest("Failed to create user");
            }

            switch (signupResult.Message)
            {
                case "BadRequest":
                    return Results.BadRequest(signupResult.Data);
                case "Ok":
                    return Results.Ok(signupResult.Data);
            }

            return Results.BadRequest();
        });
        
        app.MapPost("/Login", async (LoginModelDto model, IIdentityService identityService) =>
        {
            var loginResult = await identityService.Login(model);
            
            if (loginResult == null)
            {
                return Results.BadRequest("Failed to login user");
            }

            switch (loginResult.Message)
            {
                case "BadRequest":
                    return Results.BadRequest(loginResult.Data);
                case "Unothorized":
                    return Results.Unauthorized();
                case "Ok":
                    return Results.Ok(JsonSerializer.Deserialize<TokenModelDto>(loginResult.Data));
            }

            return Results.BadRequest();
        });
        
        return app;
    }    
}