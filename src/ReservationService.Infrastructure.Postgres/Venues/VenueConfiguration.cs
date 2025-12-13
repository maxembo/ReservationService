using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Shared;
using ReservationService.Domain.Venues;

namespace ReservationService.Infrastructure.Postgres.Venues;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");

        builder.HasKey(v => v.Id)
            .HasName("pk_venues");

        builder.Property(v => v.Id)
            .HasConversion(
                id => id.Value,
                value => new VenueId(value))
            .HasColumnName("id");

        builder.ComplexProperty(
            v => v.VenueName, cpb =>
            {
                cpb.Property(v => v.Prefix)
                    .HasMaxLength(Constants.MAX_PREFIX_LENGTH)
                    .IsRequired()
                    .HasColumnName("prefix");

                cpb.Property(v => v.Name)
                    .HasMaxLength(Constants.MAX_NAME_LENGTH)
                    .IsRequired()
                    .HasColumnName("name");
            });

        builder.HasMany(v => v.Seats)
            .WithOne(s => s.Venue)
            .HasForeignKey(s => s.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(v => v.SeatsLimit)
            .HasMaxLength(Constants.MAX_SEATS_LIMIT)
            .HasColumnName("seats_limit");

        // 1 вариант
        // builder.HasMany(v => v.Seats)
        //     .WithOne()
        //     .HasForeignKey(s => s.VenueId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);

        // 2 вариант
        // builder.HasMany<Seat>()
        //     .WithOne()
        //     .HasForeignKey(s => s.VenueId)
        //     .IsRequired()
        //     .OnDelete(DeleteBehavior.Cascade);
        //2 вариант
        // builder.OwnsOne(v => v.VenueName, ownb =>
        // {
        //     ownb.Property(v => v.Prefix)
        //         .HasMaxLength(Constants.MaxPrefixLength)
        //         .HasColumnName("prefix");
        //     
        //     ownb.Property(v => v.Name)
        //         .HasMaxLength(Constants.MaxNameLength)
        //         .HasColumnName("name");
        // });
        //
        // builder.Navigation(v => v.VenueName).IsRequired(false);
    }
}