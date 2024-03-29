﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Pack239LakeGeneva.Models;

namespace Pack239LakeGeneva.Controllers
{
  public class ResourcesController : Controller
  {
    public ResourcesController()
    {
    }

    public IActionResult Index()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Leaders()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      var leaderList = GetLeaders();

      return View(leaderList);
    }

    public IActionResult Uniforms()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Documents(string documentId)
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;
      ViewData["documentId"] = documentId;

      return View();
    }

    public IActionResult Error()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private List<Leader> GetLeaders()
    {
      List<Leader> leaders = new List<Leader>();

      string leaderData = System.IO.File.ReadAllText(Path.Combine("wwwroot", "data", "leaders.json"));
      JToken token = JObject.Parse(leaderData);

      foreach (JToken group in token.SelectToken("groups").Children())
      {
        foreach (JToken member in group.SelectToken("members").Children())
        {
          Leader leader = new Leader();

          if (group.SelectToken("title") != null)
          {
            leader.Group = group.SelectToken("title").ToString();
          }

          if (member.SelectToken("position") != null)
          {
            leader.Position = member.SelectToken("position").ToString();
          }

          if (member.SelectToken("name") != null)
          {
            leader.Name = member.SelectToken("name").ToString();
          }

          if (member.SelectToken("phone") != null)
          {
            leader.Phone = member.SelectToken("phone").ToString();
          }

          if (member.SelectToken("email") != null)
          {
            leader.EMail = member.SelectToken("email").ToString();
          }

          leaders.Add(leader);
        }
      }

      return leaders;
    }

    public async Task<IActionResult> GetDocuments(string documentId)
    {
      string webFolder = "1IjWXjO_2rthBbp77-FsPXp1im7yxTjkw";
      var documentViewModel = new DocumentViewModel();
      var breadcrumb = -1;
      var breadcrumbList = new List<Tuple<string, string>>();
      var folderList = new List<Document>();
      var documentList = new List<Document>();

      var json = System.IO.File.ReadAllText("client_secrets.json");
      JObject cr = (JObject)JsonConvert.DeserializeObject(json);

      var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.GetValue("client_email").ToString())
      {
        Scopes = new[] {
            DriveService.Scope.Drive
        }
      }.FromPrivateKey(cr.GetValue("private_key").ToString()));

      var service = new DriveService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential
      });

      try
      {
        if (String.IsNullOrEmpty(documentId))
        {
          documentId = webFolder;
        }

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.Q = "'" + documentId + "' in parents";
        listRequest.Fields = "nextPageToken, files(fileExtension, iconLink, id, mimeType, name, parents, thumbnailLink, webContentLink, webViewLink)";
        listRequest.OrderBy = "name";

        var fileList = await listRequest.ExecuteAsync();
        var parentFile = fileList.Files[0];

        breadcrumbList.Add(new Tuple<string, string>(parentFile.Parents[0], ""));

        while ((parentFile.Parents != null) && (parentFile.Parents.Count > 0) && !parentFile.Parents[0].Equals(webFolder))
        {
          FilesResource.GetRequest parentRequest = service.Files.Get(parentFile.Parents[0]);
              
          parentRequest.Fields = "id, name, parents";

          parentFile = await parentRequest.ExecuteAsync();

          if (parentFile.Parents != null)
          {
            breadcrumb = breadcrumbList.IndexOf(new Tuple<string, string>(parentFile.Id, ""));

            if (breadcrumb >= 0)
            {
              breadcrumbList[breadcrumb] = new Tuple<string, string>(parentFile.Id, parentFile.Name);
            }

            breadcrumbList.Add(new Tuple<string, string>(parentFile.Parents[0], ""));
          }
        }

        breadcrumb = breadcrumbList.IndexOf(new Tuple<string, string>(webFolder, ""));

        if (breadcrumb >= 0)
        {
          breadcrumbList[breadcrumb] = new Tuple<string, string>(webFolder, "Home");
        }

        breadcrumbList.Reverse();

        foreach (var file in fileList.Files)
        {
          Document currentDoc = new Document();

          currentDoc.FileExtension = file.FileExtension;
          currentDoc.IconLink = file.IconLink;
          currentDoc.Id = file.Id;
          currentDoc.MimeType = file.MimeType;
          currentDoc.Name = file.Name;
          currentDoc.Parents = file.Parents;
          currentDoc.ThumbnailLink = "https://drive.google.com/thumbnail?authuser=0&sz=s200&id=" + currentDoc.Id;
          currentDoc.WebContentLink = file.WebContentLink;
          currentDoc.WebViewLink = file.WebViewLink;

          if (currentDoc.MimeType.Equals("application/vnd.google-apps.folder", StringComparison.OrdinalIgnoreCase))
          {
            folderList.Add(currentDoc);
          }
          else
          {
            documentList.Add(currentDoc);
          }
        }
      }
      catch
      {
        // Ignore this exception...cause it's a Google issue
      }

      documentViewModel.breadcrumbs = breadcrumbList;
      documentViewModel.folders = folderList;
      documentViewModel.documents = documentList;

      return PartialView("Components/Resources/Default", documentViewModel);
    }
  }
}
