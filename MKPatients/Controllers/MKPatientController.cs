using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKPatients.Models;

namespace MKPatients.Controllers
{
    public class MKPatientController : Controller
    {
        private readonly PatientsContext _context;

        public MKPatientController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKPatient
        public async Task<IActionResult> Index()
        {
            var patientsContext = _context.Patients
                 .OrderBy(a => a.LastName);
            return View(await patientsContext.ToListAsync());
        }

        // GET: MKPatient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: MKPatient/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: MKPatient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            try
            {
                //if (string.IsNullOrEmpty(patient.LastName))
                //{
                //    ModelState.AddModelError("", "Last name is required field");
                //    ModelState.AddModelError("Last Name", "Filed Specific - Last name is required field");
                //}
                if (ModelState.IsValid)
                {
                    _context.Add(patient);
                    await _context.SaveChangesAsync();

                    TempData["message"] = "Patient Record created successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                TempData["Message"] = ex.GetBaseException().Message;
            }

            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: MKPatient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            ViewData["ProvinceName"] = new SelectList(_context.Provinces.OrderBy(a =>a.Name), "ProvinceCode", "Name");
            return View(patient);
        }

        // POST: MKPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Patient Record edited successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.GetBaseException().Message);
                    TempData["Message"] = ex.GetBaseException().Message;
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Provinces, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: MKPatient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: MKPatient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patients == null)
            {
                return Problem("Entity set 'PatientsContext.Patients'  is null.");
            }
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                try
                {
                    _context.Patients.Remove(patient);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.GetBaseException().Message);
                    TempData["Message"] = ex.GetBaseException().Message;
                }
            }
            
           
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
          return (_context.Patients?.Any(e => e.PatientId == id)).GetValueOrDefault();
        }

        public JsonResult BirthDateNotFuture(DateTime DateOfBirth )
        {
            if (DateOfBirth <= DateTime.Now)
                return Json(true);
            else
                return Json("Date cannot be in the future");
        }
        public JsonResult DeathDateNotFuture(DateTime DateOfDeath)
        {
            if (DateOfDeath <= DateTime.Now)
                return Json(true);
            else
                return Json("Date cannot be in the future");
        }

        public JsonResult GenderValid(string Gender)
        {
            if (Gender == "M" || Gender == "F" || Gender == "X" || Gender == "m" || Gender == "f" || Gender == "x")
                return Json(true);
            else
                return Json("Gender must be M, F, or X");
        }

        public JsonResult ValidateOHIPPattern(string Ohip)
        {
        
            var regex = new Regex("^[0-9]{4}-[0-9]{3}-[0-9]{3}-[A-Z]{2}$");
            if (regex.IsMatch(Ohip))
                return Json(true);
            else
                return Json("OHIP should be in format 1234-123-123-XX");
        }

        public JsonResult ProvinceCodeValid(string provinceCode)
        {
            
                var province = _context.Provinces
                    .SingleOrDefault(p => p.ProvinceCode == provinceCode.ToUpper());

                if (province != null)
                    return Json(true);
                else
                    return Json("Province code not found.");
            
        }

    }

}
