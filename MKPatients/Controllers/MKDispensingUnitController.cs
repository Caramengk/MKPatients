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
    public class MKDispensingUnitController : Controller
    {
        private readonly PatientsContext _context;

        public MKDispensingUnitController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKDispensingUnit
        //retrieving the dispensing unit list.
        public async Task<IActionResult> Index()
        {
              return _context.DispensingUnits != null ? 
                          View(await _context.DispensingUnits.ToListAsync()) :
                          Problem("Entity set 'PatientsContext.DispensingUnits'  is null.");
        }

        // GET: MKDispensingUnit/Details/5
        //retrieving the detials of dispensing unit.
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.DispensingUnits == null)
            {
                return NotFound();
            }

            var dispensingUnit = await _context.DispensingUnits
                .FirstOrDefaultAsync(m => m.DispensingCode == id);
            if (dispensingUnit == null)
            {
                return NotFound();
            }

            return View(dispensingUnit);
        }

        // GET: MKDispensingUnit/Create
        //showing page of creating new dispensing unit.
        public IActionResult Create()
        {
            return View();
        }

        // POST: MKDispensingUnit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //creating dispensing unit
        public async Task<IActionResult> Create([Bind("DispensingCode")] DispensingUnit dispensingUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dispensingUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dispensingUnit);
        }

        // GET: MKDispensingUnit/Edit/5
        //showing page of editing dispensing unit.
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.DispensingUnits == null)
            {
                return NotFound();
            }

            var dispensingUnit = await _context.DispensingUnits.FindAsync(id);
            if (dispensingUnit == null)
            {
                return NotFound();
            }
            return View(dispensingUnit);
        }

        // POST: MKDispensingUnit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //saving updates form input
        public async Task<IActionResult> Edit(string id, [Bind("DispensingCode")] DispensingUnit dispensingUnit)
        {
            if (id != dispensingUnit.DispensingCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dispensingUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DispensingUnitExists(dispensingUnit.DispensingCode))
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
            return View(dispensingUnit);
        }

        // GET: MKDispensingUnit/Delete/5
        //showing page of deleting dispensing unit.
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.DispensingUnits == null)
            {
                return NotFound();
            }

            var dispensingUnit = await _context.DispensingUnits
                .FirstOrDefaultAsync(m => m.DispensingCode == id);
            if (dispensingUnit == null)
            {
                return NotFound();
            }

            return View(dispensingUnit);
        }

        // POST: MKDispensingUnit/Delete/5
        //deleting dispensing unit.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.DispensingUnits == null)
            {
                return Problem("Entity set 'PatientsContext.DispensingUnits'  is null.");
            }
            var dispensingUnit = await _context.DispensingUnits.FindAsync(id);
            if (dispensingUnit != null)
            {
                _context.DispensingUnits.Remove(dispensingUnit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DispensingUnitExists(string id)
        {
          return (_context.DispensingUnits?.Any(e => e.DispensingCode == id)).GetValueOrDefault();
        }
    }
}
