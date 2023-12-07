using System.Text.RegularExpressions;
using AuthenticatorAPI.DTOs;
using AuthenticatorAPI.Data;
using AuthenticatorAPI.Models;
using AuthenticatorAPI.Regexs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthenticatorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public partial class AuthController : ControllerBase
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
        User user = _userManager.GetById(email)!;
        if (user is null)
        {
            //I Generate a fake nonce to avoid exposing users
            Random rnd = new Random(DateTime.Now.Millisecond);
            int random = rnd.Next(0, int.MaxValue);
            return Ok(Models.User.ComputeSha256Hash(random.ToString()));
        }

        return Ok(user!.GenerateUserNonce());
    }

    [HttpPost]
    [Route("/login/")]
    public ActionResult CheckUserLogin([FromBody] UserDto userDto)
    {
        User user = _userManager.GetById(userDto.Email!)!;
        if (user is null)
        {
            return NotFound("Invalid Credentials");
        }

        Token? token = user.CheckUserCredentials(user, userDto.Password!);
        if (token != null)
        {
            dynamic dynamicSerialize = new { dtoken = token.TokenGuid, duser = user };

            return Ok(JsonConvert.SerializeObject(dynamicSerialize));
        }
        return NotFound("Invalid Credentials");
    }

    [HttpPost]
    [Route("/register/")]
    public ActionResult RegisterUser([FromBody] RegistrationDto registrationDto)
    {
        User user = new User(
            registrationDto.Username!,
            registrationDto.Email!,
            registrationDto.Password!
        );

        if (!Rgex.EmailRegex().IsMatch(user.Email))
        {
            return BadRequest("Invalid Email");
        }

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
            Token? tok = Token.GetTokenFromTokenId(token);

            if (tok is null)
                return Unauthorized();
            if (tok.IsUserTokenValid(tok))
                return Ok();
            return Unauthorized();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized("Unauthorized from Catch");
        }
    }
}
