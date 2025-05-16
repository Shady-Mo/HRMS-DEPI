using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_Project.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View("NotFound");
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            return View("ServerError");
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            if (statusCode == 404)
                return RedirectToAction("Error404");

            return RedirectToAction("Error500");
        }
    }
}
