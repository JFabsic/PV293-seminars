namespace Yestino.Warehouse.Features.RegisterIncomingProducts;

public record RegisterIncomingProductsCommand(ICollection<IncomingProductItem> Items);

public record IncomingProductItem(Guid ProductCatalogId, int Quantity);