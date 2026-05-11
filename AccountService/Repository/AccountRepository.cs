using AccountService.Data;
using AccountService.Models;
using AccountService.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;

namespace AccountService.Repository
{
    public class AccountRepository : IAccountRepository
    {
        //  DbContext for the account microservice
        private readonly AccountDbContext _accountDBContext;

        // manage user for indenty framework
        private readonly UserManager<ApplicationUser> _userManager;
        // manage role for identity framework

        private readonly RoleManager<IdentityRole> _roleManager;

        //Automapper

        private readonly IMapper _mapper;

        // configure the appsetting
        private readonly IConfiguration _configuration;

        public AccountRepository(AccountDbContext accountDBContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mappper)
        {
            _accountDBContext = accountDBContext ?? throw new ArgumentNullException(nameof(accountDBContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mappper ?? throw new ArgumentNullException(nameof(mappper));

        }





        public async Task<LoginResponseDTO> LoginAsync(LoginDTO login)
        {
            var validateUser = await _userManager.FindByEmailAsync(login.Email!);

            if (validateUser == null)
            {
                return new LoginResponseDTO()
                {
                    UserId = 0,
                    Token = "Invalid Email "
                };
            }
            else
            {
                if (!await _userManager.CheckPasswordAsync(validateUser, login.Password!))
                {
                    return new LoginResponseDTO()
                    {
                        UserId = 0,
                        Token = "Invalid Password Entered"
                    };
                }
                else
                {
                    // Fetch user from User table using Email
                    var userFromTable = await _accountDBContext.Users
                        .FirstOrDefaultAsync(u => u.Email == validateUser.Email);

                    if (userFromTable == null)
                    {
                        return new LoginResponseDTO()
                        {
                            UserId = 0,
                            Token = "User record not found in User table"
                        };
                    }
                    var userRoles = await _userManager.GetRolesAsync(validateUser);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, validateUser.Id),
                        new Claim(ClaimTypes.Email, validateUser.Email!)
                    };
                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    return new LoginResponseDTO()
                    {
                        UserId = userFromTable.UserId,
                        Token = GenerateToken(claims)
                    };

                }
            }


        }

        public async Task<(int, string)> RegisterAsync(NewUserDTO user, string role)
        {
            // add a user into usermanager
            //  add a role if not exist
            // add a user to user table and add the role to user
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Name = user.UserName,
                UserName = user.UserName,
                Email = user.Email

            };

            User newUser = new User();

            _mapper.Map(user, newUser);

            var userExist = await _userManager.FindByEmailAsync(user.Email!);
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (userExist == null)
            {
                var res = await _userManager.CreateAsync(applicationUser, user.Password);

                if (res.Succeeded)
                {

                    if (!roleExist)
                    {
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (!roleResult.Succeeded)
                        {
                            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                            return (0, $"Role creation failed: {errors}");
                        }
                    }


                    await _userManager.AddToRoleAsync(applicationUser, role);

                    // save changes to the database
                    await _accountDBContext.Users.AddAsync(newUser);
                    await _accountDBContext.SaveChangesAsync();

                    return (1, "user Created Succesfully");
                }
                else
                {
                    return (0, "user creation Failed");
                }
            }
            else
            {
                return (0, "User Already Exist");
            }

        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:secret"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:validIssuer"],
                Audience = _configuration["JWT:validAudience"],
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration times
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
