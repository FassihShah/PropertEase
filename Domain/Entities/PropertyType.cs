namespace Domain.Entities
{
    public class PropertyType
    {
        public int PropertyTypeId { get; set; }
        public string Name { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
}
