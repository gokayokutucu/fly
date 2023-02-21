using FizzWare.NBuilder;
using Fly.Application.DomainEvents.Products.Commands.AddProduct;
using Fly.Application.Exceptions;
using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using Fly.Tests.Helper;
using Moq;

namespace Fly.Application.Tests;

public class CreateProductDatabaseNotificationHandlerTests : TestsFor<CreateProductDatabaseNotificationHandler>
{
    [Fact]
    public async Task Handle_IfNotificationDoesNotExist_ThrowAnCustomBusinessException()
    {
        //Arrange
        CreateProductNotification fakeNotification = null;

        //Act & Assert
        await Assert.ThrowsAsync<FlyException>(async () =>
            await Instance.Handle(fakeNotification, CancellationToken.None));

    }
    
    [Fact]
    public async Task Handle_IfCancellationTokenIsRequested_ReturnNothing()
    {
        //Arrange
        var fakeNotification = Builder<CreateProductNotification>
            .CreateNew()
            .Build();

        //Act
        await Instance.Handle(fakeNotification, new CancellationToken(true));
        
        //Assert
        GetMockFor<IProductService>()
            .Verify(p=> p.CreateAsync(It.IsAny<Product>(), CancellationToken.None), Times.Never);

    }
    
    [Fact]
    public async Task Handle_RequestMustBeExist()
    {
        //Arrange
        var fakeNotification = Builder<CreateProductNotification>
            .CreateNew()
            .Build();

        //Act
        await Instance.Handle(fakeNotification, CancellationToken.None);
        
        //Assert
        GetMockFor<IProductService>()
            .Verify(p=> p.CreateAsync(It.IsAny<Product>(), CancellationToken.None), Times.Once);

    }
}