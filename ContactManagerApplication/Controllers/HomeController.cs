using ContactManagerApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using Newtonsoft.Json;
using EntityFramework.Entities;

namespace ContactManagerApplication.Controllers
{
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

		public IActionResult Index()
        {
            var personsJson = TempData["Persons"] as string;
            if (personsJson != null)
            {
                _persons = JsonConvert.DeserializeObject<List<Models.Person>>(personsJson) ?? new List<Models.Person>();
            }

            return View(_persons);
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
