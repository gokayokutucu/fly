using FluentValidation;

namespace Fly.Application.DomainEvents.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).Must(x => x == null || x.Length == 24);
        }
    }
}
