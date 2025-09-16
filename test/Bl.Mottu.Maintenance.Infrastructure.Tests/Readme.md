# Bl.Mottu.Maintenance.Infrastructure.Tests

## How to execute the tests
You'll need to have the follow resources in your machine:
- .NET SDK installed on your machine. You can download it from the [.NET website](https://dotnet.microsoft.com/download).
- Docker installed on your machine. You can download it from the [Docker website](https://www.docker.com/products/docker-desktop).


### Setup the database
To execute the tests, will be necessary to have a MySQL database running. So keep the docker running before executing the tests.

#### Execute the migrations (optional)
to create the necessary database schema, go to the root folder `Desafio-BackEnd-Mottu` and then execute:
```batch
dotnet ef migrations add InitialMigration --project ".\src\maintenance\Bl.Mottu.Maintenance.Infrastructure\" --startup-project ".\test\Bl.Mottu.Maintenance.Infrastructure.Tests\" -o "Migrations"
```