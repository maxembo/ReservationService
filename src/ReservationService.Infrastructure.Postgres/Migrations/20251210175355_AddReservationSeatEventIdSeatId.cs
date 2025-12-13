using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationSeatEventIdSeatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeatsLimit",
                table: "venues",
                newName: "seats_limit");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "reservations",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "reservations",
                newName: "event_id");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_UserId",
                table: "reservations",
                newName: "IX_reservations_user_id");

            migrationBuilder.RenameColumn(
                name: "SeatId",
                table: "reservation_seat",
                newName: "seat_id");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "reservation_seat",
                newName: "reservation_id");

            migrationBuilder.RenameIndex(
                name: "IX_reservation_seat_SeatId",
                table: "reservation_seat",
                newName: "IX_reservation_seat_seat_id");

            migrationBuilder.RenameIndex(
                name: "IX_reservation_seat_ReservationId",
                table: "reservation_seat",
                newName: "IX_reservation_seat_reservation_id");

            migrationBuilder.RenameColumn(
                name: "VenueId",
                table: "events",
                newName: "venue_id");

            migrationBuilder.RenameIndex(
                name: "IX_events_VenueId",
                table: "events",
                newName: "IX_events_venue_id");

            migrationBuilder.AddColumn<Guid>(
                name: "event_id",
                table: "reservation_seat",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_reservation_seat_event_id_seat_id",
                table: "reservation_seat",
                columns: new[] { "event_id", "seat_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reservation_seat_event_id_seat_id",
                table: "reservation_seat");

            migrationBuilder.DropColumn(
                name: "event_id",
                table: "reservation_seat");

            migrationBuilder.RenameColumn(
                name: "seats_limit",
                table: "venues",
                newName: "SeatsLimit");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "reservations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "event_id",
                table: "reservations",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_user_id",
                table: "reservations",
                newName: "IX_reservations_UserId");

            migrationBuilder.RenameColumn(
                name: "seat_id",
                table: "reservation_seat",
                newName: "SeatId");

            migrationBuilder.RenameColumn(
                name: "reservation_id",
                table: "reservation_seat",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_reservation_seat_seat_id",
                table: "reservation_seat",
                newName: "IX_reservation_seat_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_reservation_seat_reservation_id",
                table: "reservation_seat",
                newName: "IX_reservation_seat_ReservationId");

            migrationBuilder.RenameColumn(
                name: "venue_id",
                table: "events",
                newName: "VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_events_venue_id",
                table: "events",
                newName: "IX_events_VenueId");
        }
    }
}
