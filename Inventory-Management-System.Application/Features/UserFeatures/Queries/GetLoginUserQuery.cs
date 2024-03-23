using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.UserFeatures.Queries
{
    #region Response Model
    public class LoginVM
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid RoleId { get; set; }
        public string Token { get; set; }
        public DateTime ValidTill { get; set; }
    }
    #endregion

    #region Resquest Model
    public class GetLoginUserQuery : IRequest<ResponseVm<LoginVM>>
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
    #endregion

    #region Handler
    public class GetLoginUserQueryHandler : ResponseWrapper<LoginVM>, IRequestHandler<GetLoginUserQuery, ResponseVm<LoginVM>>
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IApplicationDbContext _iAppDbContext;

        public GetLoginUserQueryHandler(IConfiguration configuration, IApplicationDbContext appDbContext, IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _iAppDbContext = appDbContext;
            _passwordHasher = passwordHasher;
        }
        public async Task<ResponseVm<LoginVM>> Handle(GetLoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _iAppDbContext.GetDbSet<User>().FirstOrDefaultAsync(x => x.Username == request.UserName.Trim());
            if (user == null)
            {
                return Return400(new ResponseMessage() { Description = "Invalid credentials", Status = ResponseStatus.Error });

            }
            var auth = AuthenticateUser(request.UserName, request.Password);
            if (auth == false)
            {
                throw new FluentValidation.ValidationException("Invalid Credentials.");
            }
            var authClaims = new List<Claim>
            {
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Username),
            };

            var token = GetToken(authClaims);
            var responseVm = new LoginVM()
            {
                Id = user.Id,
                Username = user.Username,
                RoleId = user.RoleId,

            };
            responseVm.Token = $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
            responseVm.ValidTill = token.ValidTo;

            return Return200WithData(responseVm, new ResponseMessage() { Description = "", Status = ResponseStatus.Success });
        }

        public bool AuthenticateUser(string userName, string password)
        {
            var user = _iAppDbContext.Users.SingleOrDefault(u => u.Username == userName);

            if (user != null)
            {
                var result = VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    return true; // Authentication successful
                }
            }

            return false; // Authentication failed
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

    }

    #endregion
}
