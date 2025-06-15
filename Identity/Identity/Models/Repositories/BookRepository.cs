using Identity.Models.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Identity.Data;
using static System.Reflection.Metadata.BlobBuilder;

namespace Identity.Models.Repositories
{
    public class BookRepository : IBookRepository
    {
        private ApplicationDbContext context;

        public BookRepository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public string Add(Book book)
        {
            try
            {
                context.Books.Add(book);
                context.SaveChanges();
                return "Book Added Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
                return "Error Occurred, Try Again!";
            }
        }

        public List<Book> GetAll(string id)
        {
            try
            {
                return context.Books.Where(b => b.ListedBy == id).ToList();
            }
            catch (Exception ex)
            {
                return new List<Book>{new Book
                                             {
                                                Title = "Error",
                                                Author = "System",
                                                Condition = ex.Message,
                                                ListedBy = "System"
                                               }
                                       };

                //return new List<Book>();
            }

            


        }

        public List<Book> Search(string id, string BookName)
        {
            try
            {
                return context.Books.Where(b => b.ListedBy != id && !b.IsRented &&b.Title.ToLower().Contains(BookName.Trim().ToLower()))
    .ToList();
            }
            catch (Exception ex)
            {
                return new List<Book>{new Book
                                             {
                                                Title = "Error",
                                                Author = "System",
                                                Condition = ex.Message,
                                                ListedBy = "System"
                                               }
                                       };

                //return new List<Book>();
            }
        }


        public string? BookBelongsTo(int bookid)
        {
            try
            {
                var book = context.Books.Find(bookid);
                return book.ListedBy;
            }
            catch (Exception ex) {
                return null;
            }

        }

        public string? Title(int bookid)
        {
            try
            {
                var book = context.Books.Find(bookid);
                return book.Title;
            }
            catch (Exception ex) { 
                return null;
            }

        }


        public int NoOfBooksRented(string userId)
        {
            return context.Books.Count(b => b.IsRented && b.RentedBy == userId);
        }

        public int NoOfBooksDonated(string userId)
        {
            return context.Books.Count(b => b.IsRented && b.ListedBy == userId);
        }

        public void Update(Book book)
        {
            var existingBook = context.Books.First(b => b.Id == book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Condition = book.Condition;
                existingBook.ListedBy = book.ListedBy;
                existingBook.IsRented = book.IsRented;
                existingBook.RentedBy = book.RentedBy;

                context.SaveChanges();
            }
        }
        public Book GetById(int id)
        {
            return context.Books.First(b => b.Id == id);
        }

        public List<Book> GetRentedBooks(string userId)
        {
            return context.Books
                .Where(b => b.RentedBy == userId && b.IsRented)
                .ToList();
        }

        public List<Book> GetDonatedBooks(string userId)
        {
            return context.Books
                .Where(b => b.ListedBy == userId && b.IsRented)
                .ToList();
        }

    }



}

