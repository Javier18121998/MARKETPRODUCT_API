using System;
using System.Text;
using System.Linq;

namespace Market.DataModels.EFModels
{
    public class CustomerDataUpdate
    {
        public string? StreetAddress { get; set; }
        public string? Neighborhood { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UniquePopulationRegistryCode { get; set; }
        public string? TaxId { get; set; }
    }
}