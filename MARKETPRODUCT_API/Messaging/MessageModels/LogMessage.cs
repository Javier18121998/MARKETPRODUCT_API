namespace MARKETPRODUCT_API.Messaging.MessageModels
{
    public class LogMessage
    {
        public string Action { get; set; }  
        public string ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
