using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Events;
using ReservationService.Domain.Shared;

namespace ReservationService.Infrastructure.Postgres.Events;

public class EventDetailsConfiguration : IEntityTypeConfiguration<EventDetails>
{
    public void Configure(EntityTypeBuilder<EventDetails> builder)
    {
        builder.ToTable("event_details");

        builder.HasKey(ed => ed.EventId)
            .HasName("pk_event_details");

        builder.Property(ed => ed.EventId)
            .HasConversion(
                id => id.Value,
                value => new EventId(value))
            .HasColumnName("event_id");

        builder.Property(ed => ed.Description)
            .HasMaxLength(Constants.MAX_EVENT_DETAILS_DESCRIPTION_LENGTH)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(ed => ed.Capacity)
            .IsRequired()
            .HasMaxLength(Constants.MAX_EVENT_DETAILS_CAPACITY)
            .HasColumnName("capacity");

        builder.HasOne<Event>()
            .WithOne(e => e.Details)
            .HasForeignKey<EventDetails>(ed => ed.EventId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_details_events_events");

        builder.Property(ed => ed.LastReservationUtc)
            .HasColumnType("last_reservation_utc")
            .IsRequired(false);

        builder.Property(ed => ed.Version)
            .IsRowVersion();
    }
}