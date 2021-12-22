using FluentValidation;

namespace Fly.Application.DomainEvents.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductDto.Id).Must(x => x == null || x.Length == 24);
            RuleFor(x => x.ProductDto.Name).NotEmpty().Length(1, 1024);
            RuleFor(x => x.ProductDto.Price).GreaterThan(0);
        }
    }
}
