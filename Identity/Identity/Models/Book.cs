namespace Identity.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Condition { get; set; }

        public string? ListedBy { get; set; }
        public bool IsRented { get; set; }

        public string? RentedBy { get; set; }

       
    }
}
