using BookstoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint to add or POST a new book to the database
        [HttpPost("AddBook")]

        public IActionResult AddBook(BookInputModel bookInput)
        {
            if (ModelState.IsValid)
            {
                // Here we add the book to the database
                try
                {
                    // Call method below, to add the book to the database
                    AddBookToDatabase(bookInput);

                    // Return a success message
                    return Ok("Book added successfully");
                }
                catch (Exception ex)
                {
                    // Log errors
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, "An error occurred while adding the book");
                }
            }

            // If model state is not valid, return a bad request status code
            return BadRequest(ModelState);
        }

        // Method to add a new book to the database
        private void AddBookToDatabase(BookInputModel bookInput)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                // SQL INSERT statement
                string insertQuery = "INSERT INTO Books (Title, Author, Price, Quantity) VALUES (@Title, @Author, @Price, @Quantity)";

                // Create a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Execute the INSERT command
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", bookInput.Title);
                        command.Parameters.AddWithValue("@Author", bookInput.Author);
                        command.Parameters.AddWithValue("@Price", bookInput.Price);
                        command.Parameters.AddWithValue("@Quantity", bookInput.Quantity);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errors to console
                Console.WriteLine($"Error adding book to database: {ex.Message}");
                throw;
            }
        }
    }

}
