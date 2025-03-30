namespace Domain.Entities
{
    public class Location
    {
        public int LocationId { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Street { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
        public override string ToString()
        {
            return Street + ", " + Area + ", " + City;
        }
    }
}
