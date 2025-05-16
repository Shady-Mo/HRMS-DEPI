using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace DEPI_Project.Controllers {
    public class PayrollController: Controller {
        private readonly CorpMgmt_SystemContext _context;

        public PayrollController(CorpMgmt_SystemContext context) {
            _context = context;
        }
        public async Task<IActionResult> ProcessMonthlySalaries(string? employeeId = null) {

            var employee = _context.Users
                .OfType<Employee>()
                .Where(e => e.Id == employeeId)
                .Include(e => e.Attendances)
                .Include(e => e.Leaves)
                .FirstOrDefault();

            var today = DateTime.UtcNow;
            var currentMonth = new DateTime(today.Year, today.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

            var monthAttendances = employee.Attendances?
                    .Where(a => a.Date >= currentMonth && a.Date < currentMonth.AddMonths(1))
                    .ToList() ?? new List<Attendance>();

            int presentDays = monthAttendances.Count(a => a.Status == Status.Present);

            var allDays = Enumerable.Range(1, daysInMonth)
                .Select(d => new DateTime(today.Year, today.Month, d))
                .ToList();

            var absentDays = allDays
                .Where(day => !monthAttendances.Any(a => a.Date.Date == day.Date))
                .ToList();

            var leaveDays = employee.Leaves?
                .Where(l => l.StartDate <= currentMonth.AddMonths(1) && l.EndDate >= currentMonth)
                .SelectMany(l => Enumerable.Range(0, (l.EndDate - l.StartDate).Days + 1)
                    .Select(offset => l.StartDate.AddDays(offset).Date))
                .Distinct()
                .ToList() ?? new List<DateTime>();

            int actualAbsentDays = absentDays
                .Count(d => !leaveDays.Contains(d.Date));

            decimal dayRate = employee.Salary / daysInMonth;
            decimal totalDeduction = actualAbsentDays * dayRate;
            decimal netPay = employee.Salary - totalDeduction;

            netPay = 1000;

            var payment = new Payment {
                EmployeeId = employee.Id,
                AppUserId = employee.Id,
                PaymentDate = today,
                Amount = netPay,
                Deductions = totalDeduction,
                Bonuses = 0,
                PaymentStaus = PaymentStaus.Paid
            };

            var accountService = new AccountService();
            var account = await accountService.GetAsync(employee.StripeAccountId);
            bool isEnabled = account.ChargesEnabled == true &&
                             account.PayoutsEnabled == true &&
                             account.Capabilities.Transfers == "active";

            if (isEnabled && netPay > 0) {
                var paymentIntentService = new PaymentIntentService();
                await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions {
                    Amount = (long) (netPay * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Description = $"Salary payment for {employee.FirstName} {employee.LastName}",
                    TransferData = new PaymentIntentTransferDataOptions {
                        Destination = employee.StripeAccountId
                    },
                    Confirm = true,
                    PaymentMethod = "pm_card_visa"
                });

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }
            else {
                TempData["Error"] = "Employee's Stripe account is not enabled for payouts.";
                return RedirectToAction("Index", "Employee");
            }

            TempData["Success"] = "Payments have been transferred successfully";
            return RedirectToAction("Index", "Employee");
        }

    }
}
