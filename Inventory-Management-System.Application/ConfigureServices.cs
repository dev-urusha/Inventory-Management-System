﻿
//namespace Inventory_Management_System.Application;
using Castle.Core.Smtp;
using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.CustomerFeatures.Commands;
using Inventory_Management_System.Application.Features.CustomerFeatures.Queries;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Application.Features.ProductFeatures.Queries;
using Inventory_Management_System.Application.Features.ProductSupplierFeatures.Commands;
using Inventory_Management_System.Application.Features.ProductSupplierFeatures.Queries;
using Inventory_Management_System.Application.Features.RoleFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Queries;
using Inventory_Management_System.Application.Interfaces;
using Inventory_Management_System.Application.Services;
using Inventory_Management_System.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register handlers
        services.AddTransient<IRequestHandler<SaveRoleCommand, ResponseVm<SaveRoleCommandVm>>, SaveRoleCommandHandler>();
        services.AddTransient<IRequestHandler<RegisteredUserCommand, ResponseVm<RegisteredUserVm>>, RegisteredUserCommandHandler>();
        services.AddTransient<IRequestHandler<GetLoginUserQuery, ResponseVm<LoginVM>>, GetLoginUserQueryHandler>();
        services.AddTransient<IRequestHandler<ForgotPasswordCommand, ResponseVm<ForgotPasswordResponseVM>>, ForgotPasswordCommandHandler>();
        services.AddTransient<IRequestHandler<SendUserOTPCommand, ResponseVm<SendUserOTPVm>>, SendUserOTPCommandHandler>();
        
        services.AddTransient<IRequestHandler<AddOrUpdateOrDeleteProduct, ResponseVm<AddOrUpdateOrDeleteProductVm>>, AddOrUpdateOrDeleteProductHandler>();
        services.AddTransient<IRequestHandler<AddOrUpdateOrDeleteProductSupplier, ResponseVm<AddOrUpdateOrDeleteProductSupplierResponseVM>>, AddOrUpdateOrDeleteProductSupplierHandler>();
        services.AddTransient<IRequestHandler<GetAllProductSupplierQuery, ResponseVm<List<GetAllProductSupplierResponseVM>>>, GetAllProductSupplierQueryHandler>();
        
        services.AddTransient<IRequestHandler<GetAllProductsQuery, ResponseVm<List<GetAllProductsResponseVM>>>, GetAllProductsQueryHandler>();
        services.AddTransient<IRequestHandler<ExportProducts, ResponseVm<ExportProductsResponseVM>>, ExportProductsHandler>();
        services.AddTransient<IRequestHandler<GetProductByIdQuery, ResponseVm<GetProductByIdQueryResponseVM>>, GetProductByIdQueryHandler>();

        services.AddTransient<IRequestHandler<GetCustomerByIdQuery, ResponseVm<GetCustomerByIdQueryResponseVM>>, GetCustomerByIdQueryHandler>();
        services.AddTransient<IRequestHandler<GetAllCustomersQuery, ResponseVm<List<GetAllCustomersQueryResponseVM>>>, GetAllCustomersQueryHandler>();
        services.AddTransient<IRequestHandler<AddOrUpdateOrDeleteCustomer, ResponseVm<AddOrUpdateOrDeleteCustomerResponseVM>>, AddOrUpdateOrDeleteCustomerHandler>();

        // Register services
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<SendEmailService>();
        services.AddSingleton<IEmailSender, SendEmailService>();
        services.AddSingleton<IExportExcelService, ExportExcelService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}

