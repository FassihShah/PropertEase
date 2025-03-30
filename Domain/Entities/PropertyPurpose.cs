namespace Domain.Entities
{
    public class PropertyPurpose
    {
        public int PropertyPurposeId { get; set; }
        public string Name { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
}
