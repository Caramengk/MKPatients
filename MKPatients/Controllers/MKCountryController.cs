using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKPatients.Models;
using Microsoft.AspNetCore.Authorization;


namespace MKPatients.Controllers
{
    [Authorize]
    public class MKCountryController : Controller
    {
        private readonly PatientsContext _context;

        public MKCountryController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKCountry
        //retrieving the list of country.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
              return _context.Countries != null ? 
                          View(await _context.Countries.ToListAsync()) :
                          Problem("Entity set 'PatientsContext.Countries'  is null.");
        }

        // GET: MKCountry/Details/5
        //retrievig the details of country
        
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: MKCountry/Create
        //showing form to create a new country in list
      
        public IActionResult Create()
        {
            return View();
        }

        // POST: MKCountry/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //creating new country in list according to input
        public async Task<IActionResult> Create([Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: MKCountry/Edit/5
        //showing edit form of a country
       
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: MKCountry/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //saving updates
        public async Task<IActionResult> Edit(string id, [Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax")] Country country)
        {
            if (id != country.CountryCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryCode))
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
            return View(country);
        }

        // GET: MKCountry/Delete/5
        //shwoing delete confirmation page of a country
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: MKCountry/Delete/5
        //delete the country
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'PatientsContext.Countries'  is null.");
            }
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(string id)
        {
          return (_context.Countries?.Any(e => e.CountryCode == id)).GetValueOrDefault();
        }
    }
}
