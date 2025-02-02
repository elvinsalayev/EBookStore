using EbookStore.Infrastructure.Repositories.Interfaces;
using EbookStore.Infrastructure.Repositories;
using EbookStore.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using EbookStore.Application.Common.Mappings.Catalog;
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
}

app.Run();
