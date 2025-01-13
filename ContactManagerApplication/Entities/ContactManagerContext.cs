using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Entities
{
    public class ContactManagerContext : DbContext
    {
        public ContactManagerContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; } = default!;
    }
}
