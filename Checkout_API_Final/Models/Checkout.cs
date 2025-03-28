namespace Checkout_API_Final.Models
{
    public class Checkout
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsComplete { get; set; }
        public List <CheckoutItem> Items { get; set; }
    }
}
