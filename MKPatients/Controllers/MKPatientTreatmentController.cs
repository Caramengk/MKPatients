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
    public class MKPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public MKPatientTreatmentController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKPatientTreatment
        public async Task<IActionResult> Index(string PatientDiagnosisId)
        {

            if (!string.IsNullOrEmpty(PatientDiagnosisId))
            {
                HttpContext.Session.SetString("PatientDiagnosisId", PatientDiagnosisId);
            }
            else if (Request.Query["PatientDiagnosisId"].Any())
            {
                PatientDiagnosisId = Request.Query["PatientDiagnosisId"].ToString();
                HttpContext.Session.SetString("PatientDiagnosisId", PatientDiagnosisId);
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }
            else
            {
                TempData["message"] = "Please select Diagnosis";
                return RedirectToAction("Index");
            }


            var pDiagnosis = _context.PatientTreatments
                .Include(a => a.PatientDiagnosis)
                .Include(a =>a.PatientDiagnosis.Diagnosis)
                .Include(a => a.PatientDiagnosis.Patient)
                .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            ViewBag.PatientDiagnosisName = pDiagnosis.PatientDiagnosis.Diagnosis.Name;
            ViewBag.PatientLName = pDiagnosis.PatientDiagnosis.Patient.LastName;
            ViewBag.PatientFName = pDiagnosis.PatientDiagnosis.Patient.FirstName;








            var patientsContext = _context.PatientTreatments.Include(p => p.PatientDiagnosis).Include(p => p.Treatment)
                .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId))
                .OrderBy(a => a.DatePrescribed);
            return View(await patientsContext.ToListAsync());
        }

        // GET: MKPatientTreatment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string PatientDiagnosisId = string.Empty;
            if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }

            var paDiagnosis = _context.PatientTreatments
            .Include(a => a.PatientDiagnosis)
            .Include(a => a.PatientDiagnosis.Diagnosis)
            .Include(a => a.PatientDiagnosis.Patient)
            .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            ViewBag.PatientDiagnosisName = paDiagnosis.PatientDiagnosis.Diagnosis.Name;
            ViewBag.PatientLName = paDiagnosis.PatientDiagnosis.Patient.LastName;
            ViewBag.PatientFName = paDiagnosis.PatientDiagnosis.Patient.FirstName;

            if (id == null || _context.PatientTreatments == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatments
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // GET: MKPatientTreatment/Create
        public IActionResult Create()
        {

            string PatientDiagnosisId = string.Empty;
            if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }

            var paDiagnosis = _context.PatientTreatments
            .Include(a => a.PatientDiagnosis)
            .Include(a => a.PatientDiagnosis.Diagnosis)
            .Include(a => a.PatientDiagnosis.Patient)
            .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            ViewBag.PatientDiagnosisName = paDiagnosis.PatientDiagnosis.Diagnosis.Name;
            ViewBag.PatientLName = paDiagnosis.PatientDiagnosis.Patient.LastName;
            ViewBag.PatientFName = paDiagnosis.PatientDiagnosis.Patient.FirstName;
            ViewBag.PatientDiagnosisId = paDiagnosis.PatientDiagnosisId;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId");
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId");
            return View();
        }

        // POST: MKPatientTreatment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            string PatientDiagnosisId = string.Empty;
            if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }

            var paDiagnosis = _context.PatientTreatments
            .Include(a => a.PatientDiagnosis)
            .Include(a => a.PatientDiagnosis.Diagnosis)
            .Include(a => a.PatientDiagnosis.Patient)
            .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            ViewBag.PatientDiagnosisName = paDiagnosis.PatientDiagnosis.Diagnosis.Name;
            ViewBag.PatientLName = paDiagnosis.PatientDiagnosis.Patient.LastName;
            ViewBag.PatientFName = paDiagnosis.PatientDiagnosis.Patient.FirstName;
            ViewBag.PatientDiagnosisId = paDiagnosis.PatientDiagnosisId;

            if (ModelState.IsValid)
            {
                _context.Add(patientTreatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: MKPatientTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string PatientDiagnosisId = string.Empty;
            if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }

            var paDiagnosis = _context.PatientTreatments
            .Include(a => a.PatientDiagnosis)
            .Include(a => a.PatientDiagnosis.Diagnosis)
            .Include(a => a.PatientDiagnosis.Patient)
            .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            ViewBag.PatientDiagnosisName = paDiagnosis.PatientDiagnosis.Diagnosis.Name;
            ViewBag.PatientLName = paDiagnosis.PatientDiagnosis.Patient.LastName;
            ViewBag.PatientFName = paDiagnosis.PatientDiagnosis.Patient.FirstName;

            if (id == null || _context.PatientTreatments == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatments.FindAsync(id);
            if (patientTreatment == null)
            {
                return NotFound();
            }
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.PatientTreatments, "TreatmentId", "TreatmentId", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // POST: MKPatientTreatment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {

            string PatientDiagnosisId = string.Empty;
            if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
            }

            var paDiagnosis = _context.PatientTreatments
            .Include(a => a.PatientDiagnosis)
            .Include(a => a.PatientDiagnosis.Diagnosis)
            .Include(a => a.PatientDiagnosis.Patient)
            .Where(a => a.PatientDiagnosisId == Convert.ToInt32(PatientDiagnosisId)).FirstOrDefault();
            ViewBag.PatientDiagnosisName = paDiagnosis.PatientDiagnosis.Diagnosis.Name;
            ViewBag.PatientLName = paDiagnosis.PatientDiagnosis.Patient.LastName;
            ViewBag.PatientFName = paDiagnosis.PatientDiagnosis.Patient.FirstName;

            if (id != patientTreatment.PatientTreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientTreatmentExists(patientTreatment.PatientTreatmentId))
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
            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnoses, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "TreatmentId", "TreatmentId", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: MKPatientTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PatientTreatments == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatments
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            return View(patientTreatment);
        }

        // POST: MKPatientTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PatientTreatments == null)
            {
                return Problem("Entity set 'PatientsContext.PatientTreatments'  is null.");
            }
            var patientTreatment = await _context.PatientTreatments.FindAsync(id);
            if (patientTreatment != null)
            {
                _context.PatientTreatments.Remove(patientTreatment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientTreatmentExists(int id)
        {
          return (_context.PatientTreatments?.Any(e => e.PatientTreatmentId == id)).GetValueOrDefault();
        }
    }
}
