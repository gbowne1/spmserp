using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using System.Data.Entity;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Web.Filter;
using Web.Models;
using NETCore.ViewModels;
using NETCore.Base;
using NETCore.Models;
using NETCore.Repositories.Data;
using Core.Service.APIResponses;
using Core.Service.Interfaces;
using Core.ViewModel.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Build.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using System.Xml;
using System.Xml.Linq;
using System.Web.Http;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Web.Http.Description;
using Employee.Models;



namespace spmserp.Api.Controllers
{
    [EnableCors("AllowOrigin")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Administrator,Employee")]
    [ApiController]
    {
        public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly MySQLServerDBContext _context;
        public EmployeeController(ILogger<EmployeeController> logger, MySQLServerDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("GetAllEmployees", Name = "GetAllEmployees")]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            return Ok(await _context.Employees.ToListAsync());
            //return Ok(Employees);
        }

        [HttpGet("GetEmployee/{id}", Name = "GetEmployee")]
        public async Task<ActionResult<Employee>> GetEmployee(long id)
        {
            //var emp = Employees.Find(x => x.Id == id) ;
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return BadRequest("Employee not found");
            return Ok(emp);
        }

        [HttpPost("SaveEmployee", Name = "SaveEmployee")]
        public async Task<ActionResult<List<Employee>>> SaveEmployee([FromBody] Employee emp)
        {
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            return Ok(await _context.Employees.ToListAsync());
            //Employees.Add(emp);
            //return Ok(Employees);
        }

        [HttpPut("UpdateEmployee", Name = "UpdateEmployee")]
        public async Task<ActionResult<List<Employee>>> UpdateEmployee([FromBody] Employee employee)
        {
            Employee emp = await _context.Employees.FindAsync(employee.Id);

            //Employee emp = Employees.Find(x => x.Id == employee.Id);
            if (emp == null)
                return BadRequest("Employee not found");

            emp.FirstName = employee.FirstName;
            emp.LastName = employee.LastName;
            emp.PhoneNumber = employee.PhoneNumber;
            emp.Email = employee.Email;
            emp.Country = employee.Country;

            await _context.SaveChangesAsync();

            return Ok(await _context.Employees.ToListAsync());
            //return Ok(Employees.Find(x => x.Id == employee.Id));
        }

        [HttpDelete("DeleteEmployee/{id}", Name = "DeleteEmployee")]
        public async Task<ActionResult<List<Employee>>> DeleteEmployee(long id)
        {
            Employee emp = await _context.Employees.FindAsync(id);
            //Employee emp = Employees.Find(x => x.Id == id);
            if (emp == null)
                return BadRequest("Employee not found");

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return Ok(await _context.Employees.ToListAsync());
            //return Ok(Employees);
        }
    }
}