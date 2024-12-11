using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RN_International_Website.Models;

namespace RN_International_Website.Controllers
{
    [Authorize]
    public class TeamMembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamMembersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var teamMembers = await _context.TeamMembers.ToListAsync();
            return View(teamMembers); // Pass the list of team members to the view
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null)
            {
                return View(new TeamMember()); // Create mode
            }

            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound(); // If not found, return 404
            }

            return View(teamMember); // Edit mode
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TeamMember teamMember, IFormFile? Photo)
        {
            if (!ModelState.IsValid)
            {
                return View(teamMember);
            }

            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Handle image upload
                if (Photo != null)
                {
                    Console.WriteLine($"Uploading file: {Photo.FileName}");

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(teamMember.PhotoUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, teamMember.PhotoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Generate unique file name
                    string fileName = Guid.NewGuid() + Path.GetExtension(Photo.FileName);
                    string uploadPath = Path.Combine(wwwRootPath, "images/teamimages");
                    Directory.CreateDirectory(uploadPath);

                    // Save the new image
                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }

                    teamMember.PhotoUrl = $"/images/teamimages/{fileName}";
                }

                // Add or update the team member
                if (teamMember.Id == 0)
                {
                    _context.TeamMembers.Add(teamMember);
                }
                else
                {
                    _context.TeamMembers.Update(teamMember);
                }

                await _context.SaveChangesAsync();
                TempData["success"] = "Team member saved successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the team member. Please try again.");
                return View(teamMember);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember); // Confirm delete view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember != null)
            {
                // Delete associated image
                if (!string.IsNullOrEmpty(teamMember.PhotoUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, teamMember.PhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.TeamMembers.Remove(teamMember);
                await _context.SaveChangesAsync();
                TempData["success"] = "Team member deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
