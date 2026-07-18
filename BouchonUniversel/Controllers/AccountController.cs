namespace BouchonUniversel.Controllers
{
    #region Usings

    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BouchonUniversel.Models;
    using BouchonUniversel.Models.ModelsView;
    using BouchonUniversel.Security;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.RateLimiting;
    using Microsoft.Extensions.Options;

    #endregion

    /// <summary>Contrôleur d'authentification de l'administration.</summary>
    [AllowAnonymous]
    [Route("account")]
    public sealed class AccountController : Controller
    {
        private readonly AdminSettings adminSettings;

        /// <summary>Initializes a new instance of the <see cref="AccountController" /> class.</summary>
        /// <param name="adminSettings">Les paramètres d'administration.</param>
        public AccountController(IOptions<AdminSettings> adminSettings)
            => this.adminSettings = adminSettings.Value;

        /// <summary>Affiche le formulaire de connexion.</summary>
        /// <param name="returnUrl">L'URL de retour après connexion.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View(new LoginViewModel());
        }

        /// <summary>Valide les identifiants et ouvre une session administrateur.</summary>
        /// <param name="model">Les identifiants saisis.</param>
        /// <param name="returnUrl">L'URL de retour après connexion.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;

            if (model == null || !this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (!AdminPassword.Verify(this.adminSettings, model.Username, model.Password))
            {
                this.ModelState.AddModelError(string.Empty, "Identifiant ou mot de passe incorrect.");
                return this.View(model);
            }

            var claims = new List<Claim> { new (ClaimTypes.Name, model.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).ConfigureAwait(false);

            return this.LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }

        /// <summary>Ferme la session administrateur.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return this.RedirectToAction(nameof(this.Login));
        }
    }
}
