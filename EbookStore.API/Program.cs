using EbookStore.Application.Common.Mappings.Catalog;
using EbookStore.Identity.Data;
using EbookStore.Identity.Entities;
using EbookStore.Infrastructure.Repositories;
using EbookStore.Infrastructure.Repositories.Interfaces;
using EbookStore.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<EbookStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("cString"));
});

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("cString"));
});
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddScoped<SignInManager<AppUser>>();
builder.Services.AddScoped<RoleManager<AppRole>>();


builder.Services.Configure<IdentityOptions>(cfg =>
{
    cfg.User.RequireUniqueEmail = true;
    //cfg.User.AllowedUserNameCharacters = true;

    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireUppercase = false;
    cfg.Password.RequiredLength = 8;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 1;

    cfg.Lockout.MaxFailedAccessAttempts = 3;
    cfg.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 1, 0);
});


builder.Services.AddAutoMapper(typeof(EbookMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(CategoryMappingProfile).Assembly);


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await EbookStoreDbSeed.InitDbAsync(services);
    await IdentityDbSeed.InitMembershipAsync(services);
}

app.Run();
