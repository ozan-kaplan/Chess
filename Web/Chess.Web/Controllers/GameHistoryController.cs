﻿using Chess.Common.Enums;
using Chess.Data;
using Chess.Data.Models;
using Chess.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chess.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class GameHistoryController : BaseController
    {
        private readonly UserManager<ChessUser> userManager;
        private readonly IServiceProvider serviceProvider;
        public GameHistoryController(UserManager<ChessUser> userManager, IServiceProvider serviceProvider)
        {
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> GetGameHistoryList()
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var draw = this.Request.Form["draw"].FirstOrDefault();
                var start = this.Request.Form["start"].FirstOrDefault();
                var length = this.Request.Form["length"].FirstOrDefault();
                var sortColumn = this.Request.Form["columns[" + this.Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = this.Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = this.Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();



                var userGameIdList = dbContext.ChessGamePlayers.Where(c => c.UserId == userId).Select(c => c.GameId).AsEnumerable();



                var chessGamePlayersQuery = from player in dbContext.ChessGamePlayers.AsEnumerable()
                                            where userGameIdList.Contains(player.GameId)
                                            group player by player.GameId into playerGroup
                                            orderby playerGroup.Key descending
                                            select new
                                            {
                                                Key = playerGroup.Key,
                                                Players = playerGroup.AsEnumerable(),
                                            };


                var chessGameQuery = from game in dbContext.ChessGames.AsEnumerable()
                                      join chessGamePlayers in chessGamePlayersQuery on game.Id equals chessGamePlayers.Key
                                      select new
                                      {
                                          Game = game,
                                          Players = chessGamePlayers.Players,
                                      };




                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    switch (sortColumn)
                    {
                        case "CreatedOn":
                            if (sortColumnDirection.ToLower() == "asc")
                            {
                                chessGameQuery = chessGameQuery.OrderBy(g => g.Game.CreatedOn);
                            }
                            else
                            {
                                chessGameQuery = chessGameQuery.OrderByDescending(g => g.Game.CreatedOn);
                            }
                            break;
                        default:
                            chessGameQuery = chessGameQuery.OrderByDescending(g => g.Game.CreatedOn);
                            break;
                    }
                }


                IEnumerable<GameHistoryViewModel> finalQuery = chessGameQuery.Select(c => new GameHistoryViewModel()
                {

                    Id = c.Game.Id,
                    CreatedOn = c.Game.CreatedOn.ToString("dd.MM.yyyy HH:mm:ss"),
                    GameStatus = c.Game.Status,
                    GameStatusText = ((ChessGameStatus)c.Game.Status).ToString(),
                    YourNickName = c.Players.FirstOrDefault(c => c.UserId == userId).NickName,
                    YourColor = c.Players.FirstOrDefault(c => c.UserId == userId).Color,
                    YourScore = c.Players.FirstOrDefault(c => c.UserId == userId).Score,
                    OpponentNickName = c.Players.FirstOrDefault(c => c.UserId != userId).NickName,
                    OpponentColor = c.Players.FirstOrDefault(c => c.UserId != userId).Color,
                    OpponentScore = c.Players.FirstOrDefault(c => c.UserId != userId).Score,
                });
                 

                if (!string.IsNullOrEmpty(searchValue))
                {
                    finalQuery = finalQuery.Where(m => m.YourNickName.ToLower().Contains(searchValue.ToLower())
                                                || m.OpponentNickName.ToLower().Contains(searchValue.ToLower()));
                }


                recordsTotal = finalQuery.Count();
                var data = finalQuery.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public IActionResult Detail(string gameId)
        {
            GameAnalyzeResultViewModel model = new GameAnalyzeResultViewModel();

            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);


                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();

                var player = dbContext.ChessGamePlayers.AsEnumerable().FirstOrDefault(p => p.GameId == gameId && p.UserId == userId) ;
                var opponentPlayer = dbContext.ChessGamePlayers.AsEnumerable().FirstOrDefault(p => p.GameId == gameId && p.UserId != userId );



                model.GameItem = new GameHistoryViewModel()
                {

                    YourNickName = player.NickName,
                    YourColor = player.Color,
                    YourScore = player.Score,
                    OpponentColor = opponentPlayer.Color,
                    OpponentNickName = opponentPlayer.NickName,
                    OpponentScore = opponentPlayer.Score,


                };

                model.GameQualityItem = dbContext.ChessGameAnalyzeQualityResults.AsEnumerable().Where(c => c.GameId == gameId)

                   .Select(c => new GameQuality()
                   { 

                       BlackBlunderCount  = c.BlackBlunderCount,
                       BlackInaccuraciesCount = c.BlackInaccuraciesCount,
                       BlackMistakeCount = c.BlackMistakeCount,
                       QualityOfBlackPlayer = c.QualityOfBlackPlayer,
                       QualityOfWhitePlayer = c.QualityOfWhitePlayer,
                       WhiteBlunderCount = c.WhiteBlunderCount,
                       WhiteInaccuraciesCount = c.WhiteInaccuraciesCount,
                       WhiteMistakeCount = c.WhiteMistakeCount,  

                   }).FirstOrDefault();



                model.GameMoveList = dbContext.ChessGameMoves.OrderBy(g => g.MoveNo).Where(g => g.GameId == gameId).ToList().Select(c => new MoveViewModel()
                { 
                     Move = c.Move,
                     MoveNo = c.MoveNo,
                     Color = c.UserId == player.UserId ? player.Color : opponentPlayer.Color,  
                }).ToList(); 


                model.GameAnalyzeList = dbContext.ChessGameAnalyzeResults.AsEnumerable().OrderBy(g => g.GameMoveNo).Where(c => c.GameId == gameId && c.UserId == userId)

                      .Select(c => new GameAnalyze()
                      {
                          GameMoveNumber = c.GameMoveNo,
                          SuggestedMove = c.SuggestedMove, 
                          Type = c.Type,
                          YourMove = c.Move,
                      }).ToList();


            }
            catch (Exception ex)
            {


            }
            return this.View(model);
        }
    }
}
