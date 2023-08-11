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
    public class MKPatientDiagnosisController : Controller
    {
        private readonly PatientsContext _context;

        public MKPatientDiagnosisController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MKPatientDiagnosis
        public async Task<IActionResult> Index(string PatientId)
        {


            var patientsContext = _context.PatientDiagnoses.Include(p => p.Diagnosis).Include(p => p.Patient)

                .OrderBy(a => a.Patient.LastName)
                .ThenBy(a => a.Patient.FirstName);


            return View(await patientsContext.ToListAsync());
        }

        // GET: MKPatientDiagnosis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PatientDiagnoses == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnoses
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // GET: MKPatientDiagnosis/Create
        public IActionResult Create()
        {
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "DiagnosisId", "DiagnosisId");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            return View();
        }

        // POST: MKPatientDiagnosis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientDiagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "DiagnosisId", "DiagnosisId", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: MKPatientDiagnosis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PatientDiagnoses == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnoses.FindAsync(id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "DiagnosisId", "DiagnosisId", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // POST: MKPatientDiagnosis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (id != patientDiagnosis.PatientDiagnosisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientDiagnosis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientDiagnosisExists(patientDiagnosis.PatientDiagnosisId))
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
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "DiagnosisId", "DiagnosisId", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: MKPatientDiagnosis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PatientDiagnoses == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnoses
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // POST: MKPatientDiagnosis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PatientDiagnoses == null)
            {
                return Problem("Entity set 'PatientsContext.PatientDiagnoses'  is null.");
            }
            var patientDiagnosis = await _context.PatientDiagnoses.FindAsync(id);
            if (patientDiagnosis != null)
            {
                _context.PatientDiagnoses.Remove(patientDiagnosis);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientDiagnosisExists(int id)
        {
          return (_context.PatientDiagnoses?.Any(e => e.PatientDiagnosisId == id)).GetValueOrDefault();
        }
    }
}
