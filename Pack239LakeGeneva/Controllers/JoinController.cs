﻿using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Pack239LakeGeneva.Models;

namespace Pack239LakeGeneva.Controllers
{
  public class JoinController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
