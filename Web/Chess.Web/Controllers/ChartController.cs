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
    public class ChartController : BaseController
    {
        private readonly UserManager<ChessUser> userManager;
        private readonly IServiceProvider serviceProvider;

        public ChartController(UserManager<ChessUser> userManager, IServiceProvider serviceProvider)
        {
            this.userManager = userManager;
            this.serviceProvider = serviceProvider;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetGameQualityDataForLineChart()
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);


                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();
                DateTime startDate = DateTime.Now.AddYears(-1);

                List<LineChartViewModel<DateTime, decimal>> lineChartData = new List<LineChartViewModel<DateTime, decimal>>();

                var userGameList = (from game in dbContext.ChessGames.AsEnumerable()
                                    join player in dbContext.ChessGamePlayers.AsEnumerable() on game.Id equals player.GameId
                                    where player.UserId == userId && game.CreatedOn >= startDate
                                    orderby game.CreatedOn ascending
                                    select new { Game = game, Player = player }).ToList();





                if (userGameList != null)
                {

                    var gameIdList = userGameList.Select(c => c.Game.Id);

                    var gameQualityList = dbContext.ChessGameAnalyzeQualityResults
                     .Where(c => gameIdList.Contains(c.GameId))
                     .Select(q => q)
                     .ToList();


                    if (gameQualityList != null && gameQualityList.Any())
                    {
                        foreach (var item in userGameList)
                        {
                            var gameQualityItem = gameQualityList.FirstOrDefault(c => c.GameId == item.Game.Id);
                            if (gameQualityItem != null)
                            {
                                LineChartViewModel<DateTime, decimal> chartItem = new LineChartViewModel<DateTime, decimal>()
                                {
                                    x = item.Game.CreatedOn,
                                    y = item.Player.Color.ToLower() == "white" ? gameQualityItem.QualityOfWhitePlayer : gameQualityItem.QualityOfBlackPlayer,

                                };

                                lineChartData.Add(chartItem);

                            }
                        }
                    }

                }

                return Ok(lineChartData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetGameStatisticForPieChart()
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);


                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();
                DateTime startDate = DateTime.Now.AddYears(-1);

                List<LineChartViewModel<string, int>> chartData = new List<LineChartViewModel<string, int>>();


                var statItem = dbContext.Stats.AsEnumerable().Where(c => c.UserId == userId).FirstOrDefault();
                if (statItem != null)
                {
                    chartData.Add(new LineChartViewModel<string, int>()
                    {
                        label = "Win",
                        y = statItem.Win,
                    });

                    chartData.Add(new LineChartViewModel<string, int>()
                    {
                        label = "Draw",
                        y = statItem.Draw,
                    });

                    chartData.Add(new LineChartViewModel<string, int>()
                    {
                        label = "Loss",
                        y = statItem.Loss,
                    });

                }


                return Ok(chartData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [HttpPost]
        public async Task<IActionResult> GetGameQualityDataForBarChart()
        {
            try
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);


                using var scope = this.serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDbContext>();
                DateTime startDate = DateTime.Now.AddYears(-1);

                List<BarChartViewModel<decimal>> lineChartData = new List<BarChartViewModel<decimal>>();




                var userGameList = (from game in dbContext.ChessGames.AsEnumerable()
                                    join player in dbContext.ChessGamePlayers.AsEnumerable() on game.Id equals player.GameId
                                    where player.UserId == userId && game.CreatedOn >= startDate
                                    orderby game.CreatedOn ascending
                                    select new { Game = game, Player = player }).ToList();




                if (userGameList != null)
                {

                    var gameIdList = userGameList.Select(c => c.Game.Id);

                    var gameQualityList = dbContext.ChessGameAnalyzeQualityResults
                     .Where(c => gameIdList.Contains(c.GameId))
                     .Select(q => q)
                     .ToList();


                    var opponentGameList = (from player in dbContext.ChessGamePlayers.AsEnumerable()
                                            where gameIdList.Contains(player.GameId) && player.UserId != userId
                                            select player).ToList();


                    if (gameQualityList != null && gameQualityList.Any())
                    {
                        foreach (var item in userGameList)
                        {
                            var gameQualityItem = gameQualityList.FirstOrDefault(c => c.GameId == item.Game.Id);
                            if (gameQualityItem != null)
                            {


                                var opponentPlayer = opponentGameList.FirstOrDefault(o => o.GameId == item.Game.Id);


                                BarChartViewModel<decimal> chartItem = new BarChartViewModel<decimal>()
                                {
                                    id = item.Game.Id,
                                    label = $"Date:{item.Game.CreatedOn.ToString("dd.MM.yyyy HH:mm:ss")}",

                                    extensiondata = $"Date:{item.Game.CreatedOn.ToString("dd.MM.yyyy HH:mm:ss")}" +
                                    $"<br/>Your Color:{item.Player.Color.ToUpper()}" +
                                    $"<br/>Opponent Color:{(opponentPlayer != null ? opponentPlayer.Color : string.Empty)}" +
                                    $"<br/>Your Score:{item.Player.Score}" +
                                    $"<br/>Opponent Score:{(opponentPlayer != null ? opponentPlayer.Score.ToString() : string.Empty)}",
                                    y = item.Player.Color.ToLower() == "white" ? gameQualityItem.QualityOfWhitePlayer : gameQualityItem.QualityOfBlackPlayer,

                                };

                                lineChartData.Add(chartItem);

                            }
                        }
                    }

                }

                return Ok(lineChartData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
