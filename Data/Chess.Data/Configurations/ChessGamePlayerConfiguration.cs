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
    class ChessGamePlayerConfiguration : IEntityTypeConfiguration<ChessGamePlayer>
    {
        public void Configure(EntityTypeBuilder<ChessGamePlayer> builder)
        {
            builder
            .ToTable("chessgameplayers");
        }
    }
}
