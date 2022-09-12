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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        [HttpGet("")]
        public async Task<IEnumerable<CustomerDetail>> GetCustomerDetailsAsync()
        {
            var details = await _customerRepository.GetCustomerDetailsAsync();
            return details;
        }
        [HttpGet("{customerId}")]
        [ActionName("GetCustomer")]
        public async Task<IActionResult> GetCustomer([FromRoute] int customerId)
        {
            var detail = await _customerRepository.GetCustomer(customerId);
            if (detail != null)
            {
                return Ok(detail);
            }
            return NotFound(" Not Found");
        }
        [HttpPost("")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDetail customer)
        {
            await _customerRepository.AddCustomer(customer);
            return CreatedAtAction(nameof(GetCustomer), new { customerId = customer.CustomerId }, customer);
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdateCustomerDetailsAsync([FromBody] CustomerDetail customer)
        {
            var detail = await _customerRepository.UpdateCustomerDetailsAsync(customer);
            if (detail != null)
            {
                return Ok(detail);
            }
            else
            {
                return NotFound(" not found");
            }
        }
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteEmployeeAsync([FromRoute] int customerId)
        {
            await _customerRepository.DeleteCustomerAsync(customerId);
            return Ok();
        }
    }
}