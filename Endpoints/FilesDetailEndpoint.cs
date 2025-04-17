using fc_minimalApi.Interfaces;

namespace fc_minimalApi.Endpoints
{
    public static class FilesDetailEndPoint
    {
        public static IEndpointRouteBuilder MapFilesDetailEndPoint(this IEndpointRouteBuilder app)
        {
            // Define the endpoints
            
            // Endpoint to get all filesDetail
            app.MapGet("/filesDetail", async (IFilesDetailService filesDetailService) =>
            {
                //var result = await bookService.GetBooksAsync();
                return Results.Ok();
            });


            return app;
        }
    }
}