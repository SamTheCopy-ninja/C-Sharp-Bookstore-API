using BookstoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditBooksController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EditBooksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Here we perform a GET request to fetch a list of all the books currently available
        [HttpGet("AllBooks")]
        public IActionResult GetAllBooks()
        {
            List<Book> allBooks = GetAllBooksFromDatabase();
            return Ok(allBooks);
        }

        // Here we perform an GET request to update the 'quantity' of the book selected by the admin
        // This fetches the book from the database
        [HttpGet("Edit/{id}")]
        public IActionResult EditBook(int id)
        {
            Book book = GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        // Here we perform an POST request to update the 'quantity' of the book selected by the admin
        // This adds the updated book information back into the database
        [HttpPost("Edit")]
        public IActionResult UpdateBookQuantity(int bookId, int newQuantity)
        {
            UpdateBookInDatabase(bookId, newQuantity);
            return NoContent();
        }

        // This methods helps perform the POST request above
        private void UpdateBookInDatabase(int bookId, int newQuantity)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Books SET Quantity = @NewQuantity WHERE BookId = @BookId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = bookId;
                        command.Parameters.Add("@NewQuantity", SqlDbType.Int).Value = newQuantity;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // This method is used to fetch all the books currently in the database
        private List<Book> GetAllBooksFromDatabase()
        {
            List<Book> allBooks = new List<Book>();
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
                                    BookId = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Author = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4)
                                };
                                allBooks.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return allBooks;
        }

        // This methods helps fetch a specific book from the database, based on ID
        // This allows Admins to update details for that boook
        private Book GetBookById(int bookId)
        {
            Book book = null;
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Books WHERE BookId = @BookId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@BookId", SqlDbType.Int).Value = bookId;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                book = new Book
                                {
                                    BookId = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Author = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return book;
        }

    }
}
