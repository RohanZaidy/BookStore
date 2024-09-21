using BookStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using System.Diagnostics;

//Git Hub

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookstoreContext _bookstoreContext;

        public HomeController(ILogger<HomeController> logger, BookstoreContext bookstoreContext)
        {
            _logger = logger;
            _bookstoreContext = bookstoreContext;
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("AdminEnter") != null)
            {
                return RedirectToAction("AdminHome", "Admin");
            }

            if (HttpContext.Session.GetString("loggedin") != null)
            {
                return RedirectToAction("Homme");
            }

            TempData["LoggedIn"] = null;
            return View();
        }

        public IActionResult LogOut()
        {
            if (HttpContext.Session.GetString("loggedin") != null)
            {
                HttpContext.Session.Remove("loggedin");
                TempData["LoggedIn"] = null;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Index(User u)
        {
            TempData["LoggedIn"] = null;
            var myuser = _bookstoreContext.Users.Where(x => x.Email == u.Email && x.Password == u.Password).FirstOrDefault();
            if (myuser != null)
            {
                TempData["LoggedIn"] = "loggedin";
                HttpContext.Session.SetString("loggedin", "true");
                return RedirectToAction("Homme", "Home");
            }
            else
            {
                TempData["fail"] = "Failed";
            }
            return View(u);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User u)
        {
            if (ModelState.IsValid)
            {
                await _bookstoreContext.Users.AddAsync(u);
                await _bookstoreContext.SaveChangesAsync();
                TempData["Success"] = "Successfully Register";
                return RedirectToAction("Index", "Home");
            }

            return View(u);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Homme()
        {
            if (HttpContext.Session.GetString("loggedin") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.GetString("loggedin") != null)
            {
                var books = await _bookstoreContext.Books.ToListAsync();
                TempData["LoggedIn"] = "true";
                return View(books);
            }
            return BadRequest("Invalid");
        }

        public IActionResult ForGot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForGot(User u)
        {
                var usr = await _bookstoreContext.Users.SingleOrDefaultAsync(x => x.Email == u.Email);
                if(usr == null)
                {
                TempData["NoEmail"] = "noemailregistered";
                return RedirectToAction("ForGot", "Home");
                }
                if (usr != null)
                {
                    usr.Password = u.Password;
                    await _bookstoreContext.SaveChangesAsync();
                    TempData["PasswordChanged"] = "Done";
                    return RedirectToAction("ForGot", "Home");
                }
            
            return View(u);
        }

        public async Task<IActionResult> DetailForUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookstoreContext.Books
                .FirstOrDefaultAsync(b => b.Isbn == id);

            if (book == null)
            {
                return NotFound();
            }

            TempData["LoggedIn"] = "true";
            return View(book);
        }

        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookstoreContext.Books.FirstOrDefaultAsync(b => b.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            var cartitem = await _bookstoreContext.Carts.FirstOrDefaultAsync(c => c.BookIsbn == id);
            if (cartitem == null)
            {
                cartitem = new Cart
                {
                    BookIsbn = book.Isbn
                };

               await _bookstoreContext.Carts.AddAsync(cartitem);
                TempData["BookaddtoCart"] = "true";
            }
           await _bookstoreContext.SaveChangesAsync();


            return RedirectToAction("Homme");
        }

        public async Task<IActionResult> cartview()
        {
            TempData["LoggedIn"] = "true";
            var cart =  await _bookstoreContext.Carts.Include(c => c.BookIsbnNavigation).ToListAsync();

            return View(cart);
        }

        public async Task<IActionResult> RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartitem = await _bookstoreContext.Carts.FirstOrDefaultAsync(c => c.BookIsbn == id);
            if (cartitem == null)
            {
                return NotFound();
            }

             _bookstoreContext.Carts.Remove(cartitem);
            await _bookstoreContext.SaveChangesAsync();

            return RedirectToAction("cartview");
        }

        public IActionResult ConfirmOrder()
        {
            TempData["LoggedIn"] = "loggedin";
            return View();
        }

        public IActionResult Done()
        {
            TempData["OrderComplete"] = "true";
            _bookstoreContext.Carts.RemoveRange(_bookstoreContext.Carts);
            _bookstoreContext.SaveChangesAsync();
            return RedirectToAction("Homme");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}