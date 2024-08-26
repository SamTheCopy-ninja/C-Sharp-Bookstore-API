using Microsoft.AspNetCore.Mvc;
using BookstoreAPI.Models;
using System.Data.SqlClient;
using System.Data;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;

        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Here we perform a GET request to fetch the cart items for a specific user, based on their ID
        [HttpGet]
        public IActionResult GetCartItems([FromQuery] string userId)
        {
            try
            {
                List<Carts> cartItems = GetCartItemsFromDatabase(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while fetching cart items from the database.");
            }
        }

        // This methods helps perform the GET request above
        private List<Carts> GetCartItemsFromDatabase(string userId)
        {
            List<Carts> cartItems = new List<Carts>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    
                    string query = "SELECT * FROM CartItems WHERE UserId = @UserId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userId;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Carts cartItem = new Carts
                                {
                                    CartItemId = reader.GetInt32(0),
                                    BookId = reader.GetInt32(1),
                                    Quantity = reader.GetInt32(2)
                                };

                                // Fetch book details for each cart item
                                Book book = GetBookDetailsById(cartItem.BookId);
                                cartItem.Book = book;

                                cartItems.Add(cartItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errors
                Console.WriteLine(ex.Message);
            }

            return cartItems;
        }


        // This method checks your cart, and then pulls up the information for each book in your cart to display them
        private Book GetBookDetailsById(int bookId)
        {
            Book book = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM Books WHERE BookId = @BookId", connection))
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
                // Log any errors to the console
                Console.WriteLine(ex.Message);
            }

            return book;
        }


        // Here we can delete books from a cart
        // This DELETE request removes the book a user choses to delete from their cart
        [HttpDelete("RemoveFromCart/{cartItemId}")]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            try
            {
                DeleteCartItemFromDatabase(cartItemId);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                // Log errors
                return StatusCode(500, "An error occurred while removing the item from the cart.");
            }
        }

        // This method performs the Delete request
        private void DeleteCartItemFromDatabase(int cartItemId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    string query = "DELETE FROM CartItems WHERE CartItemId = @CartItemId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@CartItemId", SqlDbType.Int).Value = cartItemId;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errors
                Console.WriteLine(ex.Message);
                throw; 
            }
        }


    }
}
