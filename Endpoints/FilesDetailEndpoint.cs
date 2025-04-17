using fc_minimalApi.Contracts.FilesDetail;
using fc_minimalApi.Interfaces;

namespace fc_minimalApi.Endpoints
{
    public static class FilesDetailEndPoint
    {
        public static IEndpointRouteBuilder MapFilesDetailEndPoint(this IEndpointRouteBuilder app)
        {
            // Define the endpoints
            
            // Endpoint to get list of categorites
            app.MapGet("/CategoryList", async (IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.GetDbCategoryList();
                return Results.Ok(result);
            });
            
            // Endpoint to get all filesDetail
            app.MapGet("/filesDetailList", async (IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.GetFileList();
                return Results.Ok(result);
            });
            
            // Endpoint to get a filesDetail by ID
            app.MapGet("/GetFilesDetail/{id:int}", async (int id, IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.GetFilesDetailById(id);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });
            
            // Endpoint to add a new filesDetail
            app.MapPost("/AddFilesDetail", async (FilesDetailRequest filesDetailRequest, IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.AddFileDetailAsync(filesDetailRequest);
                return Results.Created($"/GetFilesDetail/{result.Id}", result); 
            });


            return app;
        }
    }
}