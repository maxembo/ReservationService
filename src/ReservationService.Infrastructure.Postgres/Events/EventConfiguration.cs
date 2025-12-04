using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Events;
using ReservationService.Domain.Shared;
using ReservationService.Domain.Venues;

namespace ReservationService.Infrastructure.Postgres.Events;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(e => e.Id)
            .HasName("pk_events");

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new EventId(value))
            .HasColumnName("id");

        builder.ComplexProperty(
            e => e.Name, eb =>
            {
                eb.Property(e => e.Value)
                    .HasMaxLength(Constants.MAX_NAME_LENGTH)
                    .IsRequired()
                    .HasColumnName("name");
            });

        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_events_venues_venue_id");

        builder.Property(e => e.EventDate)
            .HasColumnName("event_date");

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasColumnName("type");

        builder.Property(e => e.Info)
            .HasConversion(new EventInfoConverter())
            .HasColumnName("info");

        builder.HasIndex(e => e.EventDate);

        // builder.HasIndex(e => e.Name)
        //     .HasDatabaseName("ix_events_name_trgm")
        //     .HasMethod("gin")
        //     .HasOperators("gin_trgm_ops");
    }
}