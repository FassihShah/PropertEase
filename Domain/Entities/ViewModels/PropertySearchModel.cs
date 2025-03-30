namespace Domain.Entities.ViewModels
{
    public class PropertySearchModel
    {
        public string City { get; set; }
        public string PropertyType { get; set; }
        public string Category { get; set; }
        public string Purpose { get; set; }
        public int? MinSize { get; set; } 
        public int? MaxSize { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }

}
