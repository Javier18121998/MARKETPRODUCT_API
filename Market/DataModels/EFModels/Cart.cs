using Market.DataModels.DTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.EFModels
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Customer? Customer { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
