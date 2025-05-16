using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories {
    public interface IAdminRepository {
        bool CheckAdminByEmail(string email);
        bool CheckAdminByUserName(string userName);
        Task<string> CheckAdminLogin(string email, string password);
        List<SelectListItem> GetAdminsAsSelectList();
    }
}
