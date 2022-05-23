using Chess.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Data.Models
{
    public class ChessGameAnalyzeResult : BaseModel<string>
    {
        public string GameId { get; set; }

        public string UserId { get; set; }
         
        public int GameMoveNo { get; set; }

        public string  Move  { get; set; }

        public string Type { get; set; }

        public string SuggestedMove { get; set; }   
        
        public int Depth { get; set; }
    }
}
