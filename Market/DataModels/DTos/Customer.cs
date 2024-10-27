using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.DTos
{
    public class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; }

        // Hashed Password
        public string Password { get; set; } 
        public string FullName { get; set; }
    }
}
