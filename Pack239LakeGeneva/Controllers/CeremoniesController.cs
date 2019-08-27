using Microsoft.AspNetCore.Mvc;

using Pack239LakeGeneva.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Pack239LakeGeneva.Controllers
{
  public class CeremoniesController : Controller
  {
    public IActionResult Index()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Skits(string skitName)
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      if (string.IsNullOrEmpty(skitName))
      {
        var skits = GetSkits();

        return View(skits);
      }
      else
      {
        ViewData["skitName"] = skitName;

        var lines = GetSkit(skitName);

        if (lines.Count > 0)
        {
          return View(lines);
        }
        else
        {
          return NotFound();
        }
      }
    }

    public IActionResult Error()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private List<string> GetSkits()
    {
      List<string> skits = new List<string>();

      foreach (var file in Directory.EnumerateFiles(@"wwwroot\data\skits\", "*.txt"))
      {
        skits.Add(file.Substring(file.LastIndexOf(@"\") + 1).Replace(".txt", "", StringComparison.OrdinalIgnoreCase));
      }

      return skits;
    }

    private List<string> GetSkit(string skitName)
    {
      List<string> lines = new List<string>();

      if (System.IO.File.Exists($@"wwwroot\data\skits\{skitName}.txt"))
      {
        using (StreamReader reader = new StreamReader($@"wwwroot\data\skits\{skitName}.txt"))
        {
          while (reader.Peek() >= 0)
          {
            lines.Add(reader.ReadLine());
          }
        }
      }

      return lines;
    }
  }
}