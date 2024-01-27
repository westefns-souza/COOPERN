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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;

    public UsuariosController(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager, 
        IUserStore<ApplicationUser> userStore
    )
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
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
    public async Task<IActionResult> CreateCooperadoAsync(CooperadoViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PreencherAreasDeAtuacao();
            viewModel.AreasAtuacao ??= [];
            return View(viewModel);
        }

        var user = Activator.CreateInstance<ApplicationUser>();
        
        user.PhoneNumber = viewModel.Celular;
        user.Email = viewModel.Email;
        user.UserName = viewModel.Email;
        user.EmailConfirmed = true;
        user.LockoutEnabled = false;
        user.FullName = viewModel.Nome;
        user.Registro = viewModel.Registro;

        var result = await _userManager.CreateAsync(user, "EducaCOOPERN$2024");

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Cooperado");

            await _context.AddRangeAsync(viewModel.AreasAtuacao.Select(x => new UsuarioAreaAtuacao { UsuarioId = user.Id, AreaAtuacaoId = x.Id }).ToList());
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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
