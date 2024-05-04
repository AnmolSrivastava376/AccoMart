using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class UserAddress
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public required string StreetLine { get; set; }  
        public required string City { get; set; }    
        public required string PostalCode { get; set; }  
        public required string Country { get; set; } 

    }
}
