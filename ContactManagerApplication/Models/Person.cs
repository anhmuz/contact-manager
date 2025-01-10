using CsvHelper.Configuration.Attributes;

namespace ContactManagerApplication.Models
{
    public class Person
    {
        [Ignore]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public bool Married { get; set; }
        public string Phone { get; set; } = default!;
        public decimal Salary { get; set; }
    }
}
