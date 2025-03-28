namespace Checkout_API_Final.Models
{
    public class CheckoutItem
    {
        public int Id { get; set; }
        public int CheckoutId { get; set; }
        public Checkout Checkout { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public  int Quantity  { get; set; }
        public decimal PriceAtTime { get; set; }
    }
}
