using System.Linq.Expressions;
using Dapper;
using ReservationService.Application.Database;
using ReservationService.Contracts.Events;

namespace ReservationService.Application.Events.Queries;

public class GetEventsHandlerDapper
{
    private readonly IReadDbContext _readDbContext;
    private readonly INpgsqlConnectionFactory _npgsqlConnectionFactory;

    public GetEventsHandlerDapper(IReadDbContext readDbContext, INpgsqlConnectionFactory npgsqlConnectionFactory)
    {
        _readDbContext = readDbContext;
        _npgsqlConnectionFactory = npgsqlConnectionFactory;
    }

    public async Task<GetEventsDto> Handle(GetEventsRequest query, CancellationToken cancellationToken)
    {
        var connection = await _npgsqlConnectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("e.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            conditions.Add("e.type = @event_type");
            parameters.Add("event_type", query.EventType);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            conditions.Add("e.status = @status");
            parameters.Add("status", query.Status);
        }

        if (query.DateFrom.HasValue)
        {
            conditions.Add("e.event_date >= @date_from");
            parameters.Add("date_from", query.DateFrom.Value.ToUniversalTime());
        }

        if (query.DateTo.HasValue)
        {
            conditions.Add("e.event_date <= @date_to");
            parameters.Add("date_to", query.DateTo.Value.ToUniversalTime());
        }

        if (query.VenueId.HasValue)
        {
            conditions.Add("e.venue_id = @venue_id");
            parameters.Add("venue_id", query.VenueId.Value);
        }

        if (query.MinAvailableSeats.HasValue)
        {
            conditions.Add(
                """
                ((SELECT COUNT(*) FROM seats s WHERE s.venue_id = e.venue_id) - 
                 COALESCE((SELECT COUNT(*) 
                          FROM reservation_seat rs 
                                    JOIN reservations r ON rs.reservation_id = r.id
                          WHERE rs.event_id = e.id 
                            AND r.status IN ('Confirmed', 'Pending')), 0)) >= @min_available_seats
                """);
            parameters.Add("min_available_seats", query.MinAvailableSeats.Value);
        }

        parameters.Add("offset", (query.Pagination.Page - 1) * query.Pagination.PageSize);
        parameters.Add("page_size", query.Pagination.PageSize);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        string orderByField = query.SortBy?.ToLower() switch
        {
            "date" => "event_date",
            "name" => "name",
            "type" => "type",
            "status" => "status",
            "popularity" => "popularity_percentage",
            _ => "event_date"
        };

        string sortDirection = query.SortDirection?.ToLower() == "asc"
            ? "asc"
            : "desc";

        string orderByClause = $"ORDER BY {orderByField} {sortDirection}";

        long? totalCount = null;

        var events = await connection.QueryAsync<EventDto, long, EventDto>(
            $"""
             WITH event_stats AS (SELECT e.id,
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
             
                                         (SELECT COUNT(*)
                                          FROM seats s
                                          WHERE s.venue_id = e.venue_id) AS total_seats,
             
                                         (SELECT COUNT(*)
                                          FROM reservation_seat rs
                                                   JOIN reservations r ON rs.reservation_id = r.id
                                          WHERE rs.event_id = e.id
                                            AND r.status IN ('Confirmed', 'Pending')) AS reserved_seats,
                                         COUNT(*) OVER () AS total_count
                                  FROM events e
                                           JOIN event_details ed ON e.id = ed.event_id
                                  {whereClause})
             SELECT id,
                    venue_id,
                    name,
                    type,
                    event_date,
                    start_date,
                    end_date,
                    status,
                    info,
                    capacity,
                    description,
                    total_seats,
                    reserved_seats,
                    total_seats - reserved_seats as available_seats, 
                    ROUND(reserved_seats::decimal / total_seats * 100, 2) as popularity_percentage,
                    total_count
             FROM event_stats
             {orderByClause}
             LIMIT @page_size OFFSET @offset
             """,
            splitOn: "total_count", map: (@event, count) =>
            {
                totalCount ??= count;

                return @event;
            },
            param: parameters);

        return new GetEventsDto(events.ToList(), totalCount ?? 0);
    }
}