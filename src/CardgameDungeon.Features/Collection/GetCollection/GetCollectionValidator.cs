using FluentValidation;

namespace CardgameDungeon.Features.Collection.GetCollection;

public class GetCollectionValidator : AbstractValidator<GetCollectionQuery>
{
    public GetCollectionValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
    }
}
