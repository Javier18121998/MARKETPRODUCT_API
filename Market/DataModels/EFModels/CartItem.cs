using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Market.DataModels.EFModels
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        [JsonIgnore]
        public decimal Price { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
