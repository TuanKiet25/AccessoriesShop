using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
