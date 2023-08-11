using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKPatients.Models;
using Microsoft.AspNetCore.Http;

namespace MKPatients.Controllers
{
    public class MKMedicationController : Controller
    {
        private readonly PatientsContext _context;

        public MKMedicationController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKMedication
        public async Task<IActionResult> Index(string MedicationTypeId)
        {
            if (!string.IsNullOrEmpty(MedicationTypeId))
            {
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId);
                //storing in sessions
                HttpContext.Session.SetString("MedicationTypeId", MedicationTypeId);
            }
            else if(Request.Query["MedicationTypeId"].Any())
            {
                MedicationTypeId = Request.Query["MedicationTypeId"].ToString();
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId);
                HttpContext.Session.SetString("MedicationTypeId", MedicationTypeId);
            }
            else if(Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if(HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTypeId = HttpContext.Session.GetString(MedicationTypeId);
            }
            else
            {
                TempData["message"] = "Please select a medication type";
                return RedirectToAction("Index", "MedicationType");
            }


            ViewBag.MedicationTypeId = MedicationTypeId;
            var mTypeId = _context.MedicationTypes.Where(a =>Convert.ToString(a.MedicationTypeId) == MedicationTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;


            var patientsContext = _context.Medications.Include(m => m.ConcentrationCodeNavigation).Include(m => m.DispensingCodeNavigation).Include(m => m.MedicationType)
                .Where(a => Convert.ToString(a.MedicationTypeId) == MedicationTypeId)
                .OrderBy(a => a.Name)
                .ThenBy(a =>a.Concentration);
            
            return View(await patientsContext.ToListAsync());
        }

        // GET: MKMedication/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Medications == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }

            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;

            return View(medication);
        }

        // GET: MKMedication/Create
        public IActionResult Create()
        {

            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;
            ViewBag.MedicationTypeId = Convert.ToInt64(MedTypeId);

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits.OrderBy(a =>a.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits.OrderBy(a =>a.DispensingCode), "DispensingCode", "DispensingCode");
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId");
            return View();
        }

        // POST: MKMedication/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {

            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;
            ViewBag.MedicationTypeId = Convert.ToInt64(MedTypeId);



            var isDuplicate = _context.Medications.Where(a => a.Name == medication.Name && a.Concentration == medication.Concentration && a.ConcentrationCode == medication.ConcentrationCode);
            if (isDuplicate.Any())
            {
                ModelState.AddModelError("", "Duplicat record");
            }

            if (ModelState.IsValid)
            {
                _context.Add(medication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits.OrderBy(a => a.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits.OrderBy(a => a.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: MKMedication/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Medications == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }


            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits.OrderBy(a => a.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits.OrderBy(a => a.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId", medication.MedicationTypeId);
            return View(medication);

            
        }

        // POST: MKMedication/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
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

            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnits.OrderBy(a => a.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnits.OrderBy(a => a.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationTypes, "MedicationTypeId", "MedicationTypeId", medication.MedicationTypeId);
            return View(medication);
        }

        // GET: MKMedication/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;

            if (id == null || _context.Medications == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // POST: MKMedication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            string MedTypeId = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }

            var mTypeId = _context.MedicationTypes.Where(a => Convert.ToString(a.MedicationTypeId) == MedTypeId).FirstOrDefault();
            ViewBag.MedicationName = mTypeId.Name;

            if (_context.Medications == null)
            {
                return Problem("Entity set 'PatientsContext.Medications'  is null.");
            }
            var medication = await _context.Medications.FindAsync(id);
            if (medication != null)
            {
                _context.Medications.Remove(medication);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
          return (_context.Medications?.Any(e => e.Din == id)).GetValueOrDefault();
        }
    }
}
