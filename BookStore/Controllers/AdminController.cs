using BookStore.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly BookstoreContext _bookstoreContext;
        IWebHostEnvironment env;

        public AdminController(BookstoreContext bookstoreContext, IWebHostEnvironment env)
        {
            _bookstoreContext = bookstoreContext;
            this.env = env;
        }

        public IActionResult Adminlogin()
        {
            if (HttpContext.Session.GetString("AdminEnter") != null)
            {
                return RedirectToAction("AdminHome", "Admin");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Adminlogin(Admin a)
        {

            var myAdmin = _bookstoreContext.Admins.Where(x => x.UserName == a.UserName && x.Password == a.Password).FirstOrDefault();

            if(myAdmin != null)
            {
                TempData["AdminAccess"] = "True";
                HttpContext.Session.SetString("AdminEnter", "true");
                return RedirectToAction("AdminHome", "Admin");
            }
            else
            {
                TempData["adminnotaccess"] = "true";
            }

            return View(a);
        }

        public async Task<IActionResult> AdminHome()
        {
            TempData["AdminAccess"] = "True";
            if (HttpContext.Session.GetString("AdminEnter") == null)
            {
                return RedirectToAction("Adminlogin", "Admin");
            }

            var b = await _bookstoreContext.Books.ToListAsync();

            return View(b);
        }

        public IActionResult logout()
        {
            if (HttpContext.Session.GetString("AdminEnter") != null)
            {
                HttpContext.Session.Remove("AdminEnter");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Create()
        {
            TempData["AdminAccess"] = "True";
            if (HttpContext.Session.GetString("AdminEnter") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(BookViewModel b)
        {
            //TempData["AdminAccess"] = "True";
            if (HttpContext.Session.GetString("AdminEnter") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string fileName = "";
            if(b.Pic != null)
            {
               string folder = Path.Combine(env.WebRootPath,"images");
               fileName = Guid.NewGuid().ToString() + "_" + b.Pic.FileName;
               string filePath = Path.Combine(folder, fileName);
               b.Pic.CopyTo(new FileStream(filePath, FileMode.Create));

                Book book = new Book()
                {
                    Title = b.Title,
                    Author = b.Author,
                    Description = b.Description,
                    Price = b.Price,
                    Language = b.Language,
                    Pages = b.Pages,
                    Genre = b.Genre,
                    Image = fileName
                };

                _bookstoreContext.Books.Add(book);
                _bookstoreContext.SaveChanges();
                TempData["BoodAdded"] = "True";
                return RedirectToAction("AdminHome", "Admin");
            }

            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            TempData["AdminAccess"] = "True";
            if (id == null)
            {
                return BadRequest("Invalid");
            }

            var book = await _bookstoreContext.Books.FirstOrDefaultAsync(m => m.Isbn == id);

            if(book == null)
            {
                return BadRequest("Invalid");
            }
            return View(book);
        }

        //public async Task<IActionResult> Delete(int? id)
        //{
        //    TempData["AdminAccess"] = "True";
        //    if (id == null)
        //    {
        //        return BadRequest("Invalid");
        //    }

        //    var book = await _bookstoreContext.Books.FindAsync(id);

        //    if (book != null)
        //    {
        //        _bookstoreContext.Remove(book);
        //    }
        //    await _bookstoreContext.SaveChangesAsync();
        //    return RedirectToAction("AdminHome");
        //}

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("Invalid");
            }

            var book = await _bookstoreContext.Books.FindAsync(id);

            if (book != null)
            {
                // Construct the file path for the image
                string filePath = Path.Combine(env.WebRootPath, "images", book.Image);

                // Check if the file exists and delete it
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                else
                {
                    return BadRequest("No File Found");
                }

                // Remove the book from the database
                _bookstoreContext.Books.Remove(book);
                await _bookstoreContext.SaveChangesAsync();
                TempData["BookDeleted"] = "True";
            }
            else
            {
                return NotFound("Book not found");
            }

            return RedirectToAction("AdminHome");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            TempData["AdminAccess"] = "True";
            if (id == null)
            {
                return BadRequest("Invalid");
            }

            var book = await _bookstoreContext.Books.FindAsync(id);
            if(book == null )
            {
                return BadRequest("Invalid");
            }

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, Book b)
        {
            TempData["AdminAccess"] = "True";
            if (id != b.Isbn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _bookstoreContext.Books.Update(b);
                await _bookstoreContext.SaveChangesAsync();
                TempData["UpdateSuccess"] = "Updated...";
                return RedirectToAction("AdminHome", "Admin");
            }
            return View(b);
        }

    }
}
