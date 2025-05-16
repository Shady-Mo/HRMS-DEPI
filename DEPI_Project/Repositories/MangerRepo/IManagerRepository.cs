using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Identity;

namespace DEPI_Project.Repositories {
    public interface IManagerRepository {
        bool CheckManagerByEmail(string email);
        bool CheckManagerByUserName(string userName);
        Task<string> CheckManagerLogin(string email, string password);
    }
}
