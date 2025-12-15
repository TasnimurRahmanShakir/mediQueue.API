using AutoMapper;
using mediQueue.API.Context;
using mediQueue.API.Helpers;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using mediQueue.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace mediQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDbOperation<User> userOperation;
        private readonly IMapper mapper;
        private readonly JwtService jwtService;

        public AuthController(IDbOperation<User> userOperation, IMapper mapper, JwtService jwtService)
        {
            this.userOperation = userOperation;
            this.mapper = mapper;
            this.jwtService = jwtService;

        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await userOperation.GetAllWithIncludesAsync(
                u => u.DoctorProfile!,
                u => u.ReceptionistProfile!
            );

            var responseDto = mapper.Map<ICollection<UserDTO.Response>>(users);
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
            var users = await userOperation.FindAsync(p =>
                p.Name.Contains(param) ||
                p.PhoneNumber.Contains(param) ||
                p.Email.Contains(param)
            );

            if (users == null || !users.Any())
            {
                return NotFound("No patients found matching.");
            }

            var result = mapper.Map<ICollection<UserDTO.Response>>(users);
            return Ok(new
            {
                messge = "Find Successfully",
                result = result
            });
        }



        [HttpPost("register")]
        //[Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Register([FromForm] UserDTO.Create userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Console.WriteLine(userDto);
            try
            {
                var user = mapper.Map<User>(userDto);

                if (userDto.Image != null && userDto.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    // Generate unique filename
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + userDto.Image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file to stream
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userDto.Image.CopyToAsync(stream);
                    }

                    // Set the URL property on the Entity
                    user.ImageUrl = $"/uploads/{uniqueFileName}";
                }
                else
                {
                    // Optional: Set a default placeholder if no image provided
                    user.ImageUrl = null;
                }

                // 4. Hash Password
                user.PasswordHash = PasswordHelper.HashPassword(userDto.Password);

                // 5. Save to DB
                await userOperation.AddAsync(user);
                await userOperation.SaveChangesAsync();

                var responseDto = mapper.Map<UserDTO.Response>(user);

                return Ok(new
                {
                    message = "User registered successfully",
                    result = responseDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Registration failed: " + ex.Message);
            }
        }

        [HttpGet("{id:Guid}")]

        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await userOperation.GetByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            var result = mapper.Map<UserDTO.Response>(user);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] UserDTO.LoginRequest userDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = await userOperation.FindAsync(u => u.Email == userDto.Email);
            var user = users.FirstOrDefault();
            if (user == null) return Unauthorized("User Does not exist");

            bool IsPassValid = PasswordHelper.VerifyPassword(userDto.Password, user.PasswordHash);

            if (!IsPassValid) return BadRequest("Email Or Password Incorrect!");

            var result_ = mapper.Map<UserDTO.Response>(user);

            var token = jwtService.JwtTokenGenerator(user);

            if (token == null) return BadRequest("Token generation failed");

            var refreshToken = jwtService.GenerateRefreshToken();

            // SAVE TO DB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userOperation.SaveChangesAsync();

            return Ok(new
            {
                message = "Login Successfull",
                result = mapper.Map<UserDTO.Response>(user),
                token = token,
                refreshToken = refreshToken
            });


        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] UserDTO.RefreshRequest request)
        {

            var users = await userOperation.FindAsync(u => u.RefreshToken == request.RefreshToken);
            var user = users.FirstOrDefault();

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            var newToken = jwtService.JwtTokenGenerator(user);
            var newRefreshToken = jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userOperation.SaveChangesAsync();

            return Ok(new
            {
                token = newToken,
                refreshToken = newRefreshToken
            });
        }


    }


}
