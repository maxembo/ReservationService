using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDetailsRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReservationUtc",
                table: "event_details",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "event_details",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReservationUtc",
                table: "event_details");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "event_details");
        }
    }
}
