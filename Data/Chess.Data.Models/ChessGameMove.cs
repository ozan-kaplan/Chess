namespace Chess.Data.Models
{
    using Chess.Data.Common.Models;

    public class ChessGameMove : BaseModel<string>
    {
        public string GameId { get; set; }

        public string UserId { get; set; }
        public int MoveNo { get; set; }
        public string Move { get; set; }
    }
}
