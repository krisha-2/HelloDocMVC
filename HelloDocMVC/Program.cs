using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;

using HelloDocMVC.Entity.Models;
using Rotativa.AspNetCore;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;

var builder = WebApplication.CreateBuilder(args);
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HelloDocDbContext>();
builder.Services.AddScoped<IAdminDashboard, AdminDashboard>();
builder.Services.AddScoped<IPatientForms, PatientForms>();
builder.Services.AddScoped<IComboBox, ComboBox>();
builder.Services.AddScoped<IPatientDashboard, PatientDashboard>();
builder.Services.AddScoped<IJwtSession, JwtSession>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IAdminProfile, AdminProfile>();
builder.Services.AddScoped<IProvider, Provider>();
builder.Services.AddScoped<IAccessRole, AccessRole>();
builder.Services.AddScoped<IScheduling, Scheduling>();
builder.Services.AddScoped<IPartners, Partners>();
builder.Services.AddScoped<IRecords, Records>();
builder.Services.AddScoped<IRequestByPatient, RequestByPatient>();
builder.Services.AddScoped<IInvoicing, Invoicing>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseRotativa();
app.UseNotyf();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index1}/{id?}");
app.Run();
