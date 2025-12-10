using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace mediQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IDbOperation<Invoice> invoiceOperation;
        private readonly IMapper mapper;

        // (Constructor Injection omitted for brevity)

        [HttpPut("Create")]
        public async Task<IActionResult> UpdateStatus([FromBody] InvoiceDTO.Create invoiceDto)
        {
            if (!ModelState.IsValid) return BadRequest();


            return Ok("Later implementataion");
        }
    }
}
