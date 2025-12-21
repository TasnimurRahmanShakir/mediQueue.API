using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mediQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController(IDbOperation<Patient> patientOperationRepo, IMapper _mapper) : ControllerBase
    {
        private readonly IDbOperation<Patient> patientOperation = patientOperationRepo;
        private readonly IMapper mapper = _mapper;

        // ===========================
        // 1. GET ALL
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await patientOperation.GetAllAsync();
            var result = mapper.Map<ICollection<PatientDTO.Response>>(patients);
            return Ok(result);
        }

        // ===========================
        // 2. GET BY ID (Added this)
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patient = await patientOperation.GetByIdAsync(id);
            if (patient == null) return NotFound();

            var result = mapper.Map<PatientDTO.Response>(patient);
            return Ok(result);
        }

        // ===========================
        // 3. SEARCH
        // ===========================
        [HttpGet("search/{param}")]
        public async Task<IActionResult> Search(string param)
        {
            var patients = await patientOperation.FindAsync(p =>
                p.Name.Contains(param) ||
                p.PhoneNumber.Contains(param)
            );

            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found matching.");
            }

            var result = mapper.Map<ICollection<PatientDTO.Response>>(patients);
            return Ok(new
            {
                message = "Patient Found Successfully",
                result = result
            });
        }

        // ===========================
        // 4. CREATE
        // ===========================
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PatientDTO.Create patientdto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var patient = mapper.Map<Patient>(patientdto);

                await patientOperation.AddAsync(patient);
                int res = await patientOperation.SaveChangesAsync();

                var response = mapper.Map<PatientDTO.Response>(patient);

                if (res == 0)
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok(new
                {
                    message = "A New Patient Registered Successfully",
                    result = response
                });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                return BadRequest("This data already exists." + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}