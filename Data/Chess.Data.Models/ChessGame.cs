namespace Chess.Data.Models
{
    using Chess.Data.Common.Models;

    public class ChessGame : BaseModel<string>
    { 
        public int Status { get; set; }
  
    }
}
