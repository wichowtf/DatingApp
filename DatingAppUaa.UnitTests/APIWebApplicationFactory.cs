﻿using DatingApp.Api;
using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingAppUaa.UnitTests
{
    class APIWebApplicationFactory<TStartUp> : WebApplicationFactory<Startup>
    {
        private IConfiguration Configuration { get; set; }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureAppConfiguration((ctx, cbld) =>
            {
                cbld.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();

                Configuration = cbld.Build();
            })

            .ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<DataContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                // Add a database context using an in-memory database for testing.
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                    options.EnableSensitiveDataLogging();
                });
            })

            .ConfigureTestServices(async services =>
            {

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var context = sp.GetRequiredService<DataContext>();
                    var userManager = sp.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = sp.GetRequiredService<RoleManager<AppRole>>();

                    try
                    {
                        await context.Database.MigrateAsync();
                        await Seed.SeedUsers(userManager, roleManager);
                    }
                    catch (Exception ex)
                    {
                        var logger = sp.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error has occurred during migration/seeding.");
                    }
                }
            });
        }

        private void __loadTestData(DataContext appDbContext)
        {
            appDbContext.Database.EnsureCreated();
            if (!LoadTestData<AppRole>.Run(appDbContext, "RoleSeedData.json")) throw new Exception("Unable to seed roles data.");
            if (!LoadTestData<AppUser>.Run(appDbContext, "UserSeedData.json")) throw new Exception("Unable to seed users data.");
        }
    }

    internal static class LoadTestData<T> where T : class
    {
        internal static readonly string FLD_SEED_DATA = "SeedData";
        internal static bool Run(DbContext context, string fileName)
        {
            if (context.Set<T>().Any()) return true;
            var seedType = new List<T>();

            using (StreamReader r = new(Path.Combine(Environment.CurrentDirectory, FLD_SEED_DATA, fileName)))
            {
                string json = r.ReadToEnd();
                seedType = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions { IgnoreNullValues = true });
            }
            if (seedType == null) return false;

            context.AddRange(seedType);
            context.SaveChanges();

            return true;
        }
    }
}
