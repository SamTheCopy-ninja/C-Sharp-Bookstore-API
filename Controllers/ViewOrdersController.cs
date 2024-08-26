using BookstoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewOrdersController : Controller
    {
        private readonly IConfiguration _configuration;

        public ViewOrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Here we perform a GET request to fetch items added to a cart
        // To allow an admin to view ALL current customer orders
        [HttpGet]
        public IActionResult GetAllCartItems()
        {
            List<Carts> allCartItems = GetAllCartItemsFromDatabase();
            return Ok(allCartItems);
        }

        // Here we perform a GET request to fetch items added to a cart by a specific user, based on ID
        [HttpGet("ByUser/{userId}")]
        public IActionResult GetCartItemsByUser(string userId)
        {
            List<Carts> userCartItems = GetCartItemsByUserFromDatabase(userId);
            return Ok(userCartItems);
        }

        // This methods gets the list of books based on the current user's ID
        private List<Carts> GetCartItemsByUserFromDatabase(string userId)
        {
            List<Carts> userCartItems = new List<Carts>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT CartItems.CartItemId, CartItems.BookId, CartItems.Quantity, CartItems.UserId, " +
                                   "Books.Title, Books.Author, Books.Price " +
                                   "FROM CartItems " +
                                   "INNER JOIN Books ON CartItems.BookId = Books.BookId " +
                                   "WHERE CartItems.UserId = @UserId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Carts cartItem = new Carts
                                {
                                    CartItemId = reader.GetInt32(0),
                                    BookId = reader.GetInt32(1),
                                    Quantity = reader.GetInt32(2),
                                    UserId = reader.GetString(3),
                                    Book = new Book
                                    {
                                        Title = reader.GetString(4),
                                        Author = reader.GetString(5),
                                        Price = reader.GetDecimal(6)
                                    }
                                };

                                userCartItems.Add(cartItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return userCartItems;
        }

        // This methods gets the list of all books in a cart, for admins to view
        private List<Carts> GetAllCartItemsFromDatabase()
        {
            List<Carts> allCartItems = new List<Carts>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT CartItems.CartItemId, CartItems.BookId, CartItems.Quantity, CartItems.UserId, " +
                                   "Books.Title, Books.Author, Books.Price " +
                                   "FROM CartItems " +
                                   "INNER JOIN Books ON CartItems.BookId = Books.BookId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Carts cartItem = new Carts
                                {
                                    CartItemId = reader.GetInt32(0),
                                    BookId = reader.GetInt32(1),
                                    Quantity = reader.GetInt32(2),
                                    UserId = reader.GetString(3),
                                    Book = new Book
                                    {
                                        Title = reader.GetString(4),
                                        Author = reader.GetString(5),
                                        Price = reader.GetDecimal(6)
                                    }
                                };

                                allCartItems.Add(cartItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return allCartItems;
        }

     
    }

}
