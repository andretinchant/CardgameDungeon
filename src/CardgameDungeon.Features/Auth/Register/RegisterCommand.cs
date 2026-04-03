using MediatR;

namespace CardgameDungeon.Features.Auth.Register;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<AuthResponse>;
