using FluentValidation;

namespace Fly.Application.DomainEvents.Products.Commands.AddProduct
{
    public class CreateProductNotificationValidator : AbstractValidator<CreateProductNotification>
    {
        public CreateProductNotificationValidator()
        {
            RuleFor(x => x.ProductDto.Name).NotEmpty().Length(1, 1024);
            RuleFor(x => x.ProductDto.Price).GreaterThan(0);
        }
    }
}
