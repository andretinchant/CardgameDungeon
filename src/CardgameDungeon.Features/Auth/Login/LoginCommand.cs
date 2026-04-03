using MediatR;

namespace CardgameDungeon.Features.Auth.Login;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponse>;
