using EbookStore.Identity.DtoModels;
using EbookStore.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EbookStore.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<AuthController> logger;

        public AuthController(SignInManager<AppUser> signInManager,
               UserManager<AppUser> userManager,
               ILogger<AuthController> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Invalid model state in register");
                    return ValidationProblem(ModelState);
                }

                var user = new AppUser
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.UserName,
                    Email = model.Email,
                };

                var registerResult = await userManager.CreateAsync(user, model.Password);

                if (!registerResult.Succeeded)
                {
                    logger.LogWarning("User registration failed: {Errors}", registerResult.Errors);
                    return BadRequest(new { Message = "Registration failed", Errors = registerResult.Errors });
                }

                logger.LogInformation("User {UserId} registered successfully", user.Id);
                return Ok(new
                {
                    Message = "User registered successfully",
                    UserId = user.Id
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in user registration");
                return StatusCode(500, new { Message = "Internal server error" });
            }

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Invalid model state in login");
                    return ValidationProblem(ModelState);
                }

                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    logger.LogWarning("Login attempt for non-existent email: {Email}", model.Email);
                    return Unauthorized(new { Message = "Invalid email or password" });
                }

                var loginResult = await signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: true,
                    lockoutOnFailure: true);

                if (loginResult.IsLockedOut)
                {
                    logger.LogWarning("User {UserId} locked out", user.Id);
                    return StatusCode(423, new { Message = "Account locked out" });
                }

                if (loginResult.IsNotAllowed)
                {
                    logger.LogWarning("Login not allowed for user {UserId}", user.Id);
                    return Forbid();
                }

                if (!loginResult.Succeeded)
                {
                    logger.LogWarning("Invalid login attempt for user {UserId}", user.Id);
                    return Unauthorized(new { Message = "Invalid email or password" });
                }

                logger.LogInformation("User {UserId} logged in", user.Id);
                return Ok(new { Message = "Login successful" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in user login");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = userManager.GetUserId(User);
                await signInManager.SignOutAsync();

                logger.LogInformation("User {UserId} logged out", userId);
                return Ok(new { Message = "Successfully logged out" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { Message = "Error during logout" });
            }
        }


        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    logger.LogWarning("Authenticated user not found");
                    return NotFound(new { Message = "User not found" });
                }

                var profileData = new ProfileDto
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName!,
                    Email = user.Email!
                };

                logger.LogInformation("Profile data fetched for user {UserId}", user.Id);
                return Ok(profileData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching profile data");
                return StatusCode(500, new { Message = "Error fetching profile data" });
            }
        }


        [Authorize]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile(EditProfileDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Invalid model state in edit profile");
                    return ValidationProblem(ModelState);
                }

                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    logger.LogWarning("User not found during profile edit");
                    return NotFound(new { Message = "User not found" });
                }


                var passwordValid = await userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordValid)
                {
                    logger.LogWarning("Invalid password attempt for profile edit by user {UserId}", user.Id);
                    return BadRequest(new { Message = "Invalid password" });
                }


                user.Name = model.Name;
                user.Surname = model.Surname;

                // belke ne vaxtsa lazim olsa, username deyishmek funksiyasi burada


                if (user.Email != model.Email)
                {
                    var emailExists = await userManager.FindByEmailAsync(model.Email) != null;
                    if (emailExists)
                    {
                        logger.LogWarning("Email {Email} already exists", model.Email);
                        return Conflict(new { Message = "Email already in use" });
                    }

                    user.Email = model.Email;
                    user.EmailConfirmed = false;
                }

                var updateResult = await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    logger.LogWarning("Profile update failed for user {UserId}: {Errors}",
                        user.Id, updateResult.Errors);
                    return BadRequest(new { Message = "Update failed", Errors = updateResult.Errors });
                }

                if (!user.EmailConfirmed)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    // email mentiqini burada yazacagam...
                    logger.LogInformation("Email confirmation token generated for user {UserId}", user.Id);
                }

                logger.LogInformation("Profile updated for user {UserId}", user.Id);
                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { Message = "Error updating profile" });
            }
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Invalid model state in change password");
                    return ValidationProblem(ModelState);
                }

                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    logger.LogWarning("User not found during password change");
                    return NotFound(new { Message = "User not found" });
                }

                var result = await userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

                if (!result.Succeeded)
                {
                    logger.LogWarning("Password change failed for user {UserId}: {Errors}",
                        user.Id, result.Errors);

                    return BadRequest(new
                    {
                        Message = "Password change failed",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                await signInManager.SignOutAsync();

                logger.LogInformation("Password changed for user {UserId}", user.Id);
                return Ok(new { Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { Message = "Error changing password" });
            }
        }


        //email mentiqini duzeltdikden sonra forgot-password action-i olacaq
        //hele ki AutoMapper lazim olmadigi ucun bu controllerde istifade etmedim

    }
}
