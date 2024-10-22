namespace MARKETPRODUCT_API.Messaging.MessageModels
{
    public class OrderServiceMessage
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string Quantity {  get; set; }
    }
}
