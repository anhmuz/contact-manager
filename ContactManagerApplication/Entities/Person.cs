using System.ComponentModel.DataAnnotations;
using PersonModel = ContactManagerApplication.Models.Person;

namespace EntityFramework.Entities
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Married { get; set; }
        public string? Phone { get; set; }
        public decimal? Salary { get; set; }

        public PersonModel ToPersonModel()
        {
            return new PersonModel
            {
                Id = this.Id,
                Name = this.Name ?? string.Empty,
                DateOfBirth = this.DateOfBirth ?? DateTime.MinValue,
                Married = this.Married ?? false,
                Phone = this.Phone ?? string.Empty,
                Salary = this.Salary ?? 0.0m,
            };
        }

        public static Person FromPersonModel(PersonModel person)
        {
            return new Person
            {
                Id = person.Id,
                Name = person.Name,
                DateOfBirth = person.DateOfBirth,
                Married = person.Married,
                Phone = person.Phone,
                Salary = person.Salary,
            };
        }
    }
}
