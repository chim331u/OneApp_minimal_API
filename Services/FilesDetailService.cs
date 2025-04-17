using fc_minimalApi.AppContext;
using fc_minimalApi.Interfaces;

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

    }
}