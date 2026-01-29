using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Events;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Users;

namespace ReservationService.Infrastructure.Postgres.Reservations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.Id)
            .HasName("pk_reservations");

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new ReservationId(value))
            .HasColumnName("id");

        builder.Property(r => r.EventId)
            .HasConversion(
                id => id.Value,
                value => new EventId(value))
            .HasColumnName("eventId");

        builder.Property(r => r.ReservationStatus)
            .HasColumnName("status");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(r => r.EventId)
            .HasColumnName("event_id");

        builder.Property(r => r.UserId)
            .HasColumnName("user_id");
    }
}