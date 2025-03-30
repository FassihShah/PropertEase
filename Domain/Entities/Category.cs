namespace Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }  // E.g., Residential, Commercial, etc.
        public virtual ICollection<Property> Properties { get; set; }
    }
}
