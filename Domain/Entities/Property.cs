namespace Domain.Entities
{
    public class Property
    {
        public int PropertyId { get; set; }
        public string SellerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int Size { get; set; }
        public PropertyType PropertyType { get; set; }
        public Category Category { get; set; }
        public PropertyPurpose Purpose { get; set; }
        public Location Location { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }

}
