using Microsoft.AspNetCore.Mvc;

using Pack239LakeGeneva.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pack239LakeGeneva.Controllers
{
  public class CampfireController : Controller
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
        var skits = GetItems("skits");

        return View(skits);
      }
      else
      {
        ViewData["skitName"] = skitName;

        var lines = GetItem("skits", skitName);

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

    public IActionResult Songs(string songName)
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      if (string.IsNullOrEmpty(songName))
      {
        var songs = GetItems("songs");

        return View(songs);
      }
      else
      {
        ViewData["songName"] = songName;

        var lines = GetItem("songs", songName);

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

    public IActionResult Ceremonies(string ceremonyName)
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      if (string.IsNullOrEmpty(ceremonyName))
      {
        var ceremonies = GetItems("ceremonies");

        return View(ceremonies);
      }
      else
      {
        ViewData["ceremonyName"] = ceremonyName;

        var lines = GetItem("ceremonies", ceremonyName);

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

    private List<string> GetItems(string itemType)
    {
      List<string> skits = new List<string>();

      foreach (var file in Directory.EnumerateFiles(Path.Combine("wwwroot", "data", itemType), "*.txt"))
      {
        skits.Add(file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(".txt", "", StringComparison.OrdinalIgnoreCase));
      }

      skits.Sort();

      return skits;
    }

    private List<string> GetItem(string itemType, string skitName)
    {
      List<string> lines = new List<string>();

      if (System.IO.File.Exists(Path.Combine("wwwroot", "data", itemType, $"{skitName}.txt")))
      {
        using (StreamReader reader = new StreamReader(Path.Combine("wwwroot", "data", itemType, $"{skitName}.txt")))
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