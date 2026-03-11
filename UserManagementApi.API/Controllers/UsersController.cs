using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Interfaces;

namespace UserManagementApi.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return (user == null) ? NotFound() : Ok(user);
        }

        public async Task<IActionResult> Create([FromBody]CreateUserRequest request)
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
    }
}
