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
            var authenticatedUserResource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(authenticatedUser.user, token);
            return Ok(authenticatedUserResource);
        }
        
        


        [HttpGet("{id}")]
        public async Task<ActionResult<UserResource>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
            return Ok(userResource);
        }

        
  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(userResources);
        }
        
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
        
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserResource resource)
        {
            if (resource == null || id != resource.Id) return BadRequest("Invalid user data.");

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.Update(resource); // Assuming User has an Update method that accepts the resource
            await _userRepository.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepository.DeleteAsync(user);
            return NoContent();
        }


    }
}