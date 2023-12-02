using AuthenticatorAPI.Data;
using AuthenticatorAPI.DTOs;
using AuthenticatorAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticatorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly UserManager _userManager;

    public AuthController(ILogger<AuthController> logger, UserManager userManager)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    [Route("/login/{email}")]
    public ActionResult UserExsists(string email)
    {
        User? user = _userManager.GetById(email);
        if (user is null)
        {
                //I Generate a fake nonce to avoid exposing users
                Random rnd = new Random(DateTime.Now.Millisecond);
                int random = rnd.Next(0, Int32.MaxValue);
        }

        return Ok(user.GenerateUserNonce());
    }

    [HttpPost]
    [Route("/login/")]
    public ActionResult CheckUserLogin([FromBody]UserDto userDto)
    {
        User? user = _userManager.GetById(userDto.Email);
        if (user is null)
        {
            return NotFound("Invalid Credentials");
        }

        if (user.CheckUserCredentials(user, userDto.Password))
        {
            return Ok(user);
        }
        return NotFound("Invalid Credentials");
        
    }

    [HttpPost]
    [Route("/register/")]
    public ActionResult RegisterUser([FromBody] RegistrationDto registrationDto)
    {
        User user = new User(registrationDto.Username!, registrationDto.Email!, registrationDto.Password!);
        user.HashOwnPassword();
        if (_userManager.AddToList(user))
        {
            return Ok("User Created");
        }

        return BadRequest("Email Already Present");
    }

    [HttpGet]
    [Route("/checkauth/")]
    public ActionResult CheckUserAuthentication([FromQuery] string token)
    {

        try
        {
            Token? tok = Token.GetUserTupleFromToken(token);

            if (tok is null)
                return Unauthorized();
            
            if (tok.IsUserTokenValid(tok.User))
                return Ok();
            return Unauthorized();
        
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized("Unauthorized from Catch");
        }
        /*User? user = Token.GetUserFromToken(token);
        if (user is null)
            return Unauthorized();
        Token? userToken = Token.GetTokenFromUser(user);

        if (userToken is null)
            return Unauthorized();

        if (!userToken.IsUserTokenValid(user))
            return Unauthorized();

        return Ok();*/

    }
    
    
}