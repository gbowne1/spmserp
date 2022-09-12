using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spmserp.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesRepository _salesRepository;
        public SalesController(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }
        [HttpGet("")]
        public async Task<IEnumerable<Sale>> GetSalesDetailsAsync()
        {
            var details = await _salesRepository.GetSalesDetailsAsync();
            return details;
        }
        [HttpGet("{customerId}")]
        [ActionName("GetSale")]
        public async Task<IActionResult> GetSale([FromRoute] string customerId)
        {
            var detail = await _salesRepository.GetSale(customerId);
            if (detail != null)
            {
                return Ok(detail);
            }
            return NotFound("Not Found");
        }
        [HttpPost("")]
        public async Task<IActionResult> AddSale([FromBody] Sale sale)
        {
            await _salesRepository.AddSale(sale);
            return CreatedAtAction(nameof(GetSale), new { customerId = sale.CustomerId }, sale);
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdateSaleDetailsAsync([FromBody] Sale sale)
        {
            var detail = await _salesRepository.UpdateSaleDetailsAsync(sale);
            if (detail != null)
            {
                return Ok(detail);
            }
            else
            {
                return NotFound("not found");
            }
        }
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteSaleAsync([FromRoute] string customerId)
        {
            await _salesRepository.DeleteSaleAsync(customerId);
            return Ok();
        }
    }
}