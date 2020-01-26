using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using MyApi.Models;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize]
    public class TwoFactorController : ControllerBase
    {
        //private readonly IEmailSender _emailSender;
        private readonly ILogger<TwoFactorController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public TwoFactorController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            //IEmailSender emailSender,
            ILogger<TwoFactorController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<ApiResult> SendCode(int userId, string tokenProvider)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);

            if (string.IsNullOrWhiteSpace(code))
                return BadRequest();

            // await _emailSender.SendEmailAsync(
            //                    user.Email,
            //                    "کد جدید اعتبارسنجی دو مرحله‌ای",
            //                    "~/Views/EmailTemplates/_TwoFactorSendCode.cshtml",
            //                    new TwoFactorSendCodeViewModel
            //                    {
            //                        Token = code,
            //                        EmailSignature = _siteOptions.Value.Smtp.FromName,
            //                        MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
            //                    });

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> VerifyCode(VerifyDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _signInManager.TwoFactorSignInAsync(
                model.Provider,
                model.Code,
                model.RememberMe,
                model.RememberBrowser);

            if (result.Succeeded)
                Ok();

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");

                return BadRequest("Locked out");
            }

            ModelState.AddModelError(string.Empty, "کد وارد شده غیر معتبر است.");

            return BadRequest("Not valid code");
        }
    }
}