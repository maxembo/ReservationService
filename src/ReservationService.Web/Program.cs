using ReservationService.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "ReservationService"));
}

app.MapControllers();

app.Run();

// app.MapPost(
//     "/users", (ApplicationDbContext dbcontext) =>
//     {
//         var socials = new SocialNetwork() { Name = "Test", Link = "Test" };
//
//         var details = Details.Create("Hi", "Popov").Value;
//         details.AddSocialNetwork(socials);
//
//         var user = User.Create(new UserId(Guid.NewGuid()), details).Value;
//
//         dbcontext.Add(user);
//
//         dbcontext.SaveChanges();
//     });
//
// app.MapGet(
//     "/users",
//     (ApplicationDbContext dbcontext) =>
//     {
//         return dbcontext.Users
//             .Where(u => u.Details.SocialNetworks.Any(s => s.Link == "Test"))
//             .ToListAsync();
//     });
//
// app.MapPost(
//     "/events", (ApplicationDbContext dbcontext) =>
//     {
//         dbcontext.Add(
//             Event.Create(
//                 new EventId(Guid.NewGuid()),
//                 new VenueId(Guid.Parse("8e44e012-d8b6-4eaf-8818-540c89f8f6a6")),
//                 EventDetails.Create("description", 10).Value,
//                 EventName.Create("name").Value,
//                 DateTime.UtcNow,
//                 new ConcertInfo("Test"),
//                 EventType.Concert).Value);
//
//         dbcontext.Add(
//             Event.Create(
//                 new EventId(Guid.NewGuid()),
//                 new VenueId(Guid.Parse("8e44e012-d8b6-4eaf-8818-540c89f8f6a6")),
//                 EventDetails.Create("description", 10).Value,
//                 EventName.Create("name").Value,
//                 DateTime.UtcNow,
//                 new ConferenceInfo("Maxim", "EfCore"),
//                 EventType.Conference).Value);
//
//         dbcontext.Add(
//             Event.Create(
//                 new EventId(Guid.NewGuid()),
//                 new VenueId(Guid.Parse("8e44e012-d8b6-4eaf-8818-540c89f8f6a6")),
//                 EventDetails.Create("description", 10).Value,
//                 EventName.Create("name").Value,
//                 DateTime.UtcNow,
//                 new OnlineInfo("Test"),
//                 EventType.Online).Value);
//
//         dbcontext.SaveChanges();
//     });

