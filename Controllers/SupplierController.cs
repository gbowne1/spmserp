using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace spmserp.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }
        [HttpGet("")]
        public async Task<IEnumerable<SupplierDetail>> GetSupplierDetails()
        {
            var details = await _supplierRepository.GetSupplierDetailsAsync();
            return details;
        }
        [HttpGet("{supplierId}")]
        [ActionName("GetSupplier")]
        public async Task<IActionResult> GetSupplier([FromRoute] string supplierId)
        {
            var detail = await _supplierRepository.GetSupplier(supplierId);
            if (detail != null)
            {
                return Ok(detail);
            }
            return NotFound("Not Found");
        }
        [HttpPost("")]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierDetail supplier)
        {
            await _supplierRepository.AddSupplier(supplier);
            return CreatedAtAction(nameof(GetSupplier), new { supplierId = supplier.SupplierId }, supplier);
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdateSupplierDetailsAsync([FromBody] SupplierDetail supplier)
        {
            var detail = await _supplierRepository.UpdateSupplierDetailsAsync(supplier);
            if (detail != null)
            {
                return Ok(detail);
            }
            else
            {
                return NotFound("Card not found");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplierAsync([FromRoute] string id)
        {
            await _supplierRepository.DeleteSupplierAsync(id);
            return Ok();
        }

    }
}