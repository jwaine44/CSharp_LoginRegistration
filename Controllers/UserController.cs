using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using LoginRegistration.Models;

namespace LoginRegistration.Controllers;

public class UserController : Controller
{
    private LoginRegContext _context;

    public UserController(LoginRegContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index");
    }

    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
                return Index();
            }

            HttpContext.Session.SetInt32("userId", 4);
            HttpContext.Session.SetString("email", newUser.Email);
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        return Index();
    }

    [HttpGet("/login")]
    public IActionResult ShowLogin()
    {
        return View("Login");
    }

    [HttpPost("/login")]
    public IActionResult Login(LoginUser userSubmission)
    {
        if(ModelState.IsValid)
        {
            // If initial ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

            // If no user exists with provided email
            if(userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return Login(userSubmission);
            }
            
            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();
            
            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
            
            // result can be compared to 0 for failure
            if(result == 0)
            {
                ModelState.AddModelError("Password", "Invalid password!");
                return Login(userSubmission);
            }

            HttpContext.Session.SetInt32("userId", userInDb.id);
            HttpContext.Session.SetString("email", userInDb.Email);

            return RedirectToAction("Success");
        }
        return View("Login");
    }

    [HttpGet("/success")]
    public IActionResult Success()
    {
        int? loggedIn = HttpContext.Session.GetInt32("userId");

        if(loggedIn != null)
        {
            return View("Success");
        }    
        else
        {
            return Index();
        }
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}