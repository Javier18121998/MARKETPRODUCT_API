namespace MARKETPRODUCT_API.Data.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
