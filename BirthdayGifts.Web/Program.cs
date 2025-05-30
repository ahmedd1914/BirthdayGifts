using BirthdayGifts.Services.Interfaces.Employee;
using BirthdayGifts.Services.Interfaces.Gift;
using BirthdayGifts.Services.Interfaces.VotingSession;
using BirthdayGifts.Services.Interfaces.Vote;
using BirthdayGifts.Services.Interfaces.Authentication;
using BirthdayGifts.Services.Implementations.Employee;
using BirthdayGifts.Services.Implementations.Gift;
using BirthdayGifts.Services.Implementations.VotingSession;
using BirthdayGifts.Services.Implementations.Vote;
using BirthdayGifts.Services.Implementations.Authentication;
using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Repository.Implementations;
using BirthdayGifts.Services.Helpers;
using BirthdayGifts.Web.Models.ViewModels;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(BirthdayGifts.Services.Helpers.MappingProfile), 
                             typeof(BirthdayGifts.Web.Models.ViewModels.MappingProfile));

// Register repositories with connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IEmployeeRepository>(sp => new EmployeeRepository(connectionString));
builder.Services.AddScoped<IGiftRepository>(sp => new GiftRepository(connectionString));
builder.Services.AddScoped<IVotingSessionRepository>(sp => new VotingSessionRepository(connectionString));
builder.Services.AddScoped<IVoteRepository>(sp => new VoteRepository(connectionString));

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IVotingSessionService, VotingSessionService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

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

// Add session middleware before authorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
