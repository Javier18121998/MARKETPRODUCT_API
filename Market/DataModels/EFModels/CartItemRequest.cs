using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.EFModels
{
    public class CartItemRequest
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
    }
}
