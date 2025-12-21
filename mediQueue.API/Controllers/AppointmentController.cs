using mediQueue.API.Context;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mediQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IDbOperation<Doctor> _doctorOperation;
        private readonly IDbOperation<Appointment> _appointmentOperation;
        private readonly IDbOperation<Patient> _patientOperation;
        private readonly ApplicationDbContext _context; 

        public AppointmentController(
            IDbOperation<Doctor> doctorOperation,
            IDbOperation<Appointment> appointmentOperation,
            IDbOperation<Patient> patientOperation,
            ApplicationDbContext context) 
        {
            _doctorOperation = doctorOperation;
            _appointmentOperation = appointmentOperation;
            _patientOperation = patientOperation;
            _context = context;
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetAppointmentsByDate([FromQuery] DateOnly date)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDate == date)
                .Select(a => new AppointmentDTO.Response
                {
                    Id = a.Id,
                    Doctor = a.Doctor ,
                    Patient = a.Patient,
                    Status = a.Status,
                    Time = a.AppointmentTime

                })
                .ToListAsync();

            return Ok(new
            {
                message = "Appointments fetched successfully",
                result = appointments
            });
        }

        [HttpPut("cancel/{id:Guid}")]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            var appointment = await _appointmentOperation.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            appointment.Status = "Cancelled";


            await _appointmentOperation.SaveChangesAsync();

            return Ok(new
            {
                message = "Appointment cancelled successfully",
                result = appointment
            });
        }


        [HttpGet("slots")]
        public async Task<IActionResult> GetDoctorSlots(Guid doctorId, DateOnly date)
        {
            var doctor = await _doctorOperation.GetByIdAsync(doctorId);
            if (doctor == null) return NotFound("Doctor not found");

            var appointments = await _appointmentOperation.FindAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == date &&
                a.Status != "Cancelled");

            var bookedTimes = appointments.Select(a => a.AppointmentTime).ToHashSet();

            // 3. Generate 30-min Slots
            var slots = new List<object>();

            TimeOnly currentTime = doctor.CounsilingStart;
            TimeOnly endTime = doctor.CounsilingEnd;

            while (currentTime < endTime)
            {
                if (currentTime.AddMinutes(30) > endTime) break;

                bool isBooked = bookedTimes.Contains(currentTime);

                slots.Add(new
                {
                    Time = currentTime,
                    FormattedTime = currentTime.ToString("h:mm tt"),
                    IsBooked = isBooked
                });

                currentTime = currentTime.AddMinutes(30);
            }

            return Ok(new
            {
                message = "Slot generate successfully",
                result = slots
            });
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentDTO.Create request)
        {
            // 1. Validation: Ensure we have EITHER a PatientId OR Patient Details
            if (request.PatientId == null && string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("You must provide either an existing PatientId or new Patient details.");
            }

            // 2. Check Appointment Availability (Concurrent Check)
            var existing = await _appointmentOperation.FindAsync(a =>
                a.DoctorId == request.DoctorId &&
                a.AppointmentDate == request.AppointmentDate &&
                a.AppointmentTime == request.AppointmentTime &&
                a.Status != "Cancelled");

            if (existing.Count != 0)
            {
                return BadRequest("This slot has already been booked.");
            }

            Guid finalPatientId;

            if (request.PatientId.HasValue)
            {
                finalPatientId = request.PatientId.Value;
            }
            else
            {
                var newPatient = new Patient
                {
                    Name = request.Name!,
                    PhoneNumber = request.PhoneNumber!,
                    BloodGroup = request.BloodGroup,
                    DOB = request.DOB ?? DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _patientOperation.AddAsync(newPatient);
                await _patientOperation.SaveChangesAsync(); 

                finalPatientId = newPatient.Id;
            }

            // 4. Create Appointment
            var appointment = new Appointment
            {
                DoctorId = request.DoctorId,
                PatientId = finalPatientId, 
                AppointmentDate = request.AppointmentDate,
                AppointmentTime = request.AppointmentTime,
                Reason = request.Reason,
                Status = "Pending"
            };

            await _appointmentOperation.AddAsync(appointment);
            int res = await _appointmentOperation.SaveChangesAsync();


            if(res != 0)
            {
                return Ok(new
                {
                    message = "Appointment booked successfully",
                    result = appointment
                });
            }
            else
            {
                return BadRequest("Appointment does not successfull");
            }
            
        }
    }
}