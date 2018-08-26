using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pack239LakeGeneva.Controllers
{
  [Authorize]
  public class AuthController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}