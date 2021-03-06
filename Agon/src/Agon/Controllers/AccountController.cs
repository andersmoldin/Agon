﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Agon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Http;

namespace Agon.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string returnUrl = null)
        {
            const string Provider = "Spotify";
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(Provider, redirectUrl);
            return Challenge(properties, Provider);
        }
       
        [AllowAnonymous]
        [HttpPost]
        public string Fail()
        {
            return "Fail";
        }

       
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Fail));
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Fail));
            }

            //Sign in the user with this external login provider if the user already has a login.
            var signinresult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signinresult.Succeeded)
            {
                AgonManager.SetTokensInSession(info, HttpContext.Session);


                return RedirectToLocal(returnUrl);
            }
            else if (signinresult.IsNotAllowed || signinresult.IsLockedOut)
            {
                return RedirectToAction(nameof(Fail));
            }
            else
            {
                // If the user does not exist, create it.
                var user = new IdentityUser { UserName = info.ProviderKey };

                var result = await userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        AgonManager.SetTokensInSession(info, HttpContext.Session);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                        return RedirectToAction(nameof(Fail));
                }
                else
                    return RedirectToAction(nameof(Fail));
            }

        }
       
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
