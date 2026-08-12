using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence.ModelsConfigurations;

namespace Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }


        //I decided to use DateTime, so to map it correctly i'll make an convention (global one)
    }
}






#region conventors


//I just realised that i might do interface for auditing and after just use it instead in future (I can get each class which implements it easily without reflection since Ef already does it)
//I like that, but seems like i'm over engineering things and losing time so i'll use simpler solution , which Ai gave me when i showed my code
/*public sealed class GlobalConventionsConfiguration : IModelFinalizingConvention //this instead of IModelFinalizedConvention (to not break future interceptors logic , idk if it's truth,but still)
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
        v => v,
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
        v => v,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);
        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (var property in entity.GetDeclaredProperties())
            {
                var propertyBuilder = property.Builder;

                if (property.ClrType == typeof(DateTime))
                {
                    propertyBuilder?.HasConversion(utcConverter, fromDataAnnotation: false);
                    propertyBuilder?.HasColumnType(PostgresDataTypes.TimestampWithTimeZone, fromDataAnnotation: false);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    propertyBuilder?.HasConversion(nullableUtcConverter, fromDataAnnotation: false);
                    propertyBuilder?.HasColumnType(PostgresDataTypes.TimestampWithTimeZone, fromDataAnnotation: false);
                }
            }
        }
    }
}*/
#endregion

public static class PostgresDataTypes
{
    public const string TimestampWithTimeZone = "timestamp with time zone";
    public const string TimestampWithoutTimeZone = "timestamp";
    public const string Text = "text";
    public const string Jsonb = "jsonb";
}
