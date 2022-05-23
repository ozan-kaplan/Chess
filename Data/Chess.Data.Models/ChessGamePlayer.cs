using Chess.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Data.Models
{
    public class ChessGamePlayer : BaseModel<string>
    {
        public string GameId { get; set; }
        public string UserId { get; set; }
        public string NickName { get; set; }
        public string Color { get; set; }
        public int Score { get; set; }

    } 
}
