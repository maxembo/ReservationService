namespace ReservationService.Domain.Venues;

public static class AdjacentSeatsFinder
{
    public static List<Seat> FindAdjacentSeatsInPreferredRow(
        IReadOnlyList<Seat> availableSeats,
        int requiredCount,
        int preferredRowNumber)
    {
        if (requiredCount <= 0)
        {
            return [];
        }

        var seatInRow = availableSeats
            .Where(s => s.RowNumber == preferredRowNumber)
            .OrderBy(s => s.SeatNumber)
            .ToList();

        return FindAdjacentSeatsInPreferredRow(seatInRow, requiredCount);
    }

    public static List<Seat> FindBestAdjacentSeats(
        IReadOnlyList<Seat> availableSeats,
        int requiredCount)
    {
        if (requiredCount <= 0 || availableSeats.Count < requiredCount)
        {
            return [];
        }

        var groupByRow = availableSeats.GroupBy(s => s.RowNumber);

        foreach (var row in groupByRow.OrderBy(g => g.Key))
        {
            var seatInRow = row
                .OrderBy(s => s.SeatNumber)
                .ToList();

            var adjacentSeats = FindAdjacentSeatsInPreferredRow(seatInRow, requiredCount);

            if (adjacentSeats.Count == requiredCount)
            {
                return adjacentSeats;
            }
        }

        return [];
    }

    private static List<Seat> FindAdjacentSeatsInPreferredRow(List<Seat> seatInRow, int requiredCount)
    {
        if (seatInRow.Count < requiredCount)
        {
            return [];
        }

        for (int i = 0; i <= seatInRow.Count - requiredCount; i++)
        {
            var candidateSeats = new List<Seat>();
            bool isAdjacent = true;
            for (int j = 0; j < requiredCount; j++)
            {
                var currentSeat = seatInRow[i + j];
                candidateSeats.Add(currentSeat);
                if (j > 0)
                {
                    var previousSeat = seatInRow[i + j - 1];
                    if (currentSeat.SeatNumber != previousSeat.SeatNumber + 1)
                    {
                        isAdjacent = false;
                        break;
                    }
                }
            }

            if (isAdjacent)
            {
                return candidateSeats;
            }
        }

        return [];
    }
}