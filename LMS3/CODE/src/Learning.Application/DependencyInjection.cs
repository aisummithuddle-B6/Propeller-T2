using Learning.Application.Learners;
using Microsoft.Extensions.DependencyInjection;

namespace Learning.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
