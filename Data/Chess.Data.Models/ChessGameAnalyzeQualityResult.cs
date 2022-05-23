using Chess.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Data.Models
{
    public class ChessGameAnalyzeQualityResult : BaseModel<string>
    {
        public string GameId { get; set; }

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
