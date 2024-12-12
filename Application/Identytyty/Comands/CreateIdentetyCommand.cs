using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Identytyty.Exceptionn;
using Domain;
using Domain.Identity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Identytyty.Comands;

public record CreateIdentetyCommand : IRequest<Result<string, IdentetyException>>
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}

public class CreateIdentetyCommandHandler(IUserRepository userRepository, IConfiguration configuration)
    : IRequestHandler<CreateIdentetyCommand, Result<string, IdentetyException>>
{
    private IConfiguration Config { get; } = configuration; 

    private const string TokenSecret = "ForTheLoveOfGodStoreAndLoadThisSecurely";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    
    public async Task<Result<string, IdentetyException>> Handle(CreateIdentetyCommand request,
        CancellationToken cancellationToken)
    {
        
        var user = await userRepository.GetByLoginAndPassword(request.Login, request.Password, cancellationToken);

         return await user.Match(
             u => GenerateToken(u),
             () => Task.FromResult<Result<string, IdentetyException>>(new LoginException())
         );
    }

    private async Task<Result<string, IdentetyException>> GenerateToken(User u)
    {
        try
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, u.Login),
                new("userid", u.Id.ToString()),
                new(Identety.IsAdminClaimName, u.IsAdmin.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = Config["JwtSettings:Issuer"],
                Audience = Config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return await Task.FromResult<Result<string, IdentetyException>>(jwt);
            
        }
        catch (Exception e)
        {
            return new TicketUnknownException(u.Login, e);
        }
    }
    
}