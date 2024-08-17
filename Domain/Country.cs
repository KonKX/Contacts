using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Person>? Persons { get; set; }
    }
}