using System.Diagnostics;
using Identity.Models;
using Identity.Models.Repositories;
using Identity.Models.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Identity;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Identity.Data;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private  ILogger<HomeController> _logger;
        private  IBookRepository bookRepository;
        private  IRequestRepository requestRepository;
        //private  IUserNameRepository userNameRepository;
        private INotificationRepository notificationRepository;
        private string? uId;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext context;
        private readonly IProfilePictureRepository profilePictureRepository;
        private readonly SignInManager<ApplicationUser> signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, IProfilePictureRepository profilePictureRepository,ApplicationDbContext context, INotificationRepository notificationRepository,IBookRepository bookRepository,IRequestRepository requestRepository,  UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.bookRepository = bookRepository;
            this.requestRepository = requestRepository;
            //this.userNameRepository = userNameRepository;
            this.notificationRepository = notificationRepository;
            //uId = HttpContext.Session.GetString("UserId");
            _userManager = userManager;
            this.context = context;
            this.profilePictureRepository = profilePictureRepository;
            this.signInManager = signInManager;
        }
        [Authorize(Policy ="All")]        
        
        public ViewResult Index()
        {
            return View("LandingPage");
        }

        public ViewResult LogIn()
        {
            return View("LogIn");
        }

        public ViewResult SignIn()
        {
            return View("SignIn");
        }
        //[HttpPost]
        [Authorize(Policy = "All")]
        public ViewResult HomePage()
        {
            return View();
        }
        [Authorize(Policy = "BothOrDonor")]
        public ViewResult ListedBooks()
        {
            //string id = HttpContext.Session.GetString("UserId");
            //BookRepository bookRepository = new BookRepository();
            string id = _userManager.GetUserId(User);
            List<Book> books = bookRepository.GetAll(id);
            return View(books);
        }

        [Authorize(Policy = "BothOrDonor")]
        public async Task<ViewResult> DonatedBooks()
        {
            string userId = _userManager.GetUserId(User);
            var books = bookRepository.GetDonatedBooks(userId); 

            var donatedToUsernames = new List<string>();

            foreach (var book in books)
            {
                
                var renter = await _userManager.FindByIdAsync(book.RentedBy);
                donatedToUsernames.Add(renter.UserName);
                
            }

            ViewBag.DonatedTo = donatedToUsernames;
            return View(books);
        }

        [Authorize(Policy = "BothOrRenter")]
        public async Task<ViewResult> RentedBooks()
        {
            string userId = _userManager.GetUserId(User);
            var books = bookRepository.GetRentedBooks(userId); 

            var rentedFromUsernames = new List<string>();

            foreach (var book in books)
            {
                var owner = await _userManager.FindByIdAsync(book.ListedBy);
                rentedFromUsernames.Add(owner.UserName);
            }

            ViewBag.RentedFrom = rentedFromUsernames;
            return View(books);
        }

        [Authorize(Policy = "BothOrDonor")]
        public ViewResult AddBook()
        {

            return View();
        }
        [Authorize(Policy = "BothOrDonor")]
        public ViewResult BookAdded(string Title, string Author, string Condition)
        {
            Book book = new Book();
            book.Title = Title;
            book.Author = Author;
            book.Condition = Condition;
            book.IsRented = false;
            string? id = _userManager.GetUserId(User);
            book.ListedBy = id;
            book.RentedBy = null;
            //BookRepository bookRepository = new BookRepository();
            ViewBag.Result = bookRepository.Add(book);
            return View();
        }
        [Authorize(Policy = "All")]
        public ViewResult Notifications()
        {

            string userId = _userManager.GetUserId(User);
            var notifications = notificationRepository.GetAll(userId);
            return View(notifications);
        }
        [Authorize(Policy = "All")]
        [HttpPost]
        public IActionResult DeleteNotification(int id)
        {
            notificationRepository.Delete(id);
            return RedirectToAction("Notifications");
        }
        [Authorize(Policy = "BothOrRenter")]
        public async  Task<ViewResult> BookReturned(int bookId)
        {
            var book = bookRepository.GetById(bookId);
            book.IsRented = false;
            var renterId = book.RentedBy;
            var ownerId = book.ListedBy;
            book.RentedBy = null;
            bookRepository.Update(book);
            var renter = await _userManager.FindByIdAsync(renterId);
            var renterName = renter.UserName;
            string message = $"{renterName} has returned your book '{book.Title}' with ID {bookId}";

            notificationRepository.Add(new Notification
            {
                Message = message,
                UserId = ownerId
            });


            return View();
        }


        [Authorize(Policy = "All")]
        public ViewResult LogOut()
        {
            return View();
        }


        [Authorize(Policy = "All")]
        public async Task<ViewResult> ProfilePage()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            //var claims = await _userManager.GetClaimsAsync(user);

            //ViewBag.Urole = claims.FirstOrDefault(c => c.Type == "UserRole")?.Value;

            ViewBag.Urole = User.FindFirst("UserRole")?.Value;
            var picture = profilePictureRepository.GetByUserId(userId);
            ViewBag.ProfileImage = picture?.ImageUrl ?? "Default.jpg";

            ViewBag.Uname = user.UserName;
            ViewBag.NoRented = bookRepository.NoOfBooksRented(userId);
            ViewBag.NoDonated = bookRepository.NoOfBooksDonated(userId);

            return View();
        }
        [Authorize(Policy = "BothOrRenter")]
        public ViewResult RentBook()
        {
            return View();
        }

        [Authorize(Policy = "BothOrDonor")]
        public async Task<ViewResult> Requests()
        {
            var id = _userManager.GetUserId(User);
            List<Identity.Models.Request> requests = requestRepository.Get(id);

            List<string> listedByUserNames = new List<string>();

            foreach (var request in requests)
            {
                var user = await _userManager.FindByIdAsync(request.requestedBy);
                string username = user.UserName;
                listedByUserNames.Add(username);
            }

            ViewBag.Usernames = listedByUserNames;
            return View(requests);
        }
        [Authorize(Policy = "BothOrRenter")]
        public async Task<ViewResult> Search(string search)
        {
            var id = _userManager.GetUserId(User);

            List<Book> books = bookRepository.Search(id, search);

            List<string> listedByUserNames = new List<string>();
            foreach (var book in books)
            {
                var user = await _userManager.FindByIdAsync(book.ListedBy);
                string username = user.UserName;
                listedByUserNames.Add(username);

            }

            ViewBag.Usernames = listedByUserNames;

            return View(books);
        }
        [Authorize(Policy = "BothOrRenter")]
        public ViewResult BookRequested(int bookId)
        {
            Identity.Models.Request req = new Identity.Models.Request();
            req.BookId = bookId;
            req.requestedBy = _userManager.GetUserId(User);
            req.requestedTo = bookRepository.BookBelongsTo(bookId);
            req.BookName = bookRepository.Title(bookId);
            ViewBag.Result =  requestRepository.add(req);
            return View();
        }

        [Authorize(Policy = "All")]
        public ViewResult Rejected(int requestId)
        {
            var request = requestRepository.GetById(requestId); 
            if (request != null)
            {
                var bookName = request.BookName;
                var bookId = request.BookId;
                var username = User.Identity.Name;
                string message = $"{username} has rejected your request for '{bookName}' with ID {bookId}";

                notificationRepository.Add(new Notification
                {
                    Message = message,
                    UserId = request.requestedBy
                });

                requestRepository.Delete(requestId);
            }

            
            return View();
        }
        [Authorize(Policy = "BothOrDonor")]
        public async Task<ViewResult> Accepted(int requestId)
        {
            var request = requestRepository.GetById(requestId);

            if (request != null)
            {
                var book = bookRepository.GetById(request.BookId);
                book.IsRented = true;
                book.RentedBy = request.requestedBy;
                bookRepository.Update(book); 

                var owner = await _userManager.FindByIdAsync(request.requestedTo);
                string ownerName = owner.UserName;

                var notification = new Notification
                {
                    UserId = request.requestedBy,
                    Message = $"{ownerName} has accepted your request for the book '{request.BookName}' with id {request.BookId}"
                };
                notificationRepository.Add(notification);

                requestRepository.Delete(requestId);
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Policy = "All")]
        [HttpGet]
        public async Task<IActionResult> EditUserName()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = user?.UserName;
            return View();
        }
        [Authorize(Policy = "All")]
        [HttpPost]
        public async Task<IActionResult> EditUserName(string newUserName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("EditUserName");
            }

            var existingUser = await _userManager.FindByNameAsync(newUserName);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                TempData["Message"] = "Username already taken.";
                return RedirectToAction("EditUserName");
            }

            user.UserName = newUserName;
            user.NormalizedUserName = _userManager.NormalizeName(newUserName);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["Message"] = "Username updated successfully.";
            }
            else
            {
                TempData["Message"] = "Error updating username.";
            }

            return RedirectToAction("EditUserName");
        }

        [Authorize(Policy = "All")]
        public async  Task<ViewResult> Chat()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Uname = user?.UserName;
            return View();
        }

        [Authorize(Policy = "All")]
        [HttpGet]
        public ViewResult EditProfilePicture()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "All")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile imageFile)
        {
            var userId = _userManager.GetUserId(User);

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var existing = profilePictureRepository.GetByUserId(userId);
                if (existing != null)
                {
                    existing.ImageUrl = fileName;
                    profilePictureRepository.Update(existing);
                }
                else
                {
                    profilePictureRepository.Add(new ProfilePicture
                    {
                        UserId = userId,
                        ImageUrl = fileName
                    });
                }

               
            }

            return RedirectToAction("ProfilePage");
        }
        //[HttpGet]
        [Authorize(Policy = "BothOrDonor")]
        public ViewResult UpdateCondition(int bookId)
        {
            var book = bookRepository.GetById(bookId);
            ViewBag.BookTitle = book.Title;
            ViewBag.CurrentCondition = book.Condition;
            ViewBag.BookId = bookId;

            return View();
        }

        [HttpPost]

        [Authorize(Policy = "BothOrDonor")]
        public IActionResult UpdateCondition(int bookId, string newCondition)
        {
            var book = bookRepository.GetById(bookId);
            book.Condition = newCondition;
            bookRepository.Update(book);

            TempData["Message"] = "Book condition updated successfully!";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
