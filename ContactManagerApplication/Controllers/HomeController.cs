using ContactManagerApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using Newtonsoft.Json;
using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApplication.Controllers
{
    public class PersonUpdateModel
    {
        public int Id { get; set; }
        public string Field { get; set; } = default!;
        public string Value { get; set; } = default!;
    }

    public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ContactManagerContext _context;

		public HomeController(ILogger<HomeController> logger, ContactManagerContext context)
		{
			_logger = logger;
            _context = context;
		}

		public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.Select(person => person.ToPersonModel()).ToListAsync());
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePerson([FromBody] PersonUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid model state." });
            }

            var person = await _context.Persons.FindAsync(model.Id);

            if (person == null)
            {
                return Json(new { success = false, message = "Person not found." });
            }

            string[] dateFormats = { "dd.MM.yyyy" };
            switch (model.Field)
            {
                case nameof(person.Name):
                    person.Name = model.Value;
                    break;

                case nameof(person.DateOfBirth):
                    if (DateTime.TryParseExact(model.Value, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        person.DateOfBirth = dateOfBirth;
                        break;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid value for 'Date of Birth'." });
                    }

                case nameof(person.Married):
                    if (bool.TryParse(model.Value, out bool married))
                    {
                        person.Married = married;
                        break;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid value for 'Married'." });
                    }

                case nameof(person.Phone):
                    person.Phone = model.Value;
                    break;

                case nameof(person.Salary):
                    if (decimal.TryParse(model.Value, out var salary))
                    {
                        person.Salary = salary;
                        break;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid value for 'Salary'." });
                    }

                default:
                    return Json(new { success = false, message = "Invalid field." });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Person data updated successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersons(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No CSV file selected or file is empty.";
                return RedirectToAction(nameof(Index));
            }

            var persons = new List<Models.Person>();

            using (var stream = new StreamReader(file.OpenReadStream()))
			{
				using (var csvReader = new CsvReader(stream, CultureInfo.InvariantCulture))
                {
                    await foreach (var person in csvReader.GetRecordsAsync<Models.Person>())
					{
						if (TryValidateModel(person))
                        {
                            persons.Add(person);
						}
						else
                        {
                            TempData["Error"] = "Invalid model state.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }

            await _context.Persons.AddRangeAsync(persons.Select(EntityFramework.Entities.Person.FromPersonModel));
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return Json(new { success = false, message = "Person not found." });
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Person deleted successfully." });
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
