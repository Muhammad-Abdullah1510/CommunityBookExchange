namespace Identity.Models.IRepositories
{
    public interface IBookRepository
    {
        public string Add(Book book);
        public List<Book> GetAll(string id);

        public List<Book> Search(string id,string BookName);

        public string BookBelongsTo(int bookid);

        public string Title(int bookid);

        public int NoOfBooksRented(string id);
        public int NoOfBooksDonated(string id);

        void Update(Book book);
        Book GetById(int id);
        public List<Book> GetDonatedBooks(string userId);
        public List<Book> GetRentedBooks(string userId);


    }
}
