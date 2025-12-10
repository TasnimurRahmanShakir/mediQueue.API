using AutoMapper;
using mediQueue.API.Context;
using mediQueue.API.Helpers;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;
using mediQueue.API.Services;
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
        public async Task<IActionResult> GetAllUser()
        {
            var users = await userOperation.GetAllWithIncludesAsync(
                u => u.DoctorProfile,
                u => u.ReceptionistProfile
            );

            var responseDto = mapper.Map<ICollection<UserDTO.Response>>(users);
            return Ok(responseDto);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO.Create userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = mapper.Map<User>(userDto);

                user.PasswordHash = PasswordHelper.HashPassword(userDto.Password);

                await userOperation.AddAsync(user);
                await userOperation.SaveChangesAsync();

                var responseDto = mapper.Map<UserDTO.Response>(user);
                return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
                //Results = responseDto
            }
            catch (Exception ex)
            {
                // Catch unique constraint errors (duplicate email/phone)
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
            return Ok(new
            {
                message = "Login Successfull",
                result = result_,
                token = token
            });

        }


    }
}
