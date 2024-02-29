using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;

using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HelloDocDbContext>();
builder.Services.AddScoped<IAdminDashboard, AdminDashboard>();
builder.Services.AddScoped<IPatientForms, PatientForms>();
builder.Services.AddScoped<IComboBox, ComboBox>();
builder.Services.AddScoped<IPatientDashboard, PatientDashboard>();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

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
app.UseNotyf();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
