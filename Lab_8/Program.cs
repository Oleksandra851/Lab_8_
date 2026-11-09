using Lab_8.Data;
using Lab_8.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UniversityDb")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
    db.Database.EnsureCreated();

    if (!db.Positions.Any())
    {
        db.Positions.AddRange(
            new Position { PositionName = "Професор" },
            new Position { PositionName = "Доцент" },
            new Position { PositionName = "Асистент" }
        );
        db.SaveChanges();
    }

    if (!db.Subjects.Any())
    {
        db.Subjects.AddRange(
            new Subject { SubjectName = "Платформи КІС" },
            new Subject { SubjectName = "Системне програмування" },
            new Subject { SubjectName = "Бази даних" },
            new Subject { SubjectName = "Веб-програмування" },
            new Subject { SubjectName = "Захист інформації" },
            new Subject { SubjectName = "Паралельні обчислення" },
            new Subject { SubjectName = "Штучний інтелект" }
        );
        db.SaveChanges();
    }
}

app.UseStaticFiles(); 
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();