
using HQMS.Web.Interfaces;
using HQMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.Web.Controllers
{
    [Authorize]
    public class PatientController : BaseController
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetAllAsync();
            return View(patients);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken] // ✅ Protect POST
        public async Task<IActionResult> Create(PatientModel model)
        {
            model.PatientId = Guid.NewGuid();
            model.RegisteredAt = DateTime.UtcNow;
            await _patientService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ✅ Protect POST
        public async Task<IActionResult> Edit(Guid id, PatientModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _patientService.UpdateAsync(id, model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // ✅ Protect POST
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _patientService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
