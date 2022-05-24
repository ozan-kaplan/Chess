using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Web.ViewModels
{
    public class GameHistoryViewModel
    {

        public string Id { get; set; } 
        public string CreatedOn { get; set; }
        public string YourNickName { get; set; }
        public string YourColor { get; set; }
        public int YourScore { get; set; }
        public string OpponentNickName { get; set; }
        public string OpponentColor { get; set; }
        public int OpponentScore { get; set; }
        public int GameStatus { get; set; }
        public string GameStatusText { get; set; }

    }
}
