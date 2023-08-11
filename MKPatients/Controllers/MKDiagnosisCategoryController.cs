using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKPatients.Models;

namespace MKPatients.Controllers
{
    public class MKDiagnosisCategoryController : Controller
    {
        private readonly PatientsContext _context;

        public MKDiagnosisCategoryController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKDiagnosisCategory
        //retrieving the diagnosis category list.
        public async Task<IActionResult> Index()
        {
              return _context.DiagnosisCategories != null ? 
                          View(await _context.DiagnosisCategories.ToListAsync()) :
                          Problem("Entity set 'PatientsContext.DiagnosisCategories'  is null.");
        }

        // GET: MKDiagnosisCategory/Details/5
        //retrieving the diagnosis category detail

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DiagnosisCategories == null)
            {
                return NotFound();
            }

            var diagnosisCategory = await _context.DiagnosisCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }

            return View(diagnosisCategory);
        }

        // GET: MKDiagnosisCategory/Create
        //showing page to create new  diagnosis category 
        public IActionResult Create()
        {
            return View();
        }

        // POST: MKDiagnosisCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //create new  diagnosis category 
        public async Task<IActionResult> Create([Bind("Id,Name")] DiagnosisCategory diagnosisCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diagnosisCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diagnosisCategory);
        }

        // GET: MKDiagnosisCategory/Edit/5
        //showing page to edit diagnosis category 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DiagnosisCategories == null)
            {
                return NotFound();
            }

            var diagnosisCategory = await _context.DiagnosisCategories.FindAsync(id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }
            return View(diagnosisCategory);
        }

        // POST: MKDiagnosisCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //saving updates
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DiagnosisCategory diagnosisCategory)
        {
            if (id != diagnosisCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosisCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosisCategoryExists(diagnosisCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(diagnosisCategory);
        }

        // GET: MKDiagnosisCategory/Delete/5
        //showing page to delete diagnosis category 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DiagnosisCategories == null)
            {
                return NotFound();
            }

            var diagnosisCategory = await _context.DiagnosisCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }

            return View(diagnosisCategory);
        }

        // POST: MKDiagnosisCategory/Delete/5
        //delete diagnosis category 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DiagnosisCategories == null)
            {
                return Problem("Entity set 'PatientsContext.DiagnosisCategories'  is null.");
            }
            var diagnosisCategory = await _context.DiagnosisCategories.FindAsync(id);
            if (diagnosisCategory != null)
            {
                _context.DiagnosisCategories.Remove(diagnosisCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosisCategoryExists(int id)
        {
          return (_context.DiagnosisCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
