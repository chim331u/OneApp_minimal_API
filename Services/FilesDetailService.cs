using fc_minimalApi.AppContext;
using fc_minimalApi.Contracts.FilesDetail;
using fc_minimalApi.Interfaces;
using fc_minimalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace fc_minimalApi.Services
{
    /// <summary>
    /// Book Services
    /// </summary>
    public class FilesDetailService : IFilesDetailService
    {
        private readonly ApplicationContext _context; // Database context
        private readonly ILogger<FilesDetailService> _logger; // Logger for logging information and error

        // Constructor to initialize the database context and logger
        /// <summary>
        /// Entry point for Files Detail Service
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger for logging information and error</param>
        public FilesDetailService(ApplicationContext context, ILogger<FilesDetailService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // /// <summary>
        //       /// Get all books
        //       /// </summary>
        //       /// <returns>List of all books</returns>
        //       public async Task<IEnumerable<BookResponse>> GetBooksAsync()
        //       {
        //           try
        //           {
        //               // Get all books from the database
        //               var books = await _context.Books.ToListAsync();
        //
        //               // Return the details of all books
        //               return books.Select(book => new BookResponse
        //               {
        //                   Id = book.Id,
        //                   Title = book.Title,
        //                   Author = book.Author,
        //                   Description = book.Description,
        //                   Category = book.Category,
        //                   Language = book.Language,
        //                   TotalPages = book.TotalPages
        //               });
        //           }
        //           catch (Exception ex)
        //           {
        //               _logger.LogError($"Error retrieving books: {ex.Message}");
        //               throw;
        //           }
        //       }

        /// <summary>
        /// Get the list of Categories
        /// </summary>
        /// <returns>Category list ordered by name</returns>
        public async Task<List<string?>> GetDbCategoryList()
        {
            try
            {
                return await _context.FilesDetail.AsNoTracking().Select(c => c.FileCategory)
                    .Distinct().OrderBy(a => a)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        public async Task<IEnumerable<FilesDetailResponse>> GetFileList()
        {
            try
            {
                var fileDetailList = await _context.FilesDetail.AsNoTracking().OrderBy(x => x.Name)
                    .Where(x => x.IsActive)
                    .ToListAsync();

                // Return the details of all files
                return fileDetailList.Select(fileDetailResposne => new FilesDetailResponse
                {
                    Id = fileDetailResposne.Id,
                    Name = fileDetailResposne.Name,
                    Path = fileDetailResposne.Path,
                    FileCategory = fileDetailResposne.FileCategory,
                    IsToCategorize = fileDetailResposne.IsToCategorize,
                    IsNew = fileDetailResposne.IsNew,
                    FileSize = fileDetailResposne.FileSize,
                    IsNotToMove = fileDetailResposne.IsNotToMove
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get a filed detail by its ID
        /// </summary>
        /// <param name="id">ID of the filesDetail</param>
        /// <returns>Details of the filesDetail</returns>
        public async Task<FilesDetailResponse?> GetFilesDetailById(int id)
        {
            try
            {
                // Find the files detail by its ID
                var filesDetail = await _context.FilesDetail.FindAsync(id);
                if (filesDetail == null)
                {
                    _logger.LogWarning($"Files Detail with ID {id} not found.");
                    return null!;
                }

                // Return the details of the filesDetail
                return new FilesDetailResponse()
                {
                    Id = filesDetail.Id, Name = filesDetail.Name, Path = filesDetail.Path,
                    FileCategory = filesDetail.FileCategory,
                    FileSize = filesDetail.FileSize, IsNotToMove = filesDetail.IsNotToMove,
                    IsToCategorize = filesDetail.IsToCategorize, IsNew = filesDetail.IsNew
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving filesDetail: {ex.Message}");
                return null!;
            }
        }

        /// <summary>
        /// Add new File detail
        /// </summary>
        /// <param name="FileDetatilRequest"></param>
        /// <returns>Details of the created book</returns>
        public async Task<FilesDetailResponse> AddFileDetailAsync(FilesDetailRequest filesDetailRequest)
        {
            try
            {
                var filesDetail = new FilesDetail
                {
                    CreatedDate = DateTime.Now,
                    FileCategory = filesDetailRequest.FileCategory,
                    FileSize = filesDetailRequest.FileSize,
                    IsActive = true, IsDeleted = false, IsNew = true, IsNotToMove = false, IsToCategorize = true,
                    Name = filesDetailRequest.Name, Path = filesDetailRequest.Path
                };

                // Add the book to the database
                _context.FilesDetail.Add(filesDetail);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Files detail added successfully.");

                // Return the details of the created book
                return new FilesDetailResponse
                {
                    Id = filesDetail.Id, FileCategory = filesDetail.FileCategory, FileSize = filesDetail.FileSize,
                    IsNotToMove = filesDetail.IsNotToMove, IsToCategorize = filesDetail.IsToCategorize,
                    IsNew = filesDetail.IsNew,
                    Name = filesDetail.Name, Path = filesDetail.Path
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding files detail: {ex.Message}");
                return null!;
            }
        }
    }
}