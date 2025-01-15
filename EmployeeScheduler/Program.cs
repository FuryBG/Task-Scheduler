using Application.Interfaces;
using Application.Services;
using Application.UseCases;
using Domain.Interfaces;
using EmployeeScheduler.Components;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmployeeScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                         builder.Configuration.GetConnectionString("DefaultConnection"),
                         sqlOptions => sqlOptions.MigrationsAssembly("Infrastructure")).UseLazyLoadingProxies());


            builder.Services.AddScoped(typeof(IDbRepository<>), typeof(DbRepository<>));
            builder.Services.AddScoped<IGetEmployeesUseCase, GetEmployeesUseCase>();
            builder.Services.AddScoped<ICreateShiftUseCase, CreateShiftUseCase>();
            builder.Services.AddScoped<IUpdateShiftUseCase, UpdateShiftUseCase>();
            builder.Services.AddScoped<IDeleteShiftUseCase, DeleteShiftUseCase>();
            
            builder.Services.AddScoped<IShiftValidationService, ShiftValidationService>();
            builder.Services.AddScoped<IWeekService, WeekService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
