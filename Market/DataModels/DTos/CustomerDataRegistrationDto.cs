using System;
using System.Text;
using System.Linq;

namespace Market.DataModels.DTos
{
    public class CustomerDataRegistrationDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string StreetAddress { get; set; }
        public string Neighborhood { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string CURP { get; set; }
        public string TaxId { get; set; }
        public Customer Customer { get; set; }
    }
}