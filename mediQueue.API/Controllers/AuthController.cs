using AutoMapper;
using mediQueue.API.Helpers;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using mediQueue.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserEntity = mediQueue.API.Model.Entity.User;

namespace mediQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Private readonly fields used by controller actions
        private readonly IDbOperation<UserEntity> _userOperation;
        private readonly IDbOperation<Doctor> _doctorOperation;
        private readonly IDbOperation<Receptionist> _receptionistOperation;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;

        // Constructor with null-checks to satisfy nullable analysis and ensure dependencies exist
        public AuthController(
            IDbOperation<UserEntity> userOperation,
            IDbOperation<Doctor> doctorOperation,
            IDbOperation<Receptionist> receptionistOperation,
            IMapper mapper,
            JwtService jwtService)
        {
            _userOperation = userOperation;
            _doctorOperation = doctorOperation;
            _receptionistOperation = receptionistOperation;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userOperation.GetAllWithIncludesAsync(
                u => u.DoctorProfile!,
                u => u.ReceptionistProfile!
            );

            var responseDto = _mapper.Map<ICollection<UserDTO.Response>>(users);
            return Ok(new
            {
                message = "All users get successfully",
                result = responseDto
            });
        }

        [HttpGet("search/{param}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string param)
        {
            var users = await _userOperation.FindAsync(p =>
                p.Name.Contains(param) ||
                p.PhoneNumber.Contains(param) ||
                p.Email.Contains(param)
            );

            if (users == null || !users.Any())
            {
                return NotFound("No patients found matching.");
            }

            var result = _mapper.Map<ICollection<UserDTO.Response>>(users);
            return Ok(new
            {
                messge = "Find Successfully",
                result = result
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserDTO.Create userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                string finalImageUrl = string.Empty;

                // =========================================================
                // 2. SAFE IMAGE UPLOAD LOGIC (Using Project Directory)
                // =========================================================
                if (userDto.Image != null && userDto.Image.Length > 0)
                {
                    try
                    {
                        // 1. Get the current Project Directory
                        string projectDir = Directory.GetCurrentDirectory();

                        // 2. Combine to create path: ProjectDir/wwwroot/uploads
                        // We put it in wwwroot so it can be accessed via URL later
                        string uploadDir = Path.Combine(projectDir, "wwwroot", "uploads");

                        // 3. Create directory if it doesn't exist
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        // 4. Generate unique filename
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(userDto.Image.FileName)}";
                        string fullPath = Path.Combine(uploadDir, fileName);

                        // 5. Save the file
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await userDto.Image.CopyToAsync(stream);
                        }

                        // 6. Set the URL path for DB (Relative path)
                        finalImageUrl = $"/uploads/{fileName}";
                    }
                    catch (Exception fileEx)
                    {
                        // Log error but DO NOT crash. Continue registration without image.
                        Console.WriteLine($"Image Upload Failed: {fileEx.Message}");
                    }
                }
                // =========================================================

                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    ImageUrl = finalImageUrl, // Will be empty string if upload failed
                    Role = userDto.Role,
                    Status = "Active"
                };

                user.PasswordHash = PasswordHelper.HashPassword(userDto.Password);

                await _userOperation.AddAsync(user);
                await _userOperation.SaveChangesAsync();

                // 2. Create and Save Role Profile
                if (userDto.Role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    var doctor = new Doctor
                    {
                        UserId = user.Id,
                        Specialization = userDto.Specialization,
                        LicenseNumber = userDto.LicenseNumber,
                        ConsultationFee = userDto.ConsultationFee ?? 0
                    };

                    await _doctorOperation.AddAsync(doctor);
                    await _doctorOperation.SaveChangesAsync();
                    user.DoctorProfile = doctor;
                }
                else if (userDto.Role.Equals("Receptionist", StringComparison.OrdinalIgnoreCase))
                {
                    var receptionist = new Receptionist
                    {
                        UserId = user.Id,
                        ShiftTime = userDto.ShiftTime
                    };

                    await _receptionistOperation.AddAsync(receptionist);
                    await _receptionistOperation.SaveChangesAsync();
                    user.ReceptionistProfile = receptionist;
                }

                return Ok(new
                {
                    message = "Registration successful",
                    result = user
                });
            }
            catch (Exception ex)
            {
                // Catches database errors or unexpected system errors
                return StatusCode(500, new
                {
                    message = "Registration failed due to server error",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userOperation.GetByIdWithIncludesAsync(id,
                    u => u.DoctorProfile!,
                    u => u.ReceptionistProfile!
                );
            if (user == null) return NotFound("User not found.");
           
                var result = _mapper.Map<UserDTO.Response>(user);
            return Ok(new
            {
                message = "User fetch successfully",
                result = result
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] UserDTO.LoginRequest userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = await _userOperation.FindAsync(u => u.Email == userDto.Email);
            var user = users.FirstOrDefault();

            if (user == null) return Unauthorized("User Does not exist");

            bool IsPassValid = PasswordHelper.VerifyPassword(userDto.Password, user.PasswordHash);
            if (!IsPassValid) return BadRequest("Email Or Password Incorrect!");

            var token = _jwtService.JwtTokenGenerator(user);

            if (token == null) return BadRequest("Token generation failed");

            var refreshToken = _jwtService.GenerateRefreshToken();

            // SAVE TO DB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userOperation.SaveChangesAsync();

            return Ok(new
            {
                message = "Login Successfull",
                result = _mapper.Map<UserDTO.Response>(user),
                token = token,
                refreshToken = refreshToken
            });
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userOperation.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            _userOperation.DeleteAsync(user);
            await _userOperation.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] UserDTO.RefreshRequest request)
        {
            var users = await _userOperation.FindAsync(u => u.RefreshToken == request.RefreshToken);
            var user = users.FirstOrDefault();

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            var newToken = _jwtService.JwtTokenGenerator(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userOperation.SaveChangesAsync();

            return Ok(new
            {
                token = newToken,
                refreshToken = newRefreshToken
            });
        }
    }
}