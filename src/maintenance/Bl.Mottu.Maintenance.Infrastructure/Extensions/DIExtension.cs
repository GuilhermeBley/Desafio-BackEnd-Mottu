using Bl.Mottu.Maintenance.Core.Events;
using Bl.Mottu.Maintenance.Infrastructure.Config;
using Bl.Mottu.Maintenance.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
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
            .AddDbContext<Core.Repository.DataContext, Repository.PostgreDataContext>()
            .AddSingleton<Core.Repository.IStreamFileRepository, Repository.StreamFileRepository>()
            .AddSingleton<IEventBus, RabbitMqEventBus>()
            .AddSingleton<IConnection>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<RabbitMqConfig>>();
                var factory = new ConnectionFactory
                {
                    HostName = opt.Value.HostName,
                    Port = AmqpTcpEndpoint.DefaultAmqpSslPort,
                    UserName = opt.Value.UserName,
                    Password = opt.Value.Password,
                    VirtualHost = "/",
                    DispatchConsumersAsync = true,
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                };
                return factory.CreateConnection();
            });
    }
}
