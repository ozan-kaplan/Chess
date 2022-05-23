namespace Chess.Data.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Chess.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ChessGameConfiguration : IEntityTypeConfiguration<ChessGame>
    {
        public void Configure(EntityTypeBuilder<ChessGame> builder)
        {
            builder
                .ToTable("chessgames"); 
        }
    }
}
