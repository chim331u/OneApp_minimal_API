using System.Net;
using fc_minimalApi.Interfaces;
using Hangfire;
using Microsoft.IdentityModel.Tokens;


namespace fc_minimalApi.Endpoints;

public static class DDEndpoint
{
    public static IEndpointRouteBuilder MapDDEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints
        
        app.MapGet("/CheckLink/{link}", async (string link, IDDService _service) =>
        {
            //_logger.LogWarning($"Received request to get links for {link}");

            var decodedStr = Base64UrlEncoder.Decode(link);
            decodedStr = WebUtility.UrlDecode(decodedStr);
            decodedStr = WebUtility.HtmlDecode(decodedStr);

            string? result = await _service.GetLinks(decodedStr);

            if (string.IsNullOrEmpty(result))
            {
                return Results.NotFound();
            }

            return Results.Ok(result);
        });
        
        app.MapGet("/CheckLinks/{threadId}", async (int threadId, IDDService _service) =>
        {
            // _logger.LogWarning($"Received request to get links for thread id {threadId}");

            string? result = await _service.GetLinks(threadId);

            if (string.IsNullOrEmpty(result))
            {
                return Results.NotFound();
            }

            return Results.Ok(result);
        });
        
        app.MapGet("/GetLinks/{id}", async (int id, IDDService _service) =>
        {
            // _logger.LogWarning($"Received request to get links for thread id {threadId}");

            var result = await _service.GetActiveLinks(id);

            return Results.Ok(result);
        });
        
        app.MapGet("/UseLink/{id}", async (int id, IDDService _service) =>
        {
            // _logger.LogWarning($"Received request to get links for thread id {threadId}");

            var result = await _service.UseLink(id);
            
            if (string.IsNullOrEmpty(result))
            {
                return Results.NotFound();
            }
            
            return Results.Ok(result);
        });
        
        app.MapGet("/GetActiveThreads", async (IDDService _service) =>
        {
            // _logger.LogWarning($"Received request to get links for thread id {threadId}");

            var result = await _service.GetActiveThreads();

            return Results.Ok(result);
        });
        
        // // Endpoint to update files from dir to db
        // app.MapGet("/RefreshFiles", async (IHangFireJobService jobService) =>
        // {
        //     string jobId =
        //         BackgroundJob.Enqueue<IHangFireJobService>(job =>
        //             job.RefreshFiles(CancellationToken.None));
        //     
        //     return Results.Ok(jobId);
        // });
        
        return app;
    }
}