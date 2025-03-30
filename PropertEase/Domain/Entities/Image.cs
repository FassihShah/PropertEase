namespace Domain.Entities
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Url { get; set; } // URL or path to the image

        // Foreign key to Property
        public int? PropertyId { get; set; }
        public virtual Property Property { get; set; }
    }
}
