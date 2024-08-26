using Microsoft.AspNetCore.Mvc;
using BookstoreAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace BookstoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;

        private readonly IConfiguration _configuration;

        public BooksController(IConfiguration configuration, ILogger<BooksController> logger)
        {
            _configuration = configuration;

            _logger = logger;
        }

        // Here we GET the current list of books from the database
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                // Fetch books from the database
                List<Book> books = GetBooksFromDatabase();
                return Ok(books);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while fetching books from the database.");
            }
        }

        // This method is used to query the database, to check for books
        private List<Book> GetBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Books";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Book book = new Book
                                {
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Author = reader.GetString(reader.GetOrdinal("Author")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                };

                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching books from the database");
                // throw; 
            }

            return books;
        }


        // This the cart
        // Here we POST or add books to the chart when a user selects the book from the list 
        [HttpPost("addToCart")]
        public IActionResult AddToCart(int bookId, string userId)
        {
            try
            {
                // Check if the book is already in the cart
                bool isBookInCart = IsBookInCart(bookId);

                if (!isBookInCart)
                {
                    // If not, add it to the cart in the database
                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand("INSERT INTO CartItems (BookId, Quantity, UserId) VALUES (@BookId, 1, @UserId)", connection))
                        {
                            command.Parameters.Add("@BookId", SqlDbType.Int).Value = bookId;
                            command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userId;

                            command.ExecuteNonQuery();
                        }
                    }
                }

                // Redirect back to the books list after adding to the cart
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log any errors to the console
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }


        // This method checks if the book you selected is already in the cart or not
        private bool IsBookInCart(int bookId)
        {
            // IMPORTANT -> Modify the connection string in 'appsettings.json' and replace it with the string from your database
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM CartItems WHERE BookId = @BookId", connection))
                {
                    command.Parameters.Add("@BookId", SqlDbType.Int).Value = bookId;

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }


    }

}
