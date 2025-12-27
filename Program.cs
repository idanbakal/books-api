using BooksApi.Data;
using BooksApi.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Book>("Books");
    return builder.GetEdmModel();
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Supabase")));

builder.Services.AddControllers()
    .AddOData(opt => opt
        .AddRouteComponents("odata", GetEdmModel())
        .Select().Filter().OrderBy().Expand().Count()
        .SetMaxTop(200));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("DevCors");

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "API is running ✅");

app.Run();
