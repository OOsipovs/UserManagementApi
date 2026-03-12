using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Application.DTOs;
using UserManagementApi.Application.Interfaces;

namespace UserManagementApi.API.Controllers
{
    /// <summary>
    /// Manages users and their profile information.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a user with their profile by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user with profile information, or 404 if not found.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return (user == null) ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Creates a new user with a profile.
        /// </summary>
        /// <param name="request">The user creation request payload.</param>
        /// <returns>The newly created user with their assigned ID.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]CreateUserRequest request)
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
    }
}
