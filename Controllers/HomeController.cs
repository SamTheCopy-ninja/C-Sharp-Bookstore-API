using BookstoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Here we perform a GET request to fetch a list of all the books currently available
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                List<Book> books = GetBooksFromDatabase();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching books from the database.");
            }
        }

        // Here we perform a GET request to fetch all the books that match the search query
        [HttpGet("search")]
        public IActionResult SearchBooks([FromQuery] string searchString)
        {
            try
            {
                List<Book> books = SearchBooksInDatabase(searchString);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching books in the database.");
            }
        }

        // This method is used to fetch all the books currently in the database
        private List<Book> GetBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
                                    BookId = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Author = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4)
                                };

                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return books;
        }

        // This method is used to fetch all books that match the search term
        private List<Book> SearchBooksInDatabase(string searchString)
        {
            List<Book> books = new List<Book>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    string query = "SELECT * FROM Books WHERE Title LIKE @SearchString OR Author LIKE @SearchString";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@SearchString", SqlDbType.NVarChar).Value = "%" + searchString + "%";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Book book = new Book
                                {
                                    BookId = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Author = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4)
                                };

                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return books;
        }
    }
}
