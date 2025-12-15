using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;



namespace mediQueue.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionistController(IDbOperation<Receptionist> receptionistOperation, IMapper mapper) : ControllerBase
    {
        private readonly IDbOperation<Receptionist> receptionistOperation = receptionistOperation;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await receptionistOperation.GetAllAsync();
            var result = mapper.Map<ICollection<ReceptionistDTO.Response>>(entities);
            return Ok(result);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await receptionistOperation.GetByIdAsync(id);
            if (entity == null) return NotFound();

            receptionistOperation.DeleteAsync(entity);
            await receptionistOperation.SaveChangesAsync();
            return NoContent(); // 204 No Content for successful deletion
        }

        // ------------
        // CREATE 
        // ------------
        [HttpPost("create")]
        public async Task<IActionResult> CreateDoctor([FromBody] ReceptionistDTO.Create receptionistDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var receptionist = mapper.Map<Receptionist>(receptionistDto);

                await receptionistOperation.AddAsync(receptionist);
                await receptionistOperation.SaveChangesAsync();

                var responseDto = mapper.Map<ReceptionistDTO.Response>(receptionist);
                return Ok(new
                {
                    message = "Doctor added successfully",
                    result = responseDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to create doctor profile: " + ex.Message);
            }
        }
    }

}
