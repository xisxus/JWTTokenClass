using ApiWIthJWTAuth.Contacts;
using ApiWIthJWTAuth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sharedlaibary.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Sharedlaibary.DTOs.ServiceResponse;

namespace ApiWIthJWTAuth.Repository
{
    public class AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IUserAccount
    {
        public async Task<ServiceResponse.GeneralResponse> CreateAccount(UserDTO userDTO)
        {
            if (userDTO == null) return new GeneralResponse(false, "Model is empty");
            var newUser = new ApplicationUser()
            {
                Name=userDTO.Name,
                Email=userDTO.Email,
                PasswordHash=userDTO.Password,
                UserName=userDTO.Email
            };
            var user=await userManager.FindByEmailAsync(newUser.Email);

            if(user is not null) return new GeneralResponse(false, "User Already Registered");
            var createuser=await userManager.CreateAsync(newUser!,userDTO.Password);
            if (!createuser.Succeeded) return new GeneralResponse(false, "Error Occured");
            var checkAdmin = await roleManager.FindByNameAsync("Admin");
            if(checkAdmin is  null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await userManager.AddToRoleAsync(newUser, "Admin");
                return new GeneralResponse(true, "Account created");
            }
            else
            {
                var checkUser = await roleManager.FindByNameAsync("User");
                if(checkUser is  null)
               
                    await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                    await userManager.AddToRoleAsync(newUser, "User");
                    return new GeneralResponse(true, "Account created");
                
            }
        }

        public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            if (loginDTO == null) return new LoginResponse(false, null!, "Login container is empty");
            var getUser=await userManager.FindByEmailAsync(loginDTO.Email);

            if(getUser is null) return new LoginResponse(false, null!, "User not Found");
            bool checkPassword=await userManager.CheckPasswordAsync(getUser,loginDTO.Password);

            if (!checkPassword) return new LoginResponse(false, null!, "Invalid username/password");
            var getUserRole=await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.Name, getUser.Email, getUserRole.First());

            string token = GenerateJWTToken(userSession);
            return new LoginResponse(true, token, "User login Successfull");

        }

        private string GenerateJWTToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
            var credentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role),
            };
            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                claims:userClaims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
