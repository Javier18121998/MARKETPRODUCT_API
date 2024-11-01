using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.EFModels
{
    public class RemoveCartItemRequest
    {
        public string ProductName { get; set; }
        public string Size { get; set; }
    }
}
