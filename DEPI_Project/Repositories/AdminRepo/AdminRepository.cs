using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories {
    public class AdminRepository: IAdminRepository {
        private readonly UserManager<AppUser> context;

        public AdminRepository(UserManager<AppUser> context) {
            this.context = context;
        }

        public async Task<string> CheckAdminLogin(string email, string password) {
            var admin = await context.FindByEmailAsync(email);
            if (admin == null) {
                return "Invalid email or password";
            }
            var isAdmin = await context.IsInRoleAsync(admin, "Admin");
            if (!isAdmin) {
                return "User is not an admin";
            }
            var result = await context.CheckPasswordAsync(admin, password);
            if (result) {
                return null;
            }
            return "Invalid email or password";
        }

        public bool CheckAdminByEmail(string email) {
            var result = context.Users.Any(e => e.Email == email);
            return result;
        }

        public bool CheckAdminByUserName(string userName) {
            var result = context.Users.Any(e => e.UserName == userName);
            return result;
        }

        public List<SelectListItem> GetAdminsAsSelectList() {
            return context.Users
                .Where(d => d.Type == "Admin")
                .Select(d => new SelectListItem {
                    Value = d.Id.ToString(),
                    Text = d.FirstName + " " + d.LastName
                })
                .ToList();
        }
    }
}
