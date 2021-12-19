using Fly.Domain.Abstracts;

namespace Fly.Domain.Entities
{
    public class Product : EntityStringKey
    {
        public Product(string name, string description, string categoryId, decimal price, string currency,
            long version, bool isDeleted = false, DateTime? lastModifiedDate = null, string modifiedBy = "1", string? id = default) 
            : base(version, isDeleted, lastModifiedDate, modifiedBy, id)
        {
            Name = name;
            Description = description;
            CategoryId = categoryId;
            Price = price;
            Currency = currency;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}