using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mediQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController(IDbOperation<Appointment> Operation, IMapper _mapper) : ControllerBase
    {
        private readonly IDbOperation<Appointment> appointmentOperation = Operation;
        private readonly IMapper mapper = _mapper;

        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] AppointmentDTO.Create appointmentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appointment = mapper.Map<Appointment>(appointmentDto);
            await appointmentOperation.AddAsync(appointment);
            await appointmentOperation.SaveChangesAsync();

            // NOTE: Retrieving the response DTO will require INCLUDEs for Patient and Doctor to map names.
            var responseDto = mapper.Map<AppointmentDTO.Response>(appointment);

            return Created("", responseDto);
        }

    }
}