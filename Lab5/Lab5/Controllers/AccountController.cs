using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Lab5.Models;
using System.Collections.Generic;
using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Text;

namespace Lab5.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private static readonly Dictionary<string, string> _passwordHashes = new Dictionary<string, string>
		{
			{ "ADMIN@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$tfQa6BqaQU/BhiieRjCBTQ$xT9BOGyDzIa7WfyUYsPFSd7xd9M" }, //password1
			{ "LENNY@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$aQ8CkyHt2i4tWTRd/HsGYw$vJK75gT6XeRr5nzhaj+Mu1W0S6U" }, //1234567890
			{ "IRYNA@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$2cWyXhlf8hH3dzAj208P6g$C5Lr7j8NJHll/+BizmKUFtp7GsY" }, //qwerty
			{ "DAVID@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$mUjfias0QeAOIoqdn4nTzA$Er7gKer9Tlis6tEq6whMeerEe8k" }, //poiuytrewq
			{ "DMYTRO@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$f3GALZOiIcrwGiBlTJFERA$4E4QBSDsTR+/QIPu3fGznMW1LKI" }, //pass123
			{ "OLEGSANDR@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$uMTuKE0FkGLlg+b514hA1g$U2INwWw9RLqOGP0tnIgknBj4ifY" }, //0leg
			{ "ZOZULIA@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$oxMhv7HbXJY/qm7F+PjgWA$umEx9F1UGDj9nCcKgufTIf2jlwQ" }, //student
			{ "UKRAINE@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$JkIhDbr7Zo71g0AJHXj7FA$qcWWWxLzel8TsxCyunYD6fcqH5Q" }, //Europe
			{ "PHONE@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$aykpMPqjm4CueSMkL2BuoA$h2U+jrSR1F1E8+TIQzwGxtNzAY8" }, //0987654321
			{ "BRAVO@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$6opq89M71yhu5SW6Dr3iHw$CzYENLs6owV7Y0wyTFq/qmL6XJ0" }, //zaqxsw
			{ "LANA@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$+M/c0bZc0cy/HYmoxIGMOg$sU3AqSFPZ2xANpqrncKspiovF2w" }, //edcvfr
			{ "PARTY@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$bPyfrH5uIJez7+JbSXNsyA$22tN3YvnL0EgWhmwUyXHPnaxTZc" }, //tgbnhy
			{ "DIPLOMA@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$B9mBOnwwGBOWRWIouSWg8w$zVvyr8TcVzY2yR8KTGWId7hlkr8" }, //ujm,ki
			{ "MOUSE@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$oZNHl6TCJeDF49MIxzvwDQ$Z7W0byHFisbl1o0F/DXitTXWRDU" }, //ol./;p
			{ "COMPUTER@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$vL9DdvFNON0aJ8VHmbQqsw$ktvDsEqU+zMQ4UqoKIxJPVsbViI" }, //okmnji
			{ "AUTOTEST@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$RAtl68qt3/aPcc5bZBQH8w$AOwHFLoIeiOdMjITBjtMmrCiBAs" }, //uhbvgy
			{ "CHATBOT@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$QaFZsJFkZAWjZ7vj/HXpag$oRuskctl8BpZqJTjG7ZgGr5qChM" }, //tfcxdr
			{ "MANAGER@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$lk9BlrgSEQU+sZ5VpfKJJg$pnHjFD8742jW8nPfWXWd1OkyufQ" }, //eszaqw
			{ "IVORY@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$XXJe/oGsyoy78cMKPR7atQ$c8MeMBg0Hv9aI7mGZsU+f/H1yUI" }, //zxcvbnm
			{ "DANIEL@GMAIL.COM", "$argon2i$v=19$m=32768,t=10,p=5$fe3l8pQGHL2dt3IlGdGPxQ$ajCDv8IqANvAZPoYfVUA2fbtNNo" }, //asdfghjkl
		};

		private static readonly Dictionary<string, string> _passwordSalts = new Dictionary<string, string>
		{
			{ "ADMIN@GMAIL.COM", "tfQa6BqaQU/BhiieRjCBTQ==" }, //password1
			{ "LENNY@GMAIL.COM", "aQ8CkyHt2i4tWTRd/HsGYw==" }, //1234567890
			{ "IRYNA@GMAIL.COM", "2cWyXhlf8hH3dzAj208P6g==" }, //qwerty
			{ "DAVID@GMAIL.COM", "mUjfias0QeAOIoqdn4nTzA==" }, //poiuytrewq
			{ "DMYTRO@GMAIL.COM", "f3GALZOiIcrwGiBlTJFERA==" }, //pass123
			{ "OLEGSANDR@GMAIL.COM", "uMTuKE0FkGLlg+b514hA1g==" }, //0leg
			{ "ZOZULIA@GMAIL.COM", "oxMhv7HbXJY/qm7F+PjgWA==" }, //student
			{ "UKRAINE@GMAIL.COM", "JkIhDbr7Zo71g0AJHXj7FA==" }, //Europe
			{ "PHONE@GMAIL.COM", "aykpMPqjm4CueSMkL2BuoA==" }, //0987654321
			{ "BRAVO@GMAIL.COM", "6opq89M71yhu5SW6Dr3iHw==" }, //zaqxsw
			{ "LANA@GMAIL.COM", "+M/c0bZc0cy/HYmoxIGMOg==" }, //edcvfr
			{ "PARTY@GMAIL.COM", "bPyfrH5uIJez7+JbSXNsyA==" }, //tgbnhy
			{ "DIPLOMA@GMAIL.COM", "B9mBOnwwGBOWRWIouSWg8w==" }, //ujm,ki
			{ "MOUSE@GMAIL.COM", "oZNHl6TCJeDF49MIxzvwDQ==" }, //ol./;p
			{ "COMPUTER@GMAIL.COM", "vL9DdvFNON0aJ8VHmbQqsw==" }, //okmnji
			{ "AUTOTEST@GMAIL.COM", "RAtl68qt3/aPcc5bZBQH8w==" }, //uhbvgy
			{ "CHATBOT@GMAIL.COM", "QaFZsJFkZAWjZ7vj/HXpag==" }, //tfcxdr
			{ "MANAGER@GMAIL.COM", "lk9BlrgSEQU+sZ5VpfKJJg==" }, //eszaqw
			{ "IVORY@GMAIL.COM", "XXJe/oGsyoy78cMKPR7atQ==" }, //zxcvbnm
			{ "DANIEL@GMAIL.COM", "fe3l8pQGHL2dt3IlGdGPxQ==" }, //asdfghjkl
		};

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		// The Authorize Action is the end point which gets called when you access any
		// protected Web API. If the user is not logged in then they will be redirected to 
		// the Login page. After a successful login you can call a Web API.
		[HttpGet]
		public ActionResult Authorize()
		{
			var claims = new ClaimsPrincipal(User).Claims.ToArray();
			var identity = new ClaimsIdentity(claims, "Bearer");
			AuthenticationManager.SignIn(identity);
			return new EmptyResult();
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var result = validSignIn(model.Email, model.Password);
			switch (result)
			{
				case SignInStatus.Success:
					return Redirect("https://localhost:44386/Help");
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return View("Error");
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Hometown = model.Hometown };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					return RedirectToAction("Index", "Home");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}

				// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				// string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
				// await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
				// return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return code == null ? View("Error") : View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Manage");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Hometown = model.Hometown };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}



		private SignInStatus validSignIn(string userName, string password)
		{
			var loginUnified = userName.ToUpper();

			if (!_passwordHashes.TryGetValue(loginUnified, out var hashString) ||
				!_passwordSalts.TryGetValue(loginUnified, out var saltStr))
			{
				return SignInStatus.Failure;
			}
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			byte[] salt = Convert.FromBase64String(saltStr);

			var secretStr = "hgSf5IUPpuebIRdEeJy8wYcWgU/41Gx/u192JB4GZcaC5OFM3GcoC+JaZdoVV0ZVKVZQFLqLHExW3XHQzv45qCRY6UZY5NjO4yp78swCBD48yQonvBIuTf3UP15VNWBiTLCiZzhGKPHV1rkoiZfyw6KRx29BVF9uB6i3RZEQ4rrmEHIxCc+YfbwkIniRSPhFkD5ZGbYOs4s2Ky406FQDiU4ZJ7DXFarzUjx7Rmp1oYytvghR3hnSCI8aJ9DibvbQKJ4tuaLY/bPj7wtDRVYE1GBTRSRegeo4wGGo8+zBmeiXlF+fHUz9QEdbznuLEarh1ZY9ca/KWfbv8Hds5A4=";
			var secret = Convert.FromBase64String(secretStr);

			var config = new Argon2Config
			{
				Type = Argon2Type.DataIndependentAddressing,
				Version = Argon2Version.Nineteen,
				TimeCost = 10,
				MemoryCost = 32768,
				Lanes = 5,
				Threads = Environment.ProcessorCount,
				Password = passwordBytes,
				Salt = salt,
				Secret = secret,
				HashLength = 20
			};

			return Argon2.Verify(hashString, config)
				? SignInStatus.Success
				: SignInStatus.Failure;
		}

		private static byte[] getSalt(int length)
		{
			var salt = new byte[length];
			using (var random = new RNGCryptoServiceProvider())
			{
				random.GetNonZeroBytes(salt);
			}

			return salt;
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}
