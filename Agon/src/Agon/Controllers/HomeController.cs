﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Agon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Agon.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        public HomeController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View(new IndexVM { Username = "Roger Rönn", LoggedIn = true, Quizzes = new List<Quiz> { new Quiz { Name = "Mitt Quiz 1" }, new Quiz { Name = "Aqua-quiz" } } });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login()
        {
            return View();
        }
    }
}