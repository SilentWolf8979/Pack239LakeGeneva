using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pack239LakeGeneva.Models;

namespace Pack239LakeGeneva.Controllers
{
  public class ResourcesController : Controller
  {
    public IActionResult Index()
    {
      ViewData["Message"] = "Your resources page.";

      return View();
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
