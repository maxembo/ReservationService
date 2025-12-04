// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.ChangeTracking;
// using ReservationService.Domain.Users;
// using ReservationService.Domain.Venues;
//
// namespace ReservationService.Application.Database;
//
// public interface IApplicationDbContext
// {
//     DbSet<Venue> Venues { get; }
//     
//     DbSet<User> Users { get; }
//
//     Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
//     
//     ChangeTracker ChangeTracker { get; }
// }