using Microsoft.AspNetCore.Mvc;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Repositories;
using generar_ticket.Users.Interface.REST.Resources;
using generar_ticket.Users.Interface.REST.Transform;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using generar_ticket.Users.Application.Internal.CommandServices;
using generar_ticket.Users.Application.Internal.OutboundServices;
using generar_ticket.Users.Domain.Model.Command;
using generar_ticket.Users.Domain.Services;
using generar_ticket.Users.Infrastructure.Tokens.Services;
using Microsoft.AspNetCore.Authorization;

namespace generar_ticket.Users.Interface.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly IUserCommandService _userCommandService;
        private readonly ITokenService _tokenService;

        public UserController(IUserRepository userRepository, AppDbContext context, IUserCommandService userCommandService, ITokenService tokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userCommandService = userCommandService ?? throw new ArgumentNullException(nameof(userCommandService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserResource>> CreateUser([FromBody] CreateUserResource resource)
        {
            if (resource == null) return BadRequest("Invalid user data.");

            var command = CreateUserCommandFromResourceAssembler.ToCommand(resource);
            var user = new User(command); // Assuming User has a constructor that accepts the command

            await _userRepository.AddAsync(user); // Ensure AddAsync method exists in IUserRepository
            await _userRepository.SaveChangesAsync(); // Ensure SaveChangesAsync method exists in IUserRepository

            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userResource);
        }
        
        
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInResource resource)
        {
            if (resource == null) return BadRequest("Invalid sign-in data.");

            var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
            var authenticatedUser = await _userCommandService.Handle(signInCommand);
            if (authenticatedUser.user == null) return Unauthorized();

            var token = _tokenService.GenerateToken(authenticatedUser.user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.StoreRefreshToken(refreshToken, authenticatedUser.user);

            var authenticatedUserResource = new AuthenticatedUserResource(
                authenticatedUser.user.Id,
                token,
                refreshToken
            );

            return Ok(authenticatedUserResource);
        }
        
        [HttpPost("sign-out")]
        public IActionResult SignOut()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _tokenService.InvalidateToken(token);

            return Ok(new { message = "Session closed successfully." });
        }
        



        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResource>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
            return Ok(userResource);
        }
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(userResources);
        }
        
        [Authorize]
        [HttpGet("area/{area}")]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetUsersByArea(string area)
        {
            var users = await _userRepository.GetUsersByAreaAsync(area);
            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "No users found in the specified area." });
            }

            var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(userResources);
        }

       
        [Authorize]
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetUsersByRole(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "No users found with the specified role." });
            }

            var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(userResources);
        }
        
        
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserResource resource)
        {
            if (resource == null) return BadRequest("Invalid user data.");

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var command = new UpdateUserCommand(id,  resource.Ventanilla, resource.Password, resource.Rol, resource.Area);
            user.Update(command);
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

 
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepository.DeleteAsync(user);
            return NoContent();
        }
        
        [Authorize]
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> UpdateUserEstado(int id, [FromBody] UpdateUserEstadoResource resource)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Estado = resource.Estado; // Update the estado field
            await _userRepository.UpdateAsync(user); // Ensure UpdateAsync method exists in IUserRepository

            return NoContent();
        }
        


    }
}