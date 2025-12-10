// Add other necessary usings
using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

[Route("api/[controller]")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly IDbOperation<Doctor> doctorOperation;
    private readonly IMapper mapper;

    public DoctorController(IDbOperation<Doctor> doctorOperation, IMapper mapper)
    {
        this.doctorOperation = doctorOperation;
        this.mapper = mapper;
    }

    // -----------
    // GET ALL
    // -----------
    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {

        //    var doctors = await doctorOperation.GetAllAsync(
        //    includes: new Expression<Func<Doctor, object>>[] { d => d.User }
        //);
        // NOTE: If your repository doesn't support .Include(d => d.User), 
        // the User.Name property in the DTO will be null.
        var doctors = await doctorOperation.GetAllAsync();
        var result = mapper.Map<ICollection<DoctorDTO.Response>>(doctors);
        return Ok(result);
    }

    // ------------
    // CREATE 
    // ------------
    [HttpPost("create")]
    public async Task<IActionResult> CreateDoctor([FromBody] DoctorDTO.Create doctorDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var doctor = mapper.Map<Doctor>(doctorDto);

            await doctorOperation.AddAsync(doctor);
            await doctorOperation.SaveChangesAsync();

            var responseDto = mapper.Map<DoctorDTO.Response>(doctor);
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

    // ----------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // This assumes you have a way to fetch the Doctor and INCLUDE the related User.
        var doctor = await doctorOperation.GetByIdAsync(id);
        if (doctor == null) return NotFound("Doctor profile not found.");

        // If the above method didn't include User, the mapper will fail to get the name.
        var result = mapper.Map<DoctorDTO.Response>(doctor);
        return Ok(result);
    }
}