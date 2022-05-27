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


                var statItem = dbContext.Stats.AsEnumerable().Where(c => c.UserId == userId) .FirstOrDefault();
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

    }
}
