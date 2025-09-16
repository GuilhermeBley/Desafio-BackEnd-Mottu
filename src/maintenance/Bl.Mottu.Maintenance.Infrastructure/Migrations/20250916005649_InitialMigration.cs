using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bl.Mottu.Maintenance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryDriver",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    cnpj = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    cnh_number = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    cnh_category = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    cnh_img = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDriver", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Motorcycle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    placa = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: false),
                    model = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motorcycle", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expected_ending_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    plan = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRent", x => x.id);
                    table.ForeignKey(
                        name: "fk_VehicleRent_DeliveryDriver",
                        column: x => x.delivery_driver_id,
                        principalTable: "DeliveryDriver",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_VehicleRent_Motorcycle",
                        column: x => x.vehicle_id,
                        principalTable: "Motorcycle",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "uq_DeliveryDriver_Cnpj",
                table: "DeliveryDriver",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_DeliveryDriver_Code",
                table: "DeliveryDriver",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_Motorcycle_Code",
                table: "Motorcycle",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_Motorcycle_Placa",
                table: "Motorcycle",
                column: "placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_VehicleRent_DeliveryDriverId",
                table: "VehicleRent",
                column: "delivery_driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_VehicleRent_EndedAt",
                table: "VehicleRent",
                column: "ended_at");

            migrationBuilder.CreateIndex(
                name: "ix_VehicleRent_VehicleId",
                table: "VehicleRent",
                column: "vehicle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleRent");

            migrationBuilder.DropTable(
                name: "DeliveryDriver");

            migrationBuilder.DropTable(
                name: "Motorcycle");
        }
    }
}
