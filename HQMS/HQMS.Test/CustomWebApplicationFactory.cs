using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using HQMS.Test.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.Services.Users;
using System;

namespace HQMS.Test
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext if any, and replace with InMemory
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Seed the user
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureCreated();

                    db.Users.Add(new User
                    {
                        Email = "admin@qms.com",
                        Password = "Admin@123", // hash if needed
                        Role = "Admin"
                    });

                    db.SaveChanges();
                }
            });
        }
    }


}
