using Microsoft.EntityFrameworkCore;
using CourseProject.Data;
using CourseProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<SimpleMemoryCache>();

builder.Services.AddDbContext<CourseProjectAzureSqlContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("quachtridat-fdu") ?? throw new InvalidOperationException("Connection string 'ConnectionStrings:quachtridat-fdu' not found.")));

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    using (var context = new CourseProjectAzureSqlContext(services.GetRequiredService<DbContextOptions<CourseProjectAzureSqlContext>>())) {
        if (context != null) {
            CourseProject.SeedData.SeedFor(context, app.Logger);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
