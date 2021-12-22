using FluentValidation;

namespace Fly.Application.DomainEvents.Products.Dtos
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Id).Must(x => x == null || x.Length == 24);
            RuleFor(x => x.Name).NotEmpty().Length(1, 1024);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
}
