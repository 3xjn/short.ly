using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Shortly_API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CodeMapContext>(opt =>
    opt.UseInMemoryDatabase("CodeMap"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


var app = builder.Build();



app.UseAuthorization();

var defaultFilesOptions = new DefaultFilesOptions();
var staticFileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.WebRootPath, "app"));
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = staticFileProvider
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = staticFileProvider
});

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}


app.MapControllers();

app.MapFallback(async context =>
{
    var path = context.Request.Path.Value;

    if (string.IsNullOrEmpty(path)) return;

    var code = path.TrimStart('/');

    if (Regex.IsMatch(code, "^[a-z0-9]{6}$"))
    {
        context.Response.Redirect($"/redirect/{code}");
    }
    else
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var indexPath = Path.Combine(env.WebRootPath, "app", "index.html");

        if (File.Exists(indexPath))
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(indexPath);
        }
    }

    //else
    //{
    //    context.Response.Redirect("index.html");
    //}
});



app.UseCors("AllowSpecificOrigin");

app.Run();
