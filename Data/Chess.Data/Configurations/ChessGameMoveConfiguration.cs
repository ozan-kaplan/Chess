using Chess.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Data.Configurations
{
    public class ChessGameMoveConfiguration : IEntityTypeConfiguration<ChessGameMove>
    {
        public void Configure(EntityTypeBuilder<ChessGameMove> builder)
        {
            builder
            .ToTable("chessgamemoves");
        }
    }
}
