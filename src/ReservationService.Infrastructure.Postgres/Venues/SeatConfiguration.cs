using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Shared;
using ReservationService.Domain.Venues;

namespace ReservationService.Infrastructure.Postgres.Venues;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");

        builder.HasKey(s => s.Id)
            .HasName("pk_seats");

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SeatId(value))
            .HasColumnName("id");

        builder.Property(s => s.RowNumber)
            .HasMaxLength(Constants.MAX_ROW_NUMBER)
            .HasColumnName("row_number");

        builder.Property(s => s.SeatNumber)
            .HasMaxLength(Constants.MAX_SEAT_NUMBER)
            .HasColumnName("seat_number");

        builder.Property(s => s.VenueId)
            .HasConversion(id => id.Value, value => new VenueId(value))
            .HasColumnName("venue_id");

        builder.HasIndex(s => new { s.VenueId, s.RowNumber, s.SeatNumber });
        // builder.HasIndex(s => new { s.VenueId, s.RowNumber, s.SeatNumber })
        //     .HasFilter("row_number > 0 AND seat_number > 0");
    }
}