using Hangfire;
using OneApp_minimalApi.Contracts.FilesDetail;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

public static class ActionsEndpoint
{
    
    public static IEndpointRouteBuilder MapActionsEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints
            
        // Endpoint to update files from dir to db
        app.MapGet("/RefreshFiles", async (IHangFireJobService jobService) =>
        {
            string jobId =
                BackgroundJob.Enqueue<IHangFireJobService>(job =>
                    job.RefreshFiles(CancellationToken.None));
            
            return Results.Ok(jobId);
        });
        
        // Endpoint to move files from origdir to destdir
        app.MapPost("/MoveFiles", async (List<FileMoveDto> filetToMoveList, IHangFireJobService jobService) =>
        {
            if (filetToMoveList==null || filetToMoveList.Count<1)
            {
                return Results.BadRequest("No files to move");
            }
            string jobId =
                BackgroundJob.Enqueue<IHangFireJobService>(job =>
                    job.MoveFilesJob(filetToMoveList, CancellationToken.None));
            
            return Results.Ok(jobId);
        });
        
        // Endpoint to force category
        app.MapPost("/ForceCategory", async ( IFilesDetailService filesDetailService) =>
        {
            string forceCategoryResult = await filesDetailService.ForceCategory();
            return Results.Ok(forceCategoryResult);
        
        });
        
        // Endpoint to trian model
        app.MapPost("/TrainModel", async ( IMachineLearningService machineLearningService) =>
        {
            string trainModelResult = machineLearningService.TrainAndSaveModel();
            return Results.Ok(trainModelResult);
        });
        
        return app;
    }
}