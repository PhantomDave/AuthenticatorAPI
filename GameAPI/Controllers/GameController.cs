using GameAPI.Dtos;
using GameAPI.DTOs;
using GameAPI.GameManagerD;
using GameAPI.Helpers;
using GameManagerD;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PokerLibrary.Enums;
using PokerLibrary.Models;

namespace GameAPI.Controllers
{
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly HttpHelper _httpHelper = new HttpHelper("https://localhost:7245");
        private readonly GameManager _gameManager;
        private readonly ScoreboardManager _scoreboardManager;

        private readonly MatchManager _matchManager;

        public GameController(
            GameManager gameManager,
            MatchManager matchManager,
            ScoreboardManager scoreboardManager
        )
        {
            _gameManager = gameManager;
            _matchManager = matchManager;
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

            Game? _game = _matchManager.GetGame(user.Email!) ?? new Game();
            _game.AddPlayerToGame(new Player(user.Email!, 0));
            _matchManager.AddGameToManager(_game, user.Email!);
            return Ok(userTok);
        }

        [HttpGet]
        [Route("/game/setusername/")]
        public async Task<IActionResult> SetUsername(
            [FromQuery] string token,
            [FromQuery] string email,
            [FromQuery] string username
        )
        {
            try
            {
                if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                    return Unauthorized();

                Game? _game = _matchManager.GetGame(email);
                if (_game.Players.Count == 0)
                    _game.AddPlayerToGame(new Player(email, 0));
                Player player = _game!.Players[0];
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
            [FromQuery] string email,
            int numplayer,
            int smallblind,
            int bigblind,
            int chips
        )
        {
            Game _game = _matchManager.GetGame(email)!;

            _game ??= new Game();

            if (_game.Started)
            {
                _matchManager.RemoveGame(email);
                _game = _game.ResetGame();
                _matchManager.AddGameToManager(_game, email);
            }

            if (numplayer > 5)
            {
                return BadRequest(JsonConvert.SerializeObject("Maximum Number of player is 5"));
            }

            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            _game.SetBlinds(smallblind, bigblind);
            _game.Players.First().SetChips(chips);
            _game.SetupAIPlayers(numplayer, chips);
            _game.StartGame();
            _game.AdvanceGame();
            return Ok(JsonConvert.SerializeObject(GenerateGameDTO(_game)));
        }

        [HttpGet]
        [Route("/game/getsavedgame")]
        public async Task<IActionResult> GetSavedGame([FromQuery] string email, [FromQuery] string token)
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            Game? _game = _gameManager.FindGame(email);
            if (_game is null) return Ok();
            if (!_game.Started)
                return Ok();
            if (_matchManager.GetGame(email) != null)
            {
                _matchManager.RemoveGame(email);
            }

            _matchManager.AddGameToManager(_game, email);

            return Ok(JsonConvert.SerializeObject(GenerateGameDTO(_game)));
        }

        [HttpGet]
        [Route("/game/clearSavedGame")]
        public async Task<IActionResult> ClearSavedGame([FromQuery] string email, [FromQuery] string token)
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            Game? _game = _gameManager.FindGame(email);
            if (_game is null) return Ok();

            if (_matchManager.GetGame(email) != null)
            {
                _matchManager.RemoveGame(email);
            }

            if (_gameManager.RemoveGameToList(email))
                return Ok();

            return BadRequest();
        }

        [HttpGet]
        [Route("/game/move/")]
        public ActionResult MakePlayerMove(
            [FromQuery] string email,
            [FromQuery] int move,
            [FromQuery] int bet
        )
        {
            try
            {
                Game _game = _matchManager.GetGame(email)!;
                //this returns the next player in the turn
                Player player = _game.Players[0];


                player.PrepareMove((NextMove)move, bet);

                if ((NextMove)move == NextMove.Bet)
                {
                    _game.AddToPot(bet);
                }

                // Player nextPlayer = _game.GetNextPlayerInTurn(player);

                return Ok(JsonConvert.SerializeObject(player));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/game/getaimove")]
        public ActionResult MakeAIMove([FromQuery] int id, [FromQuery] string email)
        {
            try
            {
                Game _game = _matchManager.GetGame(email)!;

                if (id >= _game.Players.Count)
                    return Ok();

                AIPlayer player = (AIPlayer)_game.Players[id];

                player.CalculateNextMove(_game);

                OpponentDTO opponent = OpponentDTO.GenerateStructFromClass(player);
                Console.WriteLine(opponent.Move);
                return Ok(JsonConvert.SerializeObject(opponent));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.StackTrace}");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/game/advance")]
        public ActionResult AdvanceGame([FromQuery] string email)
        {
            Game _game = _matchManager.GetGame(email)!;
            GameStage stage = _game.CurrentStage;
            _game.AdvanceGame();

            if (stage == _game.CurrentStage)
                return Ok();

            return Ok(JsonConvert.SerializeObject(GenerateGameDTO(_game)));
        }

        [HttpGet]
        [Route("/game/situp")]  
        public async Task<IActionResult> SitUpFromTable(
            [FromQuery] string token,
            [FromQuery] string email
        )
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            Game _game = _matchManager.GetGame(email)!;

            if (_gameManager.AddGameToList(_game))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/game/addtoScores")]
        public async Task<IActionResult> AddPlayerToScoreBoard(
            [FromQuery] string token,
            [FromQuery] string email
        )
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            Game _game = _matchManager.GetGame(email)!;

            Player player = _game.Players[0];
            _scoreboardManager.AddToList(
                new Scoreboard(player.Email, player.Chips, player.PlayersKnockedOut, player.TablesWon)
            );
            return Ok(JsonConvert.SerializeObject(ScoreboardManager.ScoreboardList));
        }

        [HttpGet]
        [Route("/game/getScores")]
        public async Task<IActionResult> GetScoreboard([FromQuery] string token)
        {
            if (!await _httpHelper.IsUserAuthenticatedAsync(token))
                return Unauthorized();

            var orderedDict = ScoreboardManager.ScoreboardList.OrderByDescending(s => s.ChipsWon).ToList();
            Console.WriteLine(JsonConvert.SerializeObject(orderedDict));
            return Ok(JsonConvert.SerializeObject(orderedDict));
        }

        [HttpGet]
        [Route("/game/getRoundWinner")]
        public ActionResult GetRoundWinner([FromQuery] string token,
            [FromQuery] string email)
        {
            Game _game = _matchManager.GetGame(email);
            if (_game is null)
                return NotFound();

            if (_game.LastRoundWinner is null)
                return NotFound();

            return Ok(JsonConvert.SerializeObject(_game.LastRoundWinner));
        }

        private GameDTO GenerateGameDTO(Game game)
        {
            List<OpponentDTO> opponents = new();
            for(int i = 1; i < game.Players?.Count; i++)
            {
                opponents.Add(OpponentDTO.GenerateStructFromClass(game.Players[i]));   
            }
            GameDTO dto = new GameDTO
            {
                Blinds = game.Blinds,
                Player = game.Players[0],
                Opponents = opponents,
                CurrentStage = game.CurrentStage,
                TableCards = game.TableCards,
                LastRoundWinner = game.LastRoundWinner,
                Pot = game.Pot,
            };
            return dto;
        }
    }
}
