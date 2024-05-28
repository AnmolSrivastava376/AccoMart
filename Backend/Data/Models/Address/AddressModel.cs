using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Address
{
    public class AddressModel
    {
        [Required]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        public string PhoneNumber { get; set; }


    }
}
