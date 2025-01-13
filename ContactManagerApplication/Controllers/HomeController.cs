using ContactManagerApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using Newtonsoft.Json;
using EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using PersonModel = ContactManagerApplication.Models.Person;
using Newtonsoft.Json.Linq;

namespace ContactManagerApplication.Controllers
{
    public class PersonUpdateModel
    {
        public int Id { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }

    public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ContactManagerContext _context;
        private IEnumerable<Models.Person> _persons = new List<Models.Person>();

		public HomeController(ILogger<HomeController> logger, ContactManagerContext context)
		{
			_logger = logger;
            _context = context;
		}

		public async Task<IActionResult> Index()
        {
            //var personsJson = TempData["Persons"] as string;
            //if (personsJson != null)
            //{
            //    _persons = JsonConvert.DeserializeObject<List<Models.Person>>(personsJson) ?? new List<Models.Person>();
            //}

            //return View(_persons);

            return View(await _context.Persons.Select(person => person.ToPersonModel()).ToListAsync());
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePerson([FromBody] PersonUpdateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Field) || string.IsNullOrWhiteSpace(model.Value))
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var person = await _context.Persons.FindAsync(model.Id);
            if (person == null)
            {
                return Json(new { success = false, message = "Person not found." });
            }

            // Update the field dynamically
            switch (model.Field)
            {
                case "Name":
                    person.Name = model.Value;
                    break;
                // Add cases for other fields if needed
                default:
                    return Json(new { success = false, message = "Invalid field." });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please upload a valid CSV file.";
                return RedirectToAction("Index");
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
                            TempData["Error"] = "One or more records in the file are invalid.";
                            return RedirectToAction("Index");
                        }
                    }
                }
            }

            await _context.Persons.AddRangeAsync(persons.Select(EntityFramework.Entities.Person.FromPersonModel));
            await _context.SaveChangesAsync();

            TempData["Persons"] = JsonConvert.SerializeObject(persons);
            TempData["Success"] = "File uploaded and data processed successfully.";
            return RedirectToAction("Index");
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
