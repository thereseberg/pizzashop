using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosPizzeria.Models;
using TomasosPizzeria.IdentityData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TomasosPizzeria.ViewModels;

namespace TomasosPizzeria.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private SignInManager<ApplicationUser> _signinmanager;
        private UserManager<ApplicationUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private TomasosContext _context;


        public AccountController(SignInManager<ApplicationUser> signinmanager, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> roleManager, TomasosContext context)
        {
            //Injectar Identity klasserna
            _usermanager = usermanager;
            _signinmanager = signinmanager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginViewModel model)
        {
            ApplicationUser newAccount = new ApplicationUser()
            {
                Namn = model.RegisterUser.Namn,
                UserName = model.RegisterUser.AnvandarNamn,
                Email = model.RegisterUser.Email,
                Gatuadress = model.RegisterUser.Gatuadress,
                Postnr = model.RegisterUser.Postnr,
                Postort = model.RegisterUser.Postort,
                PhoneNumber = model.RegisterUser.Telefon
            };

            if (ModelState.IsValid)
            {
                //Skapar en ny användare
                var result = await _usermanager.CreateAsync(newAccount, model.RegisterUser.Losenord);

                //var result2 = await _usermanager.AddToRoleAsync(newAccount, "RegularUser");
                var result2 = await _usermanager.AddToRoleAsync(newAccount, "RegularUser");

                //Om det gick bra och användaren skapades
                if (result.Succeeded)
                {
                    //Loggar in användaren
                    await _signinmanager.SignInAsync(newAccount, isPersistent: false);
                    
                    var user2 = await _usermanager.FindByNameAsync(model.RegisterUser.AnvandarNamn);
                    var roles = await _usermanager.GetRolesAsync(user2);

                    if (roles.Contains("RegularUser") || roles.Contains("PremiumUser"))
                    {
                        return RedirectToAction("UserMain", "User");
                    }
                    else if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminMain", "Admin");
                    }
                }
                else
                {
                    //Något gick fel 
                    ViewBag.Error = "Felaktiga uppgifter försök igen";

                    return View("Main");
                }               
            }

            return View("Main");
        }

        public IActionResult Main()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            //var logIn = new ApplicationUser() { UserName = user.AnvandarNamn };

            if (ModelState.IsValid)
            {
                //var appUser = _signinmanager.UserManager.Users.SingleOrDefault(u => u.UserName == user.AnvandarNamn); //error i nästa steg om denna är null. Se över             

                var result = await _signinmanager.PasswordSignInAsync(model.UserLogin.Username, model.UserLogin.Password, false, false);

                if (result.Succeeded)
                {
                    var user2 = await _usermanager.FindByNameAsync(model.UserLogin.Username);
                    var roles = await _usermanager.GetRolesAsync(user2);

                    if (roles.Contains("RegularUser") ||roles.Contains("PremiumUser"))
                    {
                        return RedirectToAction("UserMain", "User");
                    }
                    else if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminMain", "Admin");
                    }
                }
                else
                {
                    ViewBag.MessageFailLogin = "Felaktigt användarnamn eller lösenord";
                    return View("Main");
                }
            }

            return View("Main");
        }

        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Clear();
            await _signinmanager.SignOutAsync();
            return RedirectToAction("Main");
        }


    }
}
