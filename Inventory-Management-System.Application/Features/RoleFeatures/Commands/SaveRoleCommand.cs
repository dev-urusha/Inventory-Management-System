using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Interfaces;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Application.Features.RoleFeatures.Commands
{
    #region Response Model
    public class SaveRoleCommandVm
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }
    }
    #endregion

    #region Request Model
    public class SaveRoleCommand : IRequest<ResponseVm<SaveRoleCommandVm>>
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }

    }
    #endregion

    #region Handler
    public class SaveRoleCommandHandler : ResponseWrapper<SaveRoleCommandVm>, IRequestHandler<SaveRoleCommand, ResponseVm<SaveRoleCommandVm>>
    {
        private readonly ILogger<SaveRoleCommandHandler> _logger;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SaveRoleCommandHandler(ILogger<SaveRoleCommandHandler> logger, IApplicationDbContext context, ICurrentUserService currentUserService) 
        {
            _logger = logger;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseVm<SaveRoleCommandVm>> Handle(SaveRoleCommand command, CancellationToken cancellationToken)
        {
            try
            {
                Role role;
                #region Add a new Role
                if (command.Id == null || command.Id == Guid.Empty)
                {
                    role = new Role()
                    {
                        Id = Guid.NewGuid(),
                        Name = command.Name,
                        Description = command.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        CreatedBy = _currentUserService.UserName
                    };
                    _context.Roles.Add(role);
                }
                #endregion

                #region Update an existing Role
                else
                {
                    role = command.Id != null && command.Id != Guid.Empty ? await _context.Roles.FirstOrDefaultAsync(x => x.Id == command.Id && !x.IsDeleted) : null;
                    if (role != null)
                    {
                        role.Name = command.Name;
                        role.Description = command.Description;
                        role.LastModifiedAt = DateTime.UtcNow;
                        role.LastModifiedBy = _currentUserService.UserName;
                        role.IsDeleted = command.IsDeleted ?? role.IsDeleted;
                        _context.Roles.Update(role);
                    }
                }
                #endregion

                await _context.SaveChangesAsync(cancellationToken);

                //Set Response
                var responseVm = new SaveRoleCommandVm()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                };

                //Return response with success data
                return Return200WithData(responseVm, new ResponseMessage() { Name = "Success", Description = "Requested role added successfully.", Status = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                //Throw exception
                _logger.LogError($"SaveRoleCommandHandler.Handle: Error {ex.Message}", ex);
                return Return400(new ResponseMessage() { Name = "Error", Description = "Failed to add the requested role.", Status = ResponseStatus.Error });
            }

        }

    }

    #endregion
}
