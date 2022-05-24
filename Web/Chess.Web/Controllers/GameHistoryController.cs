using Chess.Common.Enums;
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


                var chessGameQuery = (from game in dbContext.ChessGames.AsEnumerable()
                                      join chessGamePlayers in chessGamePlayersQuery on game.Id equals chessGamePlayers.Key
                                      select new   {
                                          Game = game,
                                          Players = chessGamePlayers.Players
                                      });


                 
                 


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





                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    switch (sortColumn)
                    {
                        case "CreatedOn":
                            if (sortColumnDirection.ToLower() == "asc")
                            {
                                finalQuery = finalQuery.OrderBy(g => g.CreatedOn);
                            }
                            else
                            {
                                finalQuery = finalQuery.OrderByDescending(g => g.CreatedOn);
                            }
                            break;
                        default:
                            finalQuery = finalQuery.OrderByDescending(g => g.CreatedOn);
                            break;
                    }
                }




                //var customerData = (from tempcustomer in context.Customers select tempcustomer);
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                //}

                if (!string.IsNullOrEmpty(searchValue))
                {
                    finalQuery = finalQuery.Where(m => m.YourNickName.Contains(searchValue)
                                                || m.OpponentNickName.Contains(searchValue));
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
    }
}
