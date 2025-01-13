using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Repositories
{
    public class PersonRepository
    {
        private readonly ContactManagerContext _context;

        public PersonRepository(ContactManagerContext context)
        {
            this._context = context;
        }

        public async Task<IList<Person>> GetPersonsAsync()
        {
            var persons = await this._context.Persons.
                AsNoTracking().
                OrderBy(p => p.Id).
                ToListAsync();

            return persons;
        }

        public async Task<int> AddPersonAsync(Person person)
        {
            try
            {
                _ = await this._context.Persons.AddAsync(person);
                _ = await this._context.SaveChangesAsync();

                return person.Id;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"failed to add person {person.Name}", ex);
            }
        }
    }
}
