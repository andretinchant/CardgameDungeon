using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>());

builder.Services.AddValidatorsFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
