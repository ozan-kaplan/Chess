using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Web.ViewModels
{
    public class GameAnalyzeResultViewModel
    {
        public GameAnalyzeResultViewModel()
        {
            GameItem = new GameHistoryViewModel();
            GameAnalyzeList = new List<GameAnalyze>();
            GameQualityItem = new GameQuality();
            GameMoveList = new List<MoveViewModel>();
        }
        public GameHistoryViewModel GameItem { get; set; }

        public List<GameAnalyze> GameAnalyzeList { get; set; }

        public GameQuality GameQualityItem { get; set; }

        public List<MoveViewModel> GameMoveList { get; set; }

    }


    public class MoveViewModel
    {
        public int MoveNo { get; set; }

        public string Color { get; set; } 
      
        public string Move { get; set; } 

    }

    public class GameAnalyze
    {
        public int GameMoveNumber { get; set; }

        public string Color { get; set; }

        public string Type { get; set; }

        public string YourMove { get; set; }

        public string SuggestedMove { get; set; }

    }

    public class GameQuality
    {
        public decimal QualityOfWhitePlayer { get; set; }

        public decimal QualityOfBlackPlayer { get; set; }

        public int WhiteMistakeCount { get; set; }
        public int WhiteInaccuraciesCount { get; set; }
        public int WhiteBlunderCount { get; set; }

        public int BlackMistakeCount { get; set; }
        public int BlackInaccuraciesCount { get; set; }
        public int BlackBlunderCount { get; set; }

    }
}
