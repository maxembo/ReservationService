using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationService.Domain.Reservations;
using ReservationService.Domain.Shared;
using ReservationService.Domain.Users;

namespace ReservationService.Infrastructure.Postgres.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id)
            .HasName("pk_user");

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .HasColumnName("id");

        // builder.OwnsMany(u => u.SocialNetworks, sb =>
        // {
        //     sb.ToJson("socials");
        //     
        //     sb.Property(u => u.Name)
        //         .IsRequired()
        //         .HasMaxLength(Constants.MaxNameLength)
        //         .HasColumnName("name");
        //     
        //     sb.Property(u => u.Link)
        //         .IsRequired()
        //         .HasMaxLength(Constants.MaxNameLength)
        //         .HasColumnName("link");
        // });

        // 1 вариант
        // builder.Property(u => u.Details)
        //     .HasConversion(
        //         v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
        //         json => JsonSerializer.Deserialize<Details>(json, JsonSerializerOptions.Default)!)
        //     .HasColumnType("jsonb");

        // 2 вариант
        builder.OwnsOne(
            u => u.Details, db =>
            {
                db.ToJson("details");

                db.Property(u => u.Description)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_NAME_LENGTH)
                    .HasColumnName("description");

                db.Property(u => u.Fio)
                    .IsRequired()
                    .HasMaxLength(Constants.MAX_SEAT_NUMBER)
                    .HasColumnName("fio");

                db.OwnsMany(
                    u => u.SocialNetworks, sb =>
                    {
                        sb.ToJson("social_networks");

                        sb.Property(u => u.Name)
                            .IsRequired()
                            .HasMaxLength(Constants.MAX_NAME_LENGTH)
                            .HasColumnName("name");

                        sb.Property(u => u.Link)
                            .IsRequired()
                            .HasMaxLength(Constants.MAX_NAME_LENGTH)
                            .HasColumnName("link");
                    });
            });

        builder.HasMany(u => u.Reservations)
            .WithOne()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired()
            .HasConstraintName("fk_user_reservation_user");
    }
}