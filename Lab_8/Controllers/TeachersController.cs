using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab_8.Data;
using Lab_8.DTOs;
using Lab_8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab_8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly UniversityDbContext _context;

        public TeachersController(UniversityDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
        {
            var rawTeachers = await _context.Teachers
                .Include(t => t.Position)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .ToListAsync();

            var dtos = rawTeachers.Select(t => new TeacherDto
            {
                TeacherID = t.TeacherID,
                LastName = t.LastName,
                FirstName = t.FirstName,
                MiddleName = t.MiddleName,
                Phone = t.Phone,
                Workplace = t.Workplace,
                PositionID = t.PositionID,
                PositionName = t.Position != null ? t.Position.PositionName : string.Empty,
                HomeAddress = t.HomeAddress,
                Characteristic = t.Characteristic,
                Subjects = t.TeacherSubjects.Select(ts => new SubjectDto
                {
                    SubjectID = ts.SubjectID,
                    SubjectName = ts.Subject != null ? ts.Subject.SubjectName : string.Empty,
                    HoursRead = ts.HoursRead
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
        {
            var t = await _context.Teachers
                .Include(t => t.Position)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (t == null)
            {
                return NotFound();
            }

            var dto = new TeacherDto
            {
                TeacherID = t.TeacherID,
                LastName = t.LastName,
                FirstName = t.FirstName,
                MiddleName = t.MiddleName,
                Phone = t.Phone,
                Workplace = t.Workplace,
                PositionID = t.PositionID,
                PositionName = t.Position != null ? t.Position.PositionName : string.Empty,
                HomeAddress = t.HomeAddress,
                Characteristic = t.Characteristic,
                Subjects = t.TeacherSubjects.Select(ts => new SubjectDto
                {
                    SubjectID = ts.SubjectID,
                    SubjectName = ts.Subject != null ? ts.Subject.SubjectName : string.Empty,
                    HoursRead = ts.HoursRead
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherDto>> PostTeacher([FromBody] TeacherCreateUpdateDto dto)
        {
            var teacher = new Teacher
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                Phone = dto.Phone,
                Workplace = dto.Workplace,
                PositionID = dto.PositionID,
                HomeAddress = dto.HomeAddress,
                Characteristic = dto.Characteristic
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            if (dto.SelectedSubjects != null && dto.SelectedSubjects.Any())
            {
                foreach (var sub in dto.SelectedSubjects)
                {
                    _context.TeacherSubjects.Add(new TeacherSubject
                    {
                        TeacherID = teacher.TeacherID,
                        SubjectID = sub.SubjectID,
                        HoursRead = sub.HoursRead
                    });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.TeacherID }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, [FromBody] TeacherCreateUpdateDto dto)
        {
            if (id != dto.TeacherID)
            {
                return BadRequest();
            }

            var existingTeacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (existingTeacher == null)
            {
                return NotFound();
            }

            existingTeacher.LastName = dto.LastName;
            existingTeacher.FirstName = dto.FirstName;
            existingTeacher.MiddleName = dto.MiddleName;
            existingTeacher.Phone = dto.Phone;
            existingTeacher.Workplace = dto.Workplace;
            existingTeacher.PositionID = dto.PositionID;
            existingTeacher.HomeAddress = dto.HomeAddress;
            existingTeacher.Characteristic = dto.Characteristic;

            _context.TeacherSubjects.RemoveRange(existingTeacher.TeacherSubjects);

            if (dto.SelectedSubjects != null && dto.SelectedSubjects.Any())
            {
                foreach (var sub in dto.SelectedSubjects)
                {
                    _context.TeacherSubjects.Add(new TeacherSubject
                    {
                        TeacherID = id,
                        SubjectID = sub.SubjectID,
                        HoursRead = sub.HoursRead
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (teacher == null)
            {
                return NotFound();
            }

            if (teacher.TeacherSubjects != null)
            {
                _context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}