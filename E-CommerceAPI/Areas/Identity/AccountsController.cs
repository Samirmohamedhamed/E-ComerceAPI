using E_CommerceAPI.DtosConfiguration.DtoRequest;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_CommerceAPI.Areas.Identity
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IApplicationUserOTPRepository applicationUserOTPRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private static readonly Random random = new();


        public AccountsController(IConfiguration configuration,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender
            , IApplicationUserOTPRepository applicationUserOTPRepository, RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.applicationUserOTPRepository = applicationUserOTPRepository;
            this.roleManager = roleManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Rigester(RegisterRequest registerRequest)
        {

            //ApplicationUser applicationUser = new()
            //{
            //    UserName = registerVM.UserName,
            //    Email = registerVM.Email,
            //    Address = registerVM.Address,
            //};
            ApplicationUser applicationUser = registerRequest.Adapt<ApplicationUser>();
            var result = await userManager.CreateAsync(applicationUser, registerRequest.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(applicationUser, SD.Customer);
                string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                if (token == null)
                {
                    Console.WriteLine("Token null");
                }
                var comfirmationLink = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", applicationUser.Id, token }, Request.Scheme);
                if (comfirmationLink == null)
                {
                    Console.WriteLine("comfirmationLink null");
                }
                await emailSender.SendEmailAsync(applicationUser.Email, "Confirmation Email", $"<h1>Confirmation Your Account By Click <a href='{comfirmationLink}'>Here</a></h1>");
                return Created();
            }

            return BadRequest(result.Errors);

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {

            var applicationUser = await userManager.FindByEmailAsync(loginRequest.UserNameOrEmail);
            ModelStateDictionary keyValuePairs = new ModelStateDictionary();

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByNameAsync(loginRequest.UserNameOrEmail);
            }
            if (applicationUser is not null)
            {

                if (applicationUser.LockoutEnd == null || applicationUser.LockoutEnd <= DateTimeOffset.UtcNow)
                {
                   
                    

                    

                    var result = await userManager.CheckPasswordAsync(applicationUser, loginRequest.Password);
                    if (result)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, applicationUser.UserName));//Name of user in security
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, applicationUser.Id));// Id of user in security
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));//id of token in jwt must be unique
                                                                                                      //get role
                        var roles = await userManager.GetRolesAsync(applicationUser); //role Of User
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));

                        }
                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
                        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                        JwtSecurityToken mytoken = new JwtSecurityToken(

                                   issuer: configuration["JWT:ValidIssuer"],
                                   audience: configuration["JWT:ValidAudiance"],
                                   claims: claims,
                                   expires: DateTime.Now.AddHours(1),
                                   signingCredentials: signingCredentials

                            );
                        await signInManager.SignInAsync(applicationUser, loginRequest.RememberMe);
                        return Ok(new
                        {
                            toke = new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expiration = mytoken.ValidTo,
                        });

                    }
                    keyValuePairs.AddModelError("Password", "Invalid Password");
                }
                else
                {
                    keyValuePairs.AddModelError(string.Empty, $"User have a block until{applicationUser.LockoutEnd}");
                }
            }
            if (applicationUser == null)
            {

                keyValuePairs.AddModelError("UserNameOrEmail", "Invalid User Name Or Email");
            }

            return BadRequest(keyValuePairs);
        }
        [HttpPost("Logout")]

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }
        [HttpGet("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmailAsync([FromQuery]string id,[FromQuery] string token)
        {
            var applicationuser = await userManager.FindByIdAsync(id);
            if (applicationuser is not null)
            {
                var result = await userManager.ConfirmEmailAsync(applicationuser, token);
                if (result.Succeeded)
                {
                    return NoContent();
                }
                else
                {

                    return BadRequest(result.Errors);
                }
            }
            return NotFound();
        }
        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest resendEmailConfirmationRequest)
        {
            
            var applicationUser = await userManager.FindByEmailAsync(resendEmailConfirmationRequest.UserNameOrEmail);
            ModelStateDictionary keyValuePairs = new ModelStateDictionary();

            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByNameAsync(resendEmailConfirmationRequest.UserNameOrEmail);
            }
            if (applicationUser is not null)

            {
                if (!applicationUser.EmailConfirmed)
                {
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var comfirmationLink = Url.Action("ConfirmEmail", "Accounts", new { area = "Identity", applicationUser.Id, token }, Request.Scheme);
                    if (comfirmationLink == null)
                    {
                        Console.WriteLine("comfirmationLink null");
                    }
                    await emailSender.SendEmailAsync(applicationUser.Email, "Resend Email", $"<h1>Confirmation Your Account By Click <a href='{comfirmationLink}'>Here</a></h1>");
                    return NoContent();
                }
                else
                {
                    keyValuePairs.AddModelError(string.Empty,"Alredy Confirmation");

                    return BadRequest(keyValuePairs);


                }
            }
            keyValuePairs.AddModelError("UserNameOrEmail", "Invalid User Name Or Email");
            return BadRequest(keyValuePairs);
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest)
        {

            
            var applicationUser = await userManager.FindByEmailAsync(forgetPasswordRequest.UserNameOrEmail);
            ModelStateDictionary keyValuePairs = new ModelStateDictionary();
            if (applicationUser == null)
            {
                applicationUser = await userManager.FindByNameAsync(forgetPasswordRequest.UserNameOrEmail);
            }
            if (applicationUser is not null)
            {
                string token = await userManager.GeneratePasswordResetTokenAsync(applicationUser);
                var OTPNumber = random.Next(1000, 9999);

                //var PasswordResetLink = Url.Action("ResetPassword", "Account", new { area = "Identity", ApplicationUserId = applicationUser.Id, Token = token }, Request.Scheme);
                //await emailSender.SendEmailAsync(applicationUser.Email, "Reset Password", $"<h1>Reset Your Password  Account By Click <a href='{PasswordResetLink}'>Here</a></h1>");
                //return RedirectToAction("Index", "Home", new { area = "Customer" });
                var OTPInDb = applicationUserOTPRepository.Get(e => e.ApplicationUserId == applicationUser.Id).OrderByDescending(x => x.ReleaseDate).FirstOrDefault();
                if (OTPInDb == null || OTPInDb.ExpirationDate < DateTime.UtcNow)
                {
                    await applicationUserOTPRepository.CreateAsync(new()
                    {
                        ApplicationUserId = applicationUser.Id,
                        OTP = OTPNumber,
                        ReleaseDate = DateTime.UtcNow,
                        ExpirationDate = DateTime.UtcNow.AddMinutes(2),

                    });
                    await applicationUserOTPRepository.CommitAsync();
                    await emailSender.SendEmailAsync(applicationUser.Email, "OTP Number", $"<h1>The OTP Number : {OTPNumber}</a></h1>");
                    //TempData[key: "Notification"] = "Check Your Email,find OTP Number";
                    //TempData[key: "_Validation"] = Guid.NewGuid().ToString();
                    return NoContent();

                }
                keyValuePairs.AddModelError(string.Empty, "There is Error");
                return BadRequest(keyValuePairs);


            }
            keyValuePairs.AddModelError("UserNameOrEmail", "Invalid User Name Or Email");
            return BadRequest(keyValuePairs);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var applicationuser = await userManager.FindByIdAsync(resetPasswordRequest.ApplicationUserId);
            ModelStateDictionary keyValuePairs = new ModelStateDictionary();

            if (applicationuser is not null)
            {
                var OTPInDb = applicationUserOTPRepository.Get(e => e.ApplicationUserId == resetPasswordRequest.ApplicationUserId).OrderByDescending(x => x.ReleaseDate).FirstOrDefault();

                if (OTPInDb.OTP == resetPasswordRequest.OTP && OTPInDb.ExpirationDate > DateTime.UtcNow)
                {
                    var result = await userManager.ResetPasswordAsync(applicationuser, resetPasswordRequest.Token, resetPasswordRequest.Password);
                    if (result.Succeeded)
                    {
                        // TempData[key: "Notification"] = "Confirm Account Successfully";
                        return NoContent();
                    }
                    else
                    {

                        return BadRequest(result.Errors);
                    }
                }
                keyValuePairs.AddModelError("OTP", "Invalid OTP or Expiration");
                return BadRequest(keyValuePairs);

            }
            return BadRequest();
        }
            [HttpPost("ConfirmationResetPassword")]
            public async Task<IActionResult> ConfirmationResetPassword(ConfirmationResetPasswordRequest confirmationResetPasswordRequest)
            {
                var applicationUser = await userManager.FindByEmailAsync(confirmationResetPasswordRequest.UserNameOrEmail);
                ModelStateDictionary keyValuePairs = new ModelStateDictionary();

                if (applicationUser == null)
                {
                    applicationUser = await userManager.FindByNameAsync(confirmationResetPasswordRequest.UserNameOrEmail);
                }

                if (applicationUser is not null)
                {
                var otpInDb = applicationUserOTPRepository.Get(e => e.ApplicationUserId == applicationUser.Id).OrderByDescending(x => x.ReleaseDate).FirstOrDefault();

                    if (otpInDb is null)
                    {
                        keyValuePairs.AddModelError("OTP", "No OTP found.");
                        return BadRequest(keyValuePairs);
                    }

                    if (otpInDb.OTP != confirmationResetPasswordRequest.OTP)
                    {
                        keyValuePairs.AddModelError("OTP", "Invalid OTP.");
                        return BadRequest(keyValuePairs);
                    }

                    if (otpInDb.ExpirationDate < DateTime.UtcNow)
                    {
                        keyValuePairs.AddModelError("OTP", "OTP has expired.");
                        return BadRequest(keyValuePairs);
                    }

                    // OTP is valid
                    string token = await userManager.GeneratePasswordResetTokenAsync(applicationUser);
                    var resetPasswordLink = Url.Action("ResetPassword", "Accounts", new
                    {
                        area = "Identity",
                        ApplicationUserId = applicationUser.Id,
                        Token = token
                    }, Request.Scheme);

                    await emailSender.SendEmailAsync(applicationUser.Email, "Reset Password Confirmation",
                        $"<h1>Reset Your Password By Clicking <a href='{resetPasswordLink}'>Here</a></h1>");

                    return NoContent();
                }

                keyValuePairs.AddModelError("UserNameOrEmail", "Invalid user.");
                return BadRequest(keyValuePairs);
            }


        }
    }
