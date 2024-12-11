using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        // GET: TeamMembers
        public async Task<IActionResult> Index()
        {
            var teamMembers = await _context.TeamMembers.ToListAsync();
            return View(teamMembers); // Pass the list of team members to the view
        }



        // GET: TeamMembers/Upsert/5 (5 is optional for creating or editing)
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null)
            {
                // Create mode
                return View(new TeamMember());
            }

            // Edit mode
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            return View(teamMember);
        }

        // POST: TeamMembers/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TeamMember teamMember, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Handle image upload
                if (Photo != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                    string uploadPath = Path.Combine(wwwRootPath, "images/teamimages");

                    // Ensure the directory exists
                    Directory.CreateDirectory(uploadPath);

                    // Delete old image if editing
                    if (!string.IsNullOrEmpty(teamMember.PhotoUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, teamMember.PhotoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                    teamMember.PhotoUrl = $"/images/teamimages/{fileName}";
                }

                if (teamMember.Id == 0)
                {
                    // Create new team member
                    _context.Add(teamMember);
                }
                else
                {
                    // Update existing team member
                    _context.Update(teamMember);
                }

                await _context.SaveChangesAsync();
                TempData["success"] = "Team member saved successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(teamMember);
        }

        // GET: TeamMembers/Delete/5
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

            // Confirm delete view
            return View(teamMember);
        }

        // POST: TeamMembers/Delete/5
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
            }

            TempData["success"] = "Team member deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }

}
