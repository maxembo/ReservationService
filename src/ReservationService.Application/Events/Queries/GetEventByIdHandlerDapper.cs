using Dapper;
using ReservationService.Application.Database;
using ReservationService.Contracts.Events;
using ReservationService.Contracts.Venues.Seats;

namespace ReservationService.Application.Events.Queries;

public class GetEventByIdHandlerDapper
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetEventByIdHandlerDapper(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetEventDto?> Handle(GetEventByIdRequest query, CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        GetEventDto? eventDto = null;

        await connection.QueryAsync<GetEventDto, SeatDto, GetEventDto>(
            """
            SELECT e.id,
                   e.venue_id,
                   e.name,
                   e.type,
                   e.event_date,
                   e.start_date,
                   e.end_date,
                   e.status,
                   e.info,
                   ed.capacity,
                   ed.description,
                   s.id,
                   s.venue_id,
                   s.row_number,
                   s.seat_number,
                   rs.id is null as is_available
            FROM events e 
            JOIN event_details ed ON ed.event_id = e.id
            JOIN seats s ON e.venue_id = s.venue_id
            LEFT JOIN reservation_seat rs ON s.id = rs.seat_id and rs.event_id = e.id
            WHERE e.id = @eventId
            ORDER BY row_number, seat_number
            """,
            param: new { @eventId = query.EventId },
            splitOn: "id",
            map: (e, s) =>
            {
                eventDto ??= e;

                eventDto.Seats.Add(s);

                return eventDto;
            });

        return eventDto;
    }
}