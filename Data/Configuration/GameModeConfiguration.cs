using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class GameModeConfiguration : IEntityTypeConfiguration<GameMode>
{
    public void Configure(EntityTypeBuilder<GameMode> builder)
    {
        builder.ToTable("game_modes");

        builder.HasKey(x => x.ModeKey);
    }
}