using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Pack239LakeGeneva.Models;

namespace Pack239LakeGeneva.Controllers
{
  public class JoinController : Controller
  {
    public IActionResult Index()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Error()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
