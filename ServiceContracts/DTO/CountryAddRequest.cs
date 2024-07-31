using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class CountryAddRequest
    {
        [Required(ErrorMessage = "Country name is required")]
        public string? Name { get; set; }

        public Country ToCountry()
        {
            return new Country { Name = this.Name };
        }
    }
}
