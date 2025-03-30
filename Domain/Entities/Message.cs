namespace Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public string SenderId {  get; set; }
        public DateTime SentTime { get; set; }
        public string RecipientId {  get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
    }
}
