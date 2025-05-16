using DEPI_Project.Models.CorpMgmt_System;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Identity;

namespace DEPI_Project.Repositories {
    public class ManagerRepository: IManagerRepository {
        private readonly UserManager<AppUser> context;

        public ManagerRepository(UserManager<AppUser> context) {
            this.context = context;
        }

        public async Task<string> CheckManagerLogin(string email, string password) {
            if (email == null || password == null) {
                return "Invalid email or password";
            }
            var manager = await context.FindByEmailAsync(email);
            if (manager == null) {
                return "Invalid email or password";
            }
            var isManager = await context.IsInRoleAsync(manager, "Manager");
            if (!isManager) {
                return "User is not an manager";
            }
            var result = await context.CheckPasswordAsync(manager, password);
            if (result) {
                return null;
            }
            return "Invalid email or password";
        }

        public bool CheckManagerByEmail(string email) {
            var result = context.Users.Any(e => e.Email == email);
            return result;
        }

        public bool CheckManagerByUserName(string userName) {
            var result = context.Users.Any(e => e.UserName == userName);
            return result;
        }
    }
}
