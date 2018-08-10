using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Pack239LakeGeneva.Models;

namespace Pack239LakeGeneva.Controllers
{
  public class ResourcesController : Controller
  {
    private IConfiguration _configuration;

    public ResourcesController(IConfiguration Configuration)
    {
      _configuration = Configuration;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Leaders()
    {
      ViewData["Message"] = "Your application description page.";

      var leaderList = GetLeaders();

      return View(leaderList);
    }

    public IActionResult Uniforms()
    {
      ViewData["Message"] = "Have questions?  Contact us using the links below.";

      return View();
    }

    public IActionResult Documents(string documentId)
    {
      ViewData["documentId"] = documentId;

      return View();
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private List<Leader> GetLeaders()
    {
      List<Leader> leaders = new List<Leader>();

      string leaderData = System.IO.File.ReadAllText(@"wwwroot\data\leaders.json");
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
      var documentList = new List<Document>();

      UserCredential credential;
      using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
      {
        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
          GoogleClientSecrets.Load(stream).Secrets,
          new[] { DriveService.Scope.Drive },
          "user",
          CancellationToken.None,
          new FileDataStore("Pack239LakeGeneva.Documents"));
      }

      var service = new DriveService(new BaseClientService.Initializer()
      {
        ApiKey = _configuration["GoogleAPIKey"],
        ApplicationName = "Pack239LakeGeneva",
        HttpClientInitializer = credential
      });
      try
      {
        if (String.IsNullOrEmpty(documentId))
        {
          documentId = webFolder;
        }

        FilesResource.ListRequest listRequest = service.Files.List();
        //listRequest.PageSize = 10;
        listRequest.Q = "'" + documentId + "' in parents";
        listRequest.Fields = "nextPageToken, files(fileExtension, iconLink, id, mimeType, name, parents, thumbnailLink, webContentLink, webViewLink)";

        //FilesResource.GetRequest getRequest = service.f.Get(webFolder);
        //getRequest.Fields = "nextPageToken, files(fileExtension, iconLink, id, mimeType, name, thumbnailLink, webContentLink, webViewLink)";

        //var fileList = await getRequest.ExecuteAsync();
        var fileList = await listRequest.ExecuteAsync();

        foreach (var file in fileList.Files)
        {
          Document currentDoc = new Document();

          currentDoc.FileExtension = file.FileExtension;
          currentDoc.IconLink = file.IconLink;
          currentDoc.Id = file.Id;
          currentDoc.MimeType = file.MimeType;
          currentDoc.Name = file.Name;
          currentDoc.Parents = file.Parents;
          currentDoc.ThumbnailLink = file.ThumbnailLink;
          currentDoc.WebContentLink = file.WebContentLink;
          currentDoc.WebViewLink = file.WebViewLink;

          documentList.Add(currentDoc);
        }
      }
      catch (Exception ex)
      {
        string s = ex.Message;
      }
      documentViewModel.documents = documentList;

      return PartialView("Components/Resources/Default", documentViewModel);
    }
  }
}
