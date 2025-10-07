using Microsoft.Extensions.DependencyInjection;

namespace StyleGenie.Application;

public static class GlobalApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application hiện chưa có triển khai để đăng ký.
        // Để đây sẵn cho validators/mediator sau này.
        return services;
    }
}
