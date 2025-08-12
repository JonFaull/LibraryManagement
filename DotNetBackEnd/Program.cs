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


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});
 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer("DefaultConnection"));
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
