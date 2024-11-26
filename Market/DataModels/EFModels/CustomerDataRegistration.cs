using System;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Market.DataModels.EFModels
{
    public class CustomerDataRegistration
    {
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string Neighborhood { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string UniquePopulationRegistryCode { get; set; }
        public string? TaxId { get; set; }
    }
}