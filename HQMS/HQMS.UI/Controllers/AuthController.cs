﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using HospitalQueueSystem.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Net.Http;
using HospitalQueueSystem.Web.Interfaces;

namespace HospitalQueueSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            ViewBag.HubUrl = _configuration["SignalR:HubUrl"];
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (isSuccess, tokenData, errorMessage) = await _authService.LoginAsync(model);

            if (isSuccess && tokenData?.Token != null)
            {
                HttpContext.Session.SetString("JwtToken", tokenData.Token);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("UserName", tokenData.UserId ?? model.Email);
                HttpContext.Session.SetString("UserRole", tokenData.Role ?? "");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("JwtToken", tokenData.Token)
                };

                if (!string.IsNullOrEmpty(tokenData.UserId))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, tokenData.UserId));
                if (!string.IsNullOrEmpty(tokenData.Role))
                    claims.Add(new Claim(ClaimTypes.Role, tokenData.Role));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToAction("Index", "Dashboard");

            }

            ModelState.AddModelError("", errorMessage ?? "Login failed.");
            TempData["ErrorMessage"] = "Invalid credentials!";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
