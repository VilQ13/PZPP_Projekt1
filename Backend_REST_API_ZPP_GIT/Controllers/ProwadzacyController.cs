using Backend_REST_API_ZPP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // Добавьте эту строку


[ApiController]
[Route("api/prowadzacy")]
[Produces("application/json")]
public class ProwadzacyController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProwadzacyController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("get-full-name")]
    public async Task<IActionResult> GetFullName(string abbreviation)
    {
        if (string.IsNullOrEmpty(abbreviation))
        {
            return BadRequest("Abbreviation is required.");
        }

        var teacher = await _context.Prowadzacies
            .Where(t => t.Skrot.ToUpper() == abbreviation.ToUpper())
            .FirstOrDefaultAsync();

        if (teacher == null)
        {
            return NotFound("Teacher not found.");
        }

        // Составляем полное имя из Титула, Имени и Фамилии
        string fullName = $"{teacher.Tytul} {teacher.Imie} {teacher.Nazwisko}".Trim();

        return Ok(new { fullName });
    }
}
