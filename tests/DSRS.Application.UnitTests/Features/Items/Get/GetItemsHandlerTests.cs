using DSRS.Application.Contracts;
using DSRS.Application.Features.Items.Get;
using DSRS.Domain.Items;
using Moq;

namespace DSRS.Application.UnitTests.Features.Items.Get;

public class GetItemsHandlerTests
{
	[Fact]
	public async Task Handle_ReturnsSuccess_WhenItemsExist()
	{
		// Arrange
		var item1 = Item.Create("Iron Ore", "Brown", 100m, 0.5m).Data!;
		var item2 = Item.Create("Gold Bar", "Yellow", 500m, 0.3m).Data!;
		var item3 = Item.Create("Diamond", "Light Blue", 1000m, 0.7m).Data!;

		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync([item1, item2, item3]);

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.Equal(3, result.Data.Count);
		mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
	}

	[Fact]
	public async Task Handle_ReturnsSuccess_WhenNoItemsExist()
	{
		// Arrange
		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Item>());

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.Empty(result.Data);
		mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
	}

	[Fact]
	public async Task Handle_ReturnsCorrectItemProperties_WhenItemsExist()
	{
		// Arrange
		const string itemName = "Sapphire";
		const string itemDescription = "Blue Stone";
		const decimal basePrice = 600m;
		const decimal volatility = 0.55m;

		var item = Item.Create(itemName, itemDescription, basePrice, volatility).Data!;

		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Item> { item });

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.Single(result.Data);
		Assert.Equal(itemName, result.Data[0].Name);
		Assert.Equal(itemDescription, result.Data[0].Description);
		Assert.Equal(basePrice, result.Data[0].BasePrice);
		Assert.Equal(volatility, result.Data[0].Volatility);
	}

	[Fact]
	public async Task Handle_ReturnsMultipleItems_WhenMultipleItemsExist()
	{
		// Arrange
		var item1 = Item.Create("Emerald", "Green", 750m, 0.6m).Data!;
		var item2 = Item.Create("Ruby", "Red", 800m, 0.65m).Data!;
		var item3 = Item.Create("Copper Ore", "Brown", 50m, 0.4m).Data!;

		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Item> { item1, item2, item3 });

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.Equal(3, result.Data.Count);
		Assert.Equal("Emerald", result.Data[0].Name);
		Assert.Equal("Ruby", result.Data[1].Name);
		Assert.Equal("Copper Ore", result.Data[2].Name);
	}

	[Fact]
	public async Task Handle_CallsRepositoryGetAllAsync_Once()
	{
		// Arrange
		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Item>());

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		await handler.Handle(command, CancellationToken.None);

		// Assert
		mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
	}

	[Fact]
	public async Task Handle_ReturnsDataAsListType()
	{
		// Arrange
		var item = Item.Create("Coal", "Black", 25m, 0.3m).Data!;

		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Item> { item });

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.IsType<List<Item>>(result.Data);
	}

	[Fact]
	public async Task Handle_RespectsRepositoryReturnOrder()
	{
		// Arrange
		var item1 = Item.Create("First Item", "Item 1", 100m, 0.5m).Data!;
		var item2 = Item.Create("Second Item", "Item 2", 200m, 0.6m).Data!;
		var item3 = Item.Create("Third Item", "Item 3", 300m, 0.7m).Data!;

		var items = new List<Item> { item1, item2, item3 };

		var mockRepository = new Mock<IItemRepository>();
		mockRepository
			.Setup(r => r.GetAllAsync())
			.ReturnsAsync(items);

		var handler = new GetItemsHandler(mockRepository.Object);
		var command = new GetItemsCommand();

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(items, result.Data);
	}
}
