using Domain.Users;
namespace Apii.Dtos;

public record TokenGenerationRequest(
    string Login,
    string Password);