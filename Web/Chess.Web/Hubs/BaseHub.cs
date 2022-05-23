namespace Chess.Web.Hubs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Chess.Common.Enums;
    using Chess.Data;
    using Chess.Data.Models;
    using Chess.Services.Data.Contracts;
    using Chess.Services.Data.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;

    public partial class GameHub : Hub
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INotificationService notificationService;
        private readonly ICheckService checkService;
        private readonly IDrawService drawService;
        private readonly IUtilityService utilityService;

        private readonly ConcurrentDictionary<string, Player> players;
        private readonly ConcurrentDictionary<string, Game> games;
        private readonly List<Player> waitingPlayers;

        private IHostingEnvironment _env;
     

        public GameHub(
            IServiceProvider serviceProvider,
            INotificationService notificationService,
            ICheckService checkService,
            IDrawService drawService,
            IUtilityService utilityService,
            IHostingEnvironment env
          )
        {
            this.serviceProvider = serviceProvider;
            this.notificationService = notificationService;
            this.checkService = checkService;
            this.drawService = drawService;
            this.utilityService = utilityService;
            this._env = env;

            this.players = new ConcurrentDictionary<string, Player>(StringComparer.OrdinalIgnoreCase);
            this.games = new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);
            this.waitingPlayers = new List<Player>();

            this.notificationService.OnGameOver += this.Game_OnGameOver;
            this.notificationService.OnTakePiece += this.Game_OnTakePiece;
            this.notificationService.OnAvailableThreefoldDraw += this.Game_OnAvailableThreefoldDraw;
            this.notificationService.OnCompleteMove += this.Game_OnCompleteMove;
            this.notificationService.OnMoveEvent += this.Game_OnMoveEvent;
        }

        public override async Task OnConnectedAsync()
        {
            await this.LobbySendInternalMessage(this.Context.User.Identity.Name);
            await this.Clients.Caller.SendAsync("ListRooms", this.waitingPlayers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (this.players.Keys.Contains(this.Context.ConnectionId))
            {
                var leavingPlayer = this.players[this.Context.ConnectionId];

                if (leavingPlayer.GameId != null)
                {
                    var game = this.games[leavingPlayer.GameId];

                    if (game.GameOver == GameOver.None)
                    {
                        await this.GameSendInternalMessage(game.Id, leavingPlayer.Name, null);
                        await this.Clients.Group(leavingPlayer.GameId).SendAsync("GameOver", leavingPlayer, GameOver.Disconnected);

                        if (game.Turn > 30)
                        {
                            var winner = game.MovingPlayer.Id != leavingPlayer.Id ? game.MovingPlayer : game.Opponent;
                            if (this.players.Keys.Contains(winner.Id))
                            {
                                this.UpdateStats(winner, leavingPlayer, game, GameOver.Disconnected);
                            }
                        }
                    }
                }
                else
                {
                    this.waitingPlayers.Remove(leavingPlayer);
                    await this.Clients.All.SendAsync("ListRooms", this.waitingPlayers);
                }

                this.players.TryRemove(leavingPlayer.Id, out _);
            }
        }

        private Player GetPlayer()
        {
            return this.players[this.Context.ConnectionId];
        }

        private Player GetOpponentPlayer(Game game, Player player)
        {
            return game.MovingPlayer.Id != player.Id ? game.MovingPlayer : game.Opponent;
        }

        private Game GetGame(Player player)
        {
            return this.games[player.GameId];
        }

        private int GetUserRating(Player player)
        {
            using var scope = this.serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();

            return dbContext.Stats.Where(x => x.User.Id == player.UserId).Select(x => x.EloRating).FirstOrDefault();
        }

        private void UpdateStats(Player sender, Player opponent, Game game, GameOver gameOver)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();


                if (gameOver == GameOver.Checkmate || gameOver == GameOver.Resign || gameOver == GameOver.Disconnected)
                {
                    var chessGamePlayerItem = dbContext.ChessGamePlayers.FirstOrDefault(p => p.GameId == game.Id && p.UserId == sender.UserId);
                    if (chessGamePlayerItem != null)
                    {
                        chessGamePlayerItem.Score++;
                        dbContext.SaveChanges();
                    }
                }

                var senderStats = dbContext.Stats.Where(x => x.User.Id == sender.UserId).FirstOrDefault();
                var opponentStats = dbContext.Stats.Where(x => x.User.Id == opponent.UserId).FirstOrDefault();

                if (senderStats == null)
                {
                    senderStats = new Stats
                    {
                        Games = 0,
                        Win = 0,
                        Draw = 0,
                        Loss = 0,
                        UserId = sender.UserId,
                        EloRating = 1200,
                    };

                    dbContext.Stats.Add(senderStats);
                    dbContext.SaveChanges();
                }

                if (opponentStats == null)
                {

                    opponentStats = new Stats
                    {
                        Games = 0,
                        Win = 0,
                        Draw = 0,
                        Loss = 0,
                        UserId = opponent.UserId,
                        EloRating = 1200,
                    };

                    dbContext.Stats.Add(opponentStats);
                    dbContext.SaveChanges();
                }


                senderStats.Games += 1;
                opponentStats.Games += 1;

                if (gameOver == GameOver.Checkmate || gameOver == GameOver.Resign || gameOver == GameOver.Disconnected)
                {
                    var utilityService = this.serviceProvider.GetRequiredService<IUtilityService>();
                    int points = utilityService.CalculateRatingPoints(senderStats.EloRating, opponentStats.EloRating);

                    senderStats.Win += 1;
                    opponentStats.Loss += 1;
                    senderStats.EloRating += points;
                    opponentStats.EloRating -= points;
                }
                else if (gameOver == GameOver.Stalemate || gameOver == GameOver.Draw || gameOver == GameOver.ThreefoldDraw || gameOver == GameOver.FivefoldDraw)
                {
                    senderStats.Draw += 1;
                    opponentStats.Draw += 1;
                }

                dbContext.Stats.Update(senderStats);
                dbContext.Stats.Update(opponentStats);
                dbContext.SaveChanges();


                ChessGame chessGame = dbContext.ChessGames.FirstOrDefault(c => c.Id == game.Id);

                if (chessGame != null)
                {
                    chessGame.Status = (int)ChessGameStatus.AnalysisWaiting;
                    chessGame.ModifiedOn = DateTime.Now; 
                    dbContext.SaveChanges();   
                    this.GeneratePgnFileOfGame(chessGame);

                }

            }
            catch
            {

            }


        }


        private void AddChessGame(Game game, Player player1, Player player2)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();

                ChessGame chessGame = new ChessGame()
                {
                    Id = game.Id,
                    CreatedOn = DateTime.Now,
                    Status = (int)ChessGameStatus.Started,
                };

                dbContext.ChessGames.Add(chessGame);


                ChessGamePlayer chessGamePlayer = new ChessGamePlayer()
                {
                    Id = Guid.NewGuid().ToString(),
                    GameId = game.Id,
                    UserId = player1.UserId,
                    NickName = player1.Name,
                    Color = player1.Color.ToString(),
                    Score = 0,
                    CreatedOn = DateTime.Now,
                };
                dbContext.ChessGamePlayers.Add(chessGamePlayer);


                ChessGamePlayer chessGamePlayer2 = new ChessGamePlayer()
                {
                    Id = Guid.NewGuid().ToString(),
                    GameId = game.Id,
                    UserId = player2.UserId,
                    NickName = player2.Name,
                    Color = player2.Color.ToString(),
                    Score = 0,
                    CreatedOn = DateTime.Now,
                };
                dbContext.ChessGamePlayers.Add(chessGamePlayer2);


                dbContext.SaveChanges();


            }
            catch
            {

            }
        }


        private void AddChessGameMove(Game game, string userId, string notation)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();

                if (!string.IsNullOrEmpty(notation))
                {

                    var notationArr = notation.Split(' ');
                    if (notationArr != null && notationArr.Length == 2)
                    {
                        int moveNo = 0;

                        var moveNoArr = notationArr[0].ToString().Split('.');
                        if (moveNoArr != null)
                        {
                            int.TryParse(moveNoArr[0].ToString(), out moveNo);
                            ChessGameMove chessGameMove = new ChessGameMove()
                            {
                                Id = Guid.NewGuid().ToString(),
                                GameId = game.Id,
                                UserId = userId,
                                MoveNo = moveNo,
                                Move = notationArr[1].ToString(),
                                CreatedOn = DateTime.Now,
                            };
                            dbContext.ChessGameMoves.Add(chessGameMove);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch
            {

            }
        }


        private void GeneratePgnFileOfGame(ChessGame chessGame)
        {

            using var scope = this.serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();

            List<ChessGameMove> chessGameMoves = dbContext.ChessGameMoves.OrderBy(c => c.CreatedOn).Where(c => c.GameId == chessGame.Id).ToList();

            ChessGamePlayer whitePlayer = dbContext.ChessGamePlayers.Where(c => c.Color == Color.White.ToString() && c.GameId == chessGame.Id).FirstOrDefault();
            ChessGamePlayer blackPlayer = dbContext.ChessGamePlayers.Where(c => c.Color == Color.Black.ToString() && c.GameId == chessGame.Id).FirstOrDefault();

            if (whitePlayer != null && blackPlayer != null && chessGameMoves != null && chessGameMoves.Any())
            {
                string content = "[Event \"Chess Performance Evaulation App\"]";

                content += "\n[Site \"Chess Performance Evaulation App\"]";
                content += "\n[Date \"" + chessGame.CreatedOn.ToString("yyyy.MM.dd") + "\"]";
                content += "\n[Round \"1\"]";
                content += "\n[White \"" + whitePlayer.NickName + "\"]";
                content += "\n[Black \"" + blackPlayer.NickName + "\"]";
                content += "\n[Result \"" + whitePlayer.Score.ToString() + "-" + blackPlayer.Score.ToString() + "\"]";

                content += "\n";

                var moveGroups = chessGameMoves.GroupBy(p => p.MoveNo, p => p.Move, (key, g) => new { MoveNo = key, Moves = g.ToList() });
                if (moveGroups != null && moveGroups.Any())
                {
                    foreach (var item in moveGroups)
                    {
                        content += item.MoveNo + ". " + string.Join(" ", item.Moves) + " ";
                    }
                }
                string path = Path.Combine(this._env.WebRootPath, "pgn-files", $"{chessGame.Id}.pgn");
                File.WriteAllText(path, content);
            }
        }
    }
}
