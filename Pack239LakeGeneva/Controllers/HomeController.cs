using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;

using Pack239LakeGeneva.Models;

namespace Pack239LakeGeneva.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult About()
    {
      return View();
    }

    public IActionResult Contact()
    {
      return View();
    }

    public IActionResult Error()
    {
      ErrorViewModel model = new ErrorViewModel();

      model.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
      model.StatusCode = HttpContext.Response.StatusCode;
      model.StatusMessage = ((HttpStatusCode)HttpContext.Response.StatusCode).ToString();
      model.StatusMessage = Regex.Replace(model.StatusMessage, "(\\B[A-Z])", " $1");

      switch (model.StatusCode)
      {
        case 401:
          model.ErrorHeadline = "A Scout is Trustworthy";
          model.ErrorText = "This page requires you to <a href='/Account/Login'>login</a>. Please be sure you have an account and have entered the correct user ID and password.";
          break;
        case 404:
          model.ErrorHeadline = "A Scout is Helpful";
          model.ErrorText = "But despite our best efforts, we couldn't find what you were looking for!";
          break;
        case 500:
          model.ErrorHeadline = "A Scout is Courteous";
          model.ErrorText = "Something went wrong. Please try again, and if it continues please be courteous and <a href='/Contact'>let us know</a> about it!";
          break;
        default:
          model.ErrorHeadline = "A Scout is Helpful";
          model.ErrorText = "But despite our best efforts, we couldn't find what you were looking for!";
          break;
      }

      return View(model);
    }
  }
}