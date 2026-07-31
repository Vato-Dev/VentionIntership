using System.Collections.Concurrent;
using BackendService.Models;

namespace BackendService.Services
{
    public class ProductRepository
    {
        private readonly ConcurrentDictionary<Guid, Product> _products = new();
        private readonly ConcurrentDictionary<string, List<CartItem>> _carts = new();

        public ProductRepository()
        {
            var p1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var p2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        
            _products[p1] = new Product(p1, "Laptop Developer Pro 16", 145000.00m, 5);
            _products[p2] = new Product(p2, "Keyborard Clicky", 9500.00m, 25);
        }

        public IEnumerable<Product> GetAllProducts() => _products.Values;

        public Product? GetProductById(Guid id) => _products.TryGetValue(id, out var p) ? p : null;

        public List<CartItem> GetCart(string userId) => _carts.GetOrAdd(userId, _ => new List<CartItem>());

        public bool AddToCart(string userId, Guid productId, int quantity)
        {
            if (!_products.TryGetValue(productId, out var product) || product.Stock < quantity) 
                return false;

            var cart = GetCart(userId);
            var existing = cart.FirstOrDefault(i => i.ProductId == productId);

            if (existing != null)
            {
                cart.Remove(existing);
                cart.Add(existing with { Quantity = existing.Quantity + quantity });
            }
            else
            {
                cart.Add(new CartItem(productId, product.Name, quantity, product.Price));
            }
            return true;
        }
    }
}
