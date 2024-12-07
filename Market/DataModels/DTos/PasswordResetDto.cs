using System;
using System.Text;
using System.Linq;

namespace Market.DataModels.DTos
{
    public class PasswordResetDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ResetToken { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}