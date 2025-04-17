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


            return app;
        }
    }
}