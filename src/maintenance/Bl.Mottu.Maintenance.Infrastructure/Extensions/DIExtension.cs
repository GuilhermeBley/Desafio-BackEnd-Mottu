using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bl.Mottu.Maintenance.Infrastructure.Extensions;

public static class DIExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Core.Commands.CreateDeliveryDriver.CreateDeliveryDriverHandler).Assembly);
            })
            .AddDbContext<Core.Repository.DataContext, Repository.PostgreDataContext>();
    }
}
