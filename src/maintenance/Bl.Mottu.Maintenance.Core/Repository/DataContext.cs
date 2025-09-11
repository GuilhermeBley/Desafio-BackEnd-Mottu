using Bl.Mottu.Maintenance.Core.Model;

namespace Bl.Mottu.Maintenance.Core.Repository;

public class DataContext : DbContext
{
    public DbSet<DeliveryDriverModel> DeliveryDrivers { get; set; }
    public DbSet<MotorcycleModel> Motorcycles { get; set; }
    public DbSet<VehicleRentModel> VehiclesRent { get; set; }
}
