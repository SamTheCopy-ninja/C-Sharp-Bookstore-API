namespace BookstoreAPI.Models
{
    public class Carts
    {
        public int CartItemId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }

        public string UserId { get; set; }

        // This is Book property to store book details so they are displayed in the cart properly
        public Book Book { get; set; }
    }
}
