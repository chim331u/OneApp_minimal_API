using OneApp_minimalApi.Contracts.FilesDetail;
using OneApp_minimalApi.Interfaces;

namespace OneApp_minimalApi.Endpoints;

/// <summary>
/// Provides extension methods to map file detail-related endpoints.
/// </summary>
public static class FilesDetailEndPoint
{
    /// <summary>
    /// Maps the file detail-related endpoints to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoints to.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> with the mapped endpoints.</returns>
    public static IEndpointRouteBuilder MapFilesDetailEndPoint(this IEndpointRouteBuilder app)
    {
        // Define the endpoints

        /// <summary>
        /// Endpoint to get a list of categories.
        /// </summary>
        /// <param name="filesDetailService">The service to retrieve the categories.</param>
        app.MapGet("/CategoryList", async (IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.GetDbCategoryList();
            return Results.Ok(result);
        });

        /// <summary>
        /// Endpoint to get all file details.
        /// </summary>
        /// <param name="filesDetailService">The service to retrieve the file details.</param>
        app.MapGet("/filesDetailList", async (IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.GetFileList();
            return Results.Ok(result);
        });

        /// <summary>
        /// Endpoint to get a filtered list of file details.
        /// </summary>
        /// <param name="searchPar">The search parameter to filter the file list.</param>
        /// <param name="filesDetailService">The service to retrieve the file details.</param>
        app.MapGet("/GetFileList/{searchPar}", async (string searchPar, IFilesDetailService filesDetailService) =>
        {
            var fileList = await filesDetailService.GetFileList();

            switch (searchPar)
            {
                case "3": // To categorize
                    fileList = fileList.Where(x => x.IsToCategorize).ToList();
                    break;

                case "2": // Categorized
                    fileList = fileList.Where(x => !x.IsToCategorize).ToList();
                    break;

                default:
                    break;
            }

            return Results.Ok(fileList);
        });

        /// <summary>
        /// Endpoint to get file details by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the file detail.</param>
        /// <param name="filesDetailService">The service to retrieve the file detail.</param>
        app.MapGet("/GetFilesDetail/{id:int}", async (int id, IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.GetFilesDetailById(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to get files to move.
        /// </summary>
        /// <param name="filesDetailService">The service to retrieve the files to move.</param>
        app.MapGet("/GetFileToMove", async (IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.GetFileListToCategorize();
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to add a new file detail.
        /// </summary>
        /// <param name="filesDetailRequest">The file detail data to add.</param>
        /// <param name="filesDetailService">The service to process the request.</param>
        app.MapPost("/AddFilesDetail", async (FilesDetailRequest filesDetailRequest, IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.AddFileDetailAsync(filesDetailRequest);
            return Results.Created($"/GetFilesDetail/{result.Id}", result);
        });

        /// <summary>
        /// Endpoint to update an existing file detail.
        /// </summary>
        /// <param name="id">The unique identifier of the file detail to update.</param>
        /// <param name="filesDetailUpdateRequest">The updated file detail data.</param>
        /// <param name="filesDetailService">The service to process the request.</param>
        app.MapPut("/UpdateFilesDetail/{id:int}", async (int id, FilesDetailUpdateRequest filesDetailUpdateRequest, IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.UpdateFilesDetailAsync(id, filesDetailUpdateRequest);
            return result != null ? Results.Ok(result) : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to delete a file detail by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the file detail to delete.</param>
        /// <param name="filesDetailService">The service to process the request.</param>
        app.MapDelete("/DeleteFilesDetail/{id:int}", async (int id, IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.DeleteFilesDetailAsync(id);
            return result ? Results.NoContent() : Results.NotFound();
        });

        /// <summary>
        /// Endpoint to get the last viewed file details.
        /// </summary>
        /// <param name="filesDetailService">The service to retrieve the last viewed file details.</param>
        app.MapGet("/GetLastViewList", async (IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.GetLastViewList();
            return Results.Ok(result);
        });

        /// <summary>
        /// Endpoint to get all files by category.
        /// </summary>
        /// <param name="cat">The category to filter the files.</param>
        /// <param name="filesDetailService">The service to retrieve the files.</param>
        app.MapGet("/GetAllFiles/{cat}", async (string cat, IFilesDetailService filesDetailService) =>
        {
            var result = await filesDetailService.GetAllFiles(cat);
            return Results.Ok(result);
        });

        return app;
    }
}