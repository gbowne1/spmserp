using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace spmserp.Controllers
{
    [Authorize(Roles = "HR Admin")]
    public class LeaveRequestController : Controller
    {
        private AppDbContext _context { get; }

        public LeaveRequestController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.ActivePage = "leaves";
            List<Leave> leaves = _context.Leaves.ToList();
            return View(leaves);
        }

        public async Task<IActionResult> IsAccepted(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var leaveDb = await _context.Leaves.FindAsync(id);
            if (leaveDb == null)
            {
                return NotFound();
            }
            leaveDb.isAccepted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IsDenied(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var leaveDb = await _context.Leaves.FindAsync(id);
            if (leaveDb == null)
            {
                return NotFound();
            }
            leaveDb.isAccepted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}