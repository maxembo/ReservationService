using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm");
            // migrationBuilder.Sql("CREATE UNIQUE INDEX IF NOY EXISTS ix_events_lower_name_trgm ON events USING gin (lower(name) gin_trgm_ops);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
