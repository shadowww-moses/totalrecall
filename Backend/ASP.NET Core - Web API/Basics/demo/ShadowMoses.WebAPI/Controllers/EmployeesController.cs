using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ShadowMoses.DAL.EF.Context;
using ShadowMoses.DAL.EF.Entities;

namespace ShadowMoses.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private NorthwindContext _context;
        private LinkGenerator _linkGenerator;

        public EmployeesController(NorthwindContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var all = await _context.Employees.ToListAsync();

                return Ok(all);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server failure");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var emp = await _context.Employees
                    .FirstOrDefaultAsync(emp => emp.EmployeeId == id);

                if (emp == default)
                    return NotFound(String.Format("Employee with id {0} not found", id));

                return Ok(emp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server failure");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetFiltered(string? firstName, string? lastName, string? title, string? country)
        {
            try
            {
                var emp = await _context.Employees
                    .Where(emp => (firstName != null) ? emp.FirstName == firstName : true)
                    .Where(emp => (lastName != null) ? emp.LastName == lastName : true)
                    .Where(emp => (title != null) ? emp.Title == title : true)
                    .Where(emp => (country != null) ? emp.Country == country : true)
                    .Select(emp => new
                    {
                        firstName = emp.FirstName,
                        lastName = emp.LastName,
                        title = emp.Title,
                        country = emp.Country
                    })
                    .ToArrayAsync();

                if (!emp.Any())
                    return NotFound("No matches found");

                return Ok(emp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server failue");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostEmployee(Employees employee)
        {
            try
            {
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                // Не добавляется айдишник
                string path = _linkGenerator.GetPathByAction("GetEmployeeById", "Employees", new { id = employee.EmployeeId });

                return Created(path, employee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server failue");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employees employee)
        {
            try
            {
                var emp = await _context.Employees.FirstOrDefaultAsync(emp => emp.EmployeeId == id);

                if (emp == null)
                    return NotFound(String.Format("No employee with id {0} found", id));

                emp.FirstName = employee.FirstName;
                emp.LastName = employee.LastName;
                emp.City = employee.City;
                _context.Update(emp);
                await _context.SaveChangesAsync();

                return Ok(emp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server failue");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var emp = await _context.Employees.FirstOrDefaultAsync(emp => emp.EmployeeId == id);

                if (emp == null)
                    return NotFound(String.Format("No employee with id {0} found", id));

                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();

                return Ok("Employee was deleted successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server failue");
            }
        }
    }
}
