using System;
using System.Text;
using System.Linq;

namespace Market.DataModels.DTos
{
    public class CustomerDataUpdateDto
    {
        public string? StreetAddress { get; set; }
        public string? Neighborhood { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
    }
}