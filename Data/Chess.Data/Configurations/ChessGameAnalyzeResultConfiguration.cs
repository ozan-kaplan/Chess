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
    class ChessGameAnalyzeResultConfiguration : IEntityTypeConfiguration<ChessGameAnalyzeResult>
    {
        public void Configure(EntityTypeBuilder<ChessGameAnalyzeResult> builder)
        {
            builder
            .ToTable("chessgameanalyzeresults");
        }
    }

}
