namespace Domain.Entities.ViewModels
{
    public class DetailsViewModel
    {
        public int PropertyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int Size { get; set; }
        public string PropertyType { get; set; }
        public string Category { get; set; }
        public string Purpose { get; set; }
        public List<string>? ImagesUrl { get; set; }
        public string SellerName { get; set; }
        public string SellerPhone { get; set; }
        public string Location {  get; set; }
    }
}
