namespace BackendService.Models
{
    public record Product(Guid Id, string Name, decimal Price, int Stock);

    public record CartItem(Guid ProductId, string ProductName, int Quantity, decimal Price);

    public record AddToCartRequest(Guid ProductId, int Quantity);
}
