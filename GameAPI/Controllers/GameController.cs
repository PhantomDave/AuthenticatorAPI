using System.Runtime.CompilerServices;
using System.Text.Json;
using GameAPI.DTOs;
using GameAPI.GameManagerD;
using GameAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PokerLibrary.Enums;
using PokerLibrary.Interfaces;
using PokerLibrary.Models;

namespace GameAPI.Controllers
{
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly HttpHelper _httpHelper = new HttpHelper("https://localhost:7245");
        private readonly GameManager _gameManager;
        private readonly ScoreboardManager _scoreboardManager;
        private readonly IGame _game;

        public GameController(
            GameManager gameManager,
            IGame game,
            ScoreboardManager scoreboardManager
        )
        {
            _gameManager = gameManager;
            _game = game;
            _scoreboardManager = scoreboardManager;
        }

        [HttpGet]
        [Route("/login/{email}")]
        public async Task<IActionResult> GetUserNonce(string email)
        {
            string nonce = await _httpHelper.ValidateUserEmailAsync(email);
            return Ok(JsonConvert.SerializeObject(nonce));
        }

        [HttpPost]
        [Route("/login/")]
        public async Task<IActionResult> LoginUser([FromBody] UserDto user)
        {
            string userTok = await _httpHelper.LoginUserAsync(user.Email!, user.Password!);
            if (string.IsNullOrEmpty(userTok))
            {
                return NotFound(JsonConvert.SerializeObject("Invalid Credentials"));
            }

            _game.AddPlayerToGame(new Player(user.Email!, 0));
            return Ok(userTok);
        }

        [HttpGet]
        [Route("/game/setusername/")]
        public async Task<IActionResult> SetUsername(
            [FromQuery] string token,
            [FromQuery] string username
        )
        {
            try
            {
                if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                    return Unauthorized();

                Player player = _game.Players[0];
                player.SetUsername(username);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/game/start/{numplayer}/{smallblind}/{bigblind}/{chips}")]
        public async Task<IActionResult> StartGame(
            [FromQuery] string token,
            int numplayer,
            int smallblind,
            int bigblind,
            int chips
        )
        {
            if (_game.Started)
                return BadRequest("Game has started");

            if (numplayer > 5)
            {
                return BadRequest(JsonConvert.SerializeObject("Maxinum Number of player is 5"));
            }

            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            _game.SetBlinds(smallblind, bigblind);
            _game.Players.First().SetChips(chips);
            _game.SetupAIPlayers(numplayer, chips);
            _game.StartGame();
            return Ok(JsonConvert.SerializeObject(GenerateDTO()));
        }

        [HttpGet]
        [Route("/game/move/")]
        public ActionResult MakePlayerMove(
            [FromQuery] int move,
            [FromQuery] int id,
            [FromQuery] int bet
        )
        {
            try
            {
                //this returns the next player in the turn
                Player player = _game.Players[id];

                player.PrepareMove((NextMove)move, bet);

                Player nextPlayer = _game.GetNextPlayerInTurn(player);

                return Ok(JsonConvert.SerializeObject(nextPlayer));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/game/advance")]
        public ActionResult AdvanceGame()
        {
            GameStage stage = _game.CurrentStage;
            _game.AdvanceGame();

            if (stage == _game.CurrentStage)
                return BadRequest();

            return Ok(JsonConvert.SerializeObject(GenerateDTO()));
        }

        [HttpGet]
        [Route("/game/situp")]
        public async Task<IActionResult> SitUpFromTable([FromQuery] string token)
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            if (_gameManager.AddGameToList(GenerateDTO()))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/game/")]
        public async Task<IActionResult> AddPlayerToScoreBoard(
            [FromQuery] string token,
            [FromQuery] int chipsWon,
            [FromQuery] int playerKnocked,
            [FromQuery] int tableKnocked
        )
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            Player player = _game.Players[0];
            ScoreboardManager.AddToList(
                new Scoreboard(player.Email, chipsWon, playerKnocked, tableKnocked)
            );
            return Ok(JsonConvert.SerializeObject(ScoreboardManager.ScoreboardList));
        }

        private GameDTO GenerateDTO()
        {
            GameDTO dto =
                new()
                {
                    Pot = _game.Pot,
                    Deck = _game.Deck,
                    Players = _game.Players,
                    CurrentStage = _game.CurrentStage,
                    Blinds = _game.Blinds!,
                    TableCards = _game.TableCards
                };
            return dto;
        }
    }
}
