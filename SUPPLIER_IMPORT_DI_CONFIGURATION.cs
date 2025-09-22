// Add this configuration to your Program.cs or Startup.cs

using GoodsEnterprise.Web.Services;
using GoodsEnterprise.DataAccess.Interface;
using GoodsEnterprise.DataAccess.Implementation;

// In Program.cs (ASP.NET Core 6+)
var builder = WebApplication.CreateBuilder(args);

// Register ExcelDataReader encoding provider (required for Excel reading)
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Add existing services
builder.Services.AddControllersWithViews();

// Add repository services (adjust based on your existing DI configuration)
builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));

// Add supplier import services
builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
builder.Services.AddScoped<ISupplierValidationService, SupplierValidationService>();
builder.Services.AddScoped<ISupplierImportService, SupplierImportService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map controllers for API endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers
app.MapControllers();

app.Run();

/*
Alternative for Startup.cs (ASP.NET Core 3.1/5.0):

public void ConfigureServices(IServiceCollection services)
{
    // Register ExcelDataReader encoding provider
    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    
    // Add existing services
    services.AddControllersWithViews();
    
    // Add repository services
    services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));
    
    // Add supplier import services
    services.AddScoped<IExcelReaderService, ExcelReaderService>();
    services.AddScoped<ISupplierValidationService, SupplierValidationService>();
    services.AddScoped<ISupplierImportService, SupplierImportService>();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Your existing configuration...
    
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapControllers(); // For API controllers
    });
}
*/
