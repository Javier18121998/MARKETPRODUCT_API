using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.DTos
{
    public class CustomerSession
    {
        public int Id { get; set; }                   
        public int CustomerId { get; set; }           
        public string Token { get; set; }             
        public DateTime CreatedAt { get; set; }       
        public DateTime ExpiresAt { get; set; }       
        public bool IsRevoked { get; set; }           
        public Customer Customer { get; set; }
    }
}
