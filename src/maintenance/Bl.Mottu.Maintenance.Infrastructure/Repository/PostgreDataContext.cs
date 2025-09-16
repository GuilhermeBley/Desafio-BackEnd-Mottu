using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Repository;
using Bl.Mottu.Maintenance.Infrastructure.Config;
using Bl.Mottu.Maintenance.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bl.Mottu.Maintenance.Infrastructure.Repository;

public class PostgreDataContext(IOptions<PostgreConfig>? Options = null) : DataContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var config = Options?.Value ?? new() { ConnectionString = "Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;" };
        optionsBuilder.UseNpgsql(config.ConnectionString, opt =>
        {
            
        });
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DeliveryDriverModel>(b =>
        {
            b.ToTable("DeliveryDriver");

            b.HasKey(b => b.Id);

            b.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .IsRequired();

            b.Property(p => p.Code)
                .HasColumnName("code")
                .HasColumnType("varchar(150)")
                .HasMaxLength(150)
                .IsRequired();

            b.Property(p => p.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired();

            b.Property(p => p.Cnpj)
                .HasColumnName("cnpj")
                .HasColumnType("varchar(14)")
                .HasMaxLength(14)
                .IsRequired();

            b.Property(p => p.BirthDate)
                .HasColumnName("birth_date")
                .HasColumnType("date")
                .IsRequired();

            b.Property(p => p.CnhNumber)
                .HasColumnName("cnh_number")
                .HasColumnType("varchar(11)")
                .HasMaxLength(11)
                .IsRequired();

            b.Property(p => p.CnhCategory)
                .HasColumnName("cnh_category")
                .HasColumnType("varchar(10)")
                .HasMaxLength(10)
                .IsRequired();

            b.Property(p => p.CnhImg)
                .HasColumnName("cnh_img")
                .HasColumnType("varchar(250)")
                .HasMaxLength(250)
                .IsRequired(false);

            b.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            b.HasIndex(p => p.Code)
                .IsUnique()
                .HasDatabaseName("uq_DeliveryDriver_Code");

            b.HasIndex(p => p.Cnpj)
                .IsUnique()
                .HasDatabaseName("uq_DeliveryDriver_Cnpj");
        });

        modelBuilder.Entity<MotorcycleModel>(b =>
        {
            b.ToTable("Motorcycle");

            b.HasKey(b => b.Id);

            b.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .IsRequired();

            b.Property(p => p.Code)
                .HasColumnName("code")
                .HasColumnType("varchar(150)")
                .HasMaxLength(150)
                .IsRequired();

            b.Property(p => p.Placa)
                .HasColumnName("placa")
                .HasColumnType("varchar(7)")
                .HasMaxLength(7)
                .IsRequired();

            b.Property(p => p.Model)
                .HasColumnName("model")
                .HasColumnType("varchar(250)")
                .HasMaxLength(250)
                .IsRequired();

            b.Property(p => p.Year)
                .HasColumnName("year")
                .HasColumnType("int")
                .IsRequired();

            b.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            b.HasIndex(p => p.Placa)
                .IsUnique()
                .HasDatabaseName("uq_Motorcycle_Placa");

            b.HasIndex(p => p.Code)
                .IsUnique()
                .HasDatabaseName("uq_Motorcycle_Code");
        });

        modelBuilder.Entity<VehicleRentModel>(b =>
        {
            b.ToTable("VehicleRent");

            b.HasKey(b => b.Id);

            b.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .IsRequired();

            b.Property(p => p.DeliveryDriverId)
                .HasColumnName("delivery_driver_id")
                .HasColumnType("uuid")
                .IsRequired();

            b.Property(p => p.VehicleId)
                .HasColumnName("vehicle_id")
                .HasColumnType("uuid")
                .IsRequired();

            b.Property(p => p.StartAt)
                .HasColumnName("start_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            b.Property(p => p.EndedAt)
                .HasColumnName("ended_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            b.Property(p => p.ExpectedEndingDate)
                .HasColumnName("expected_ending_date")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            b.Property(p => p.Plan)
                .HasColumnName("plan")
                .HasColumnType("int")
                .IsRequired();

            b.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            b.HasOne<DeliveryDriverModel>()
                .WithMany()
                .HasForeignKey(p => p.DeliveryDriverId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_VehicleRent_DeliveryDriver");

            b.HasOne<MotorcycleModel>()
                .WithMany()
                .HasForeignKey(p => p.VehicleId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("fk_VehicleRent_Motorcycle");

            b.HasIndex(p => p.DeliveryDriverId)
                .HasDatabaseName("ix_VehicleRent_DeliveryDriverId");

            b.HasIndex(p => p.VehicleId)
                .HasDatabaseName("ix_VehicleRent_VehicleId");

            b.HasIndex(p => p.EndedAt)
                .HasDatabaseName("ix_VehicleRent_EndedAt");
        });
    }
}
