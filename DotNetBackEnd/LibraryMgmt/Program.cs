using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using Microsoft.EntityFrameworkCore;
using LibraryMgmt.Helpers;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;
using LibraryMgmt.Services;
using LibraryMgmt.DTOs;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

/*builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();*/




//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];



// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

/*builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer("DefaultConnection"));*/
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

/*builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));*/


/*builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));*/


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<JsonPatchExampleFilter>();
    options.SchemaFilter<SwaggerDefaultValues>();
});

builder.Services.AddAutoMapper(typeof(MappingProfile));






builder.Services.AddScoped<IBookStatusRepository, BookStatusRepository>();
builder.Services.AddScoped<IBookStatusService, BookStatusService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

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

app.Run();
