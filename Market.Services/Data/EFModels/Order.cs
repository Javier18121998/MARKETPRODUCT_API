namespace MARKETPRODUCT_API.Data.EFModels
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; 
        public List<OrderItem> Items { get; set; } = new List<OrderItem>(); 
    }
}
