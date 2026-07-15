using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Questly.Data.Context;
using Questly.Data.Entities;
using Questly.Services.Implementations;
using Questly.Services.Interfaces;
using Questly.UI;
using Questly.UI.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuestlyDbContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = "0123456789";
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<QuestlyDbContext>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<ISurveyTemplateService, SurveyTemplateService>();
builder.Services.AddScoped<ISurveySessionService, SurveySessionService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

var app = builder.Build();

// Seed database
await app.Services.SeedDatabaseAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
