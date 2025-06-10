using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using HospitalQueueSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalQueueSystem.Web.Controllers
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
        public async Task<IActionResult> Create(PatientModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            model.PatientId = Guid.NewGuid();
            model.RegisteredAt = DateTime.UtcNow;
            await _patientService.CreateAsync(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, PatientModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _patientService.UpdateAsync(id, model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return View(patient);
            //await _patientService.DeleteAsync(id);
            //return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _patientService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
