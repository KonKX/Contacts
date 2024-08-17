using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        public string? Email { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }
    }
}
