using DEPI_Project.DTOs;
using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DEPI_Project.Controllers {
    public class AuthController: Controller {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Login() => View();

        #region Registeration
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(EmployeeDTO employeeDTO) {
            if (!ModelState.IsValid)
                return View("Register", employeeDTO);

            var userExists = await _userManager.FindByEmailAsync(employeeDTO.Email);
            if (userExists != null) {
                ModelState.AddModelError("", "Email already exists");
                return View("Register", employeeDTO);
            }

            var userNameExists = await _userManager.FindByNameAsync(employeeDTO.UserName);
            if (userNameExists != null) {
                ModelState.AddModelError("", "Username already exists");
                return View("Register", employeeDTO);
            }

            var user = new Employee {
                Email = employeeDTO.Email,
                UserName = employeeDTO.UserName,
                FirstName = employeeDTO.FirstName,
                LastName = employeeDTO.LastName,
                Gender = employeeDTO.Gender,
                DateOfBirth = employeeDTO.DateOfBirth,
                PhoneNumber = employeeDTO.PhoneNumber,
                Nationality = employeeDTO.Nationality,
                Salary = employeeDTO.Salary,
                Position = employeeDTO.Position,
                EmpStatus = StatusType.Active,
                ContractType = employeeDTO.ContractType,
                HireDate = employeeDTO.HireDate,
                MaritalStatus = employeeDTO.MaritalStatus,
                Skills = employeeDTO.Skills,
                Address = employeeDTO.Address,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, employeeDTO.Password);
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View("Register", employeeDTO);
            }
            return RedirectToAction("Index", "Home");
        } 
        #endregion

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO) {
            if (!ModelState.IsValid)
                return View("Login");

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null) {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("Login");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, true);
            if (!result.Succeeded) {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();
            if (role == null || !string.Equals(role, loginDTO.UserType, StringComparison.OrdinalIgnoreCase)) {
                ModelState.AddModelError("", "You do not have permission to access this type.");
                return View("Login");
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return RedirectToDashboard(role);
        }

        public IActionResult RedirectToDashboard(string role) {
            return role switch {
                "Admin" => RedirectToAction("Index", "DashboardAdmin"),
                "Manager" => RedirectToAction("Index", "DashboardEmployee"),
                "Employee" => RedirectToAction("Index", "DashboardEmployee")
            };
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Start() {
            if (User.Identity != null && User.Identity.IsAuthenticated) {
                var user = await _userManager.GetUserAsync(User);
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                return role switch {
                    "Admin" => RedirectToAction("Index", "DashboardAdmin"),
                    "Manager" => RedirectToAction("Index", "DashboardEmployee"),
                    "Employee" => RedirectToAction("Index", "DashboardEmployee")
                };
            }
        
            return RedirectToAction("Login", "Auth");
        }
    }
}
