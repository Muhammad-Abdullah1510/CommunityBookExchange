namespace Identity.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string? requestedBy { get; set; }
        public string? requestedTo { get; set; }

        public int BookId { get; set; }

        public string? BookName { get; set; }

    }
}
