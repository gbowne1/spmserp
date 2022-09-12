using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spmserp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace spmserp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ModelContext _context;
        public SalaryController()
        {
            _context = new ModelContext();
        }

        public class SalaryInfo
        {
            public string? occupation { get; set; }
            public decimal? amount { get; set; }
            public decimal? count { get; set; }
        }

        // GET
        [HttpGet("GetSalaryInfo")]
        public async Task<ActionResult<List<SalaryInfo>>> GetSalaryInfo()
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employees)
                .ToListAsync();

            if (salaries == null) return NoContent();

            List<SalaryInfo> salaryInfos = new List<SalaryInfo>();
            foreach(var salary in salaries)
            {
                SalaryInfo salaryInfo = new SalaryInfo();
                salaryInfo.occupation = salary.Occupation;
                salaryInfo.amount = salary.Amount;
                salaryInfo.count = salary.Employees.Count();
                salaryInfos.Add(salaryInfo);
            }

            return Ok(salaryInfos);
        }

        public class SalaryRecord
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? occupation { get; set; }
            public decimal? amount { get; set; }
            public string? time { get; set; }
        }

        // GET
        [HttpGet("GetSalaryRecord")]
        public async Task<ActionResult<List<SalaryRecord>>> GetSalaryRecord(string? occupation, string? id, string? time_start, string? time_end)
        {
            var records = await _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.OccupationNavigation)
                .ToListAsync();

            try
            {
                List<SalaryRecord> salaryRecords = new List<SalaryRecord>();
                foreach (var record in records)
                {
                    if (id != null && record.EmployeeId.ToString() != id) continue;
                    DateTime start, end;
                    if (time_start != null) start = Convert.ToDateTime(time_start);
                    else start = DateTime.MinValue;
                    if (time_end != null) end = Convert.ToDateTime(time_end);
                    else end = DateTime.MaxValue;
                    if (record.PayDatetime < start || record.PayDatetime > end) continue;
                    if (occupation != null && record.Employee.Occupation != occupation) continue;

                    SalaryRecord salaryRecord = new SalaryRecord();
                    salaryRecord.id = record.EmployeeId.ToString();
                    salaryRecord.name = record.Employee.Name;
                    salaryRecord.occupation = record.Employee.Occupation;
                    salaryRecord.time = record.PayDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                    salaryRecord.amount = record.Employee.OccupationNavigation.Amount;
                    salaryRecords.Add(salaryRecord);
                }

                return Ok(salaryRecords);
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class PostSalaryInfo
        {
            public string? occupation { get; set; }
            public string? amount { get; set; }
        }

        // POST
        [HttpPost("PostOneSalaryInfo")]
        public async Task<ActionResult> PostOneSalaryInfo(PostSalaryInfo p)
        {
            if (p.occupation == null || p.amount == null) return BadRequest();

            try
            {
                Salary salary = new Salary();
                salary.Occupation = p.occupation;
                salary.Amount = Convert.ToDecimal(p.amount);
                _context.Salaries.Add(salary);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class PostSalaryRecord
        {
            public string? id { get; set; }
            public string? time { get; set; }
        }

        // POST
        [HttpPost("PostOneSalaryRecord")]
        public async Task<ActionResult> PostOneSalaryRecord(PostSalaryRecord p)
        {
            if (p.id == null || p.time == null) return BadRequest();

            try
            {
                Payroll payroll = new Payroll();
                payroll.EmployeeId = Convert.ToDecimal(p.id);
                payroll.PayDatetime = Convert.ToDateTime(p.time);
                _context.Payrolls.Add(payroll);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE
        [HttpDelete("DeleteSalaryInfo")]
        public async Task<ActionResult> DeleteSalaryInfo(string? occupation)
        {
            if (occupation == null) return BadRequest();
            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.Occupation == occupation);
            if (salary == null) return NoContent();

            try
            {
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}