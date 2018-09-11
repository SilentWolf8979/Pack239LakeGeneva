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
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult About()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Contact()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Error()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      ErrorViewModel model = new ErrorViewModel();

      model.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

      if (model.StatusCode == (int)HttpStatusCode.OK)
      {
        model.StatusCode = (int)HttpStatusCode.NotFound;
      }
      else
      {
        model.StatusCode = HttpContext.Response.StatusCode;
      }
      
      model.StatusMessage = (model.StatusCode).ToString();
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