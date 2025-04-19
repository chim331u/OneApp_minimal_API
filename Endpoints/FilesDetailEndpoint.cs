using fc_minimalApi.Contracts.FilesDetail;
using fc_minimalApi.Interfaces;

namespace fc_minimalApi.Endpoints
{
    public static class FilesDetailEndPoint
    {
        public static IEndpointRouteBuilder MapFilesDetailEndPoint(this IEndpointRouteBuilder app)
        {
            // Define the endpoints
            
            // Endpoint to get a list of categorites
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
            
            // Endpoint to get file list
            app.MapGet("/GetFileList/{searchPar}", async (string searchPar, IFilesDetailService filesDetailService) =>
            {
                //all
                var fileList = await filesDetailService.GetFileList();

                switch (searchPar)
                {
                    //to categorize
                    case "3":
                        fileList = fileList.Where(x => x.IsToCategorize).ToList();
                        break;

                    //categorized
                    case "2":
                        fileList = fileList.Where(x => x.IsToCategorize == false).ToList();
                        break;

                    default:
                        break;
                }
                
                return Results.Ok(fileList);
            });
            
            // Endpoint to get a filesDetail by ID
            app.MapGet("/GetFilesDetail/{id:int}", async (int id, IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.GetFilesDetailById(id);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });

            app.MapGet("/GetFileToMove", async (IFilesDetailService filesDetailService) =>
                {
                    var result = await filesDetailService.GetFileListToCategorize();
                    return result != null ? Results.Ok(result) : Results.NotFound();
                }
            );
            
            // Endpoint to add a new filesDetail
            app.MapPost("/AddFilesDetail", async (FilesDetailRequest filesDetailRequest, IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.AddFileDetailAsync(filesDetailRequest);
                return Results.Created($"/GetFilesDetail/{result.Id}", result); 
            });

            // Endpoint to updatte a filesDetail
            app.MapPut("/UpdateFilesDetail/{id:int}", async (int id, FilesDetailUpdateRequest filesDetailUpdateRequest, IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.UpdateFilesDetailAsync(id, filesDetailUpdateRequest);
                return result != null ? Results.Ok(result) : Results.NotFound();
            });
            
            
            // Endpoint to delete a filesDetail
            app.MapDelete("/DeleteFilesDetail/{id:int}", async (int id, IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.DeleteFilesDetailAsync(id);
                return result ? Results.NoContent() : Results.NotFound();
            });

            // Endpoint to get all filesDetail
            app.MapGet("/GetLastViewList", async (IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.GetLastViewList();
                return Results.Ok(result);
            });
            
            // Endpoint to get all filesDetail
            app.MapGet("/GetAllFiles/{cat}", async (string cat,IFilesDetailService filesDetailService) =>
            {
                var result = await filesDetailService.GetAllFiles(cat);
                return Results.Ok(result);
            });
            
            return app;
        }
    }
}