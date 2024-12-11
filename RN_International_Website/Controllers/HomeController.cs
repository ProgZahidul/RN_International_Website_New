using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RN_International_Website.Models;
using RN_International_Website.ViewModels;
using System.Diagnostics;

namespace RN_International_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // Single constructor to initialize dependencies
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Index action to fetch both team members and products
        public async Task<IActionResult> Index()
        {
            var teamMembers = await _context.TeamMembers.ToListAsync();
            var products = await _context.Products.ToListAsync();

            // Use a ViewModel to pass both datasets
            var viewModel = new HomeIndexViewModel
            {
                TeamMembers = teamMembers,
                Products = products
            };

            return View(viewModel);
        }

        
    

        // Privacy action
        public IActionResult Privacy()
        {
            return View();
        }

        // Error action
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }

}
