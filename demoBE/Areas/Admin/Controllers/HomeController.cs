using System.Web.Mvc;

namespace demoBE.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
