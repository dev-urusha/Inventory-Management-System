
//namespace Inventory_Management_System.Application;
using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.RoleFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Queries;
using Inventory_Management_System.Domain;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register the handler
        services.AddTransient<IRequestHandler<SaveRoleCommand, ResponseVm<SaveRoleCommandVm>>, SaveRoleCommandHandler>();
        services.AddTransient<IRequestHandler<RegisteredUserCommand, ResponseVm<RegisteredUserVm>>, RegisteredUserCommandHandler>();
        services.AddTransient<IRequestHandler<GetLoginUserQuery, ResponseVm<LoginVM>>, GetLoginUserQueryHandler>();

        return services;
    }
}

