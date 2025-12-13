using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Venues;

namespace ReservationService.Infrastructure.Postgres.Reservations;

public class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.ToTable("reservation_seat");

        builder.HasKey(rs => rs.Id)
            .HasName("pk_reservation_seat");

        builder.Property(rs => rs.Id)
            .HasConversion(
                id => id.Value,
                value => new ReservationSeatId(value))
            .HasColumnName("id");

        builder.Property(rs => rs.ReservedAt)
            .HasColumnName("reserved_at");

        builder.HasOne(rs => rs.Reservation)
            .WithMany(r => r.ReservedSeats)
            .HasForeignKey(r => r.ReservationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_reservation_seat_reservation");

        builder.HasOne<Seat>()
            .WithMany()
            .HasForeignKey(s => s.SeatId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_reservation_seat_seat");

        builder.Property(rs => rs.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(r => r.SeatId)
            .HasColumnName("seat_id")
            .IsRequired();

        builder.Property(rs => rs.ReservationId)
            .HasColumnName("reservation_id");

        builder.HasIndex(rs => new { rs.EventId, rs.SeatId })
            .IsUnique();
    }
}