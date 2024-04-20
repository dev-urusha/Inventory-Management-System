
//namespace Inventory_Management_System.Application;
using Castle.Core.Smtp;
using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Application.Features.RoleFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Queries;
using Inventory_Management_System.Application.Services;
using Inventory_Management_System.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register the handler
        services.AddTransient<IRequestHandler<SaveRoleCommand, ResponseVm<SaveRoleCommandVm>>, SaveRoleCommandHandler>();
        services.AddTransient<IRequestHandler<RegisteredUserCommand, ResponseVm<RegisteredUserVm>>, RegisteredUserCommandHandler>();
        services.AddTransient<IRequestHandler<GetLoginUserQuery, ResponseVm<LoginVM>>, GetLoginUserQueryHandler>();
        services.AddTransient<IRequestHandler<ForgotPasswordCommand, ResponseVm<ForgotPasswordResponseVM>>, ForgotPasswordCommandHandler>();
        services.AddTransient<IRequestHandler<SendUserOTPCommand, ResponseVm<SendUserOTPVm>>, SendUserOTPCommandHandler>();
        services.AddTransient<IRequestHandler<AddOrUpdateOrDeleteProduct, ResponseVm<AddOrUpdateOrDeleteProductVm>>, AddOrUpdateOrDeleteProductHandler>();

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<SendEmailService>();
        services.AddSingleton<IEmailSender, SendEmailService>();

        return services;
    }
}

