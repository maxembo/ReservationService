using Microsoft.EntityFrameworkCore;
using ReservationService.Contracts;
using ReservationService.Contracts.Venues.CreateVenue;
using ReservationService.Domain.Venues;
using ReservationService.Infrastructure.Postgres.Database;

namespace ReservationService.Infrastructure.Postgres.Venues;

public class VenueRepositoryTest(ApplicationDbContext dbContext)
{
    public async Task AddAsync(Venue venue) => await dbContext.Venues.AddAsync(venue);

    public async Task<List<CreateVenueDto>> GetVenuesAsync() =>
        await dbContext.Venues
            .Select(v => new CreateVenueDto(v.Id, v.VenueName))
            .ToListAsync();


    // private const string Database =
    //     "Server=localhost;Database=reservation_service_db;Port=5432;Username=postgres;Password=postgres";
    //
    // public async Task Add(Venue venue)
    // {
    //     await using var dataSource = NpgsqlDataSource.Create(Database);
    //
    //     const string sql = "Insert into venues (id, name) values (@id, @name)";
    //
    //     var command = dataSource.CreateCommand(sql);
    //
    //     command.Parameters.Add(new NpgsqlParameter("id", venue.Id));
    //     command.Parameters.Add(new NpgsqlParameter("name", venue.Name));
    //
    //     await command.ExecuteNonQueryAsync();
    // }
    //
    // public async Task<List<VenueDto>> GetVenues()
    // {
    //    await using var dataSource = NpgsqlDataSource.Create(Database);
    //    
    //    var command = dataSource.CreateCommand("select id, name from venues");
    //
    //    var venues = new List<VenueDto>();
    //    
    //    using var reader = await command.ExecuteReaderAsync();
    //
    //    while (await reader.ReadAsync())
    //    {
    //        var venueDto = new VenueDto(reader.GetGuid(0), reader.GetString(1));
    //       
    //        venues.Add(venueDto);
    //    }
    //    
    //    return venues;
    // }
}