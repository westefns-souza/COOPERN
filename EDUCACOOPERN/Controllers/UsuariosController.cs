using EDUCACOOPERN.Data;
using EDUCACOOPERN.Models;
using EDUCACOOPERN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDUCACOOPERN.Controllers;

[Authorize(Roles = "Coordenador")]
public class UsuariosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UsuariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarios = await _userManager.Users
            .OrderBy(x => x.Email)
            .ToListAsync();

        return View(usuarios);
    }

    [HttpGet]
    public IActionResult CreateCooperado()
    {
        var viewModel = new CooperadoViewModel()
        {
            Ativo = true,
            AreasAtuacao = [],
        };
        
        PreencherAreasDeAtuacao();
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult CreateCooperado(CooperadoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PreencherAreasDeAtuacao();
            return View(viewModel);
        }

        return View(viewModel);
    }

    #region ViewBag
    
    private void PreencherAreasDeAtuacao()
    {
        var areasDeAtuacao = _context.AreaAtuacao
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nome })
            .ToList();

        ViewBag.AreaAtuacao = areasDeAtuacao;
    }

    #endregion

    //public async Task<IActionResult> Post(string returnUrl = null)
    //{
    //    returnUrl ??= Url.Content("~/");
    //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    //    if (ModelState.IsValid)
    //    {
    //        var user = CreateUser();

    //        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
    //        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
    //        var result = await _userManager.CreateAsync(user, Input.Password);

    //        if (result.Succeeded)
    //        {
    //            _logger.LogInformation("User created a new account with password.");

    //            var userId = await _userManager.GetUserIdAsync(user);
    //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    //            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
    //            var callbackUrl = Url.Page(
    //                "/Account/ConfirmEmail",
    //                pageHandler: null,
    //                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
    //                protocol: Request.Scheme);

    //            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
    //                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

    //            if (_userManager.Options.SignIn.RequireConfirmedAccount)
    //            {
    //                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
    //            }
    //            else
    //            {
    //                await _signInManager.SignInAsync(user, isPersistent: false);
    //                return LocalRedirect(returnUrl);
    //            }
    //        }
    //        foreach (var error in result.Errors)
    //        {
    //            ModelState.AddModelError(string.Empty, error.Description);
    //        }
    //    }

    //    // If we got this far, something failed, redisplay form
    //    return Page();
    //}
}
