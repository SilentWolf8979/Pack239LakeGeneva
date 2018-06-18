using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pack239LakeGeneva.Models;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace Pack239LakeGeneva.Controllers
{
  public class CalendarController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> GetEvents()
    {
      string ClientID = "854586420351-fsk8kc7t1k6pg6vut9nh12faftodn161.apps.googleusercontent.com";
      string ClientSecret = "Q11OHXliR_3viOaQ5qZ4yMf-";
      string APIKey = "AIzaSyAWkmarUCn_q7hD-HV7LAwm8f6XVCwjCHk";







      //ServiceAccountCredential credential;
      //string[] Scopes = { CalendarService.Scope.CalendarReadonly };
      //string ApplicationName = "Google Calendar API .NET Quickstart";

      //using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
      //{
      //  credential = GoogleCredential.FromStream(stream)
      //                                   .CreateScoped(new[] { CalendarService.Scope.Calendar })
      //                                   .UnderlyingCredential as ServiceAccountCredential;

      //  //profit
      //}

      //UserCredential credential;

      //using (var stream =
      //    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
      //{
      //  string credPath = System.Environment.GetFolderPath(
      //      System.Environment.SpecialFolder.Personal);
      //  credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

      //  credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
      //      GoogleClientSecrets.Load(stream).Secrets,
      //      Scopes,
      //      "user",
      //      CancellationToken.None,
      //      new FileDataStore(credPath, true)).Result;
      //  Console.WriteLine("Credential file saved to: " + credPath);
      //}

      // Create Google Calendar API service.
      //var service = new CalendarService(new BaseClientService.Initializer()
      //{
      //  HttpClientInitializer = credential,
      //  ApplicationName = ApplicationName,
      //});

      //// Define parameters of request.
      //EventsResource.ListRequest request = service.Events.List("primary");
      //request.TimeMin = DateTime.Now;
      //request.ShowDeleted = false;
      //request.SingleEvents = true;
      //request.MaxResults = 10;
      //request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

      var calendarEvents = new List<CalendarViewModel>();

      var service = new CalendarService(new BaseClientService.Initializer()
      {
        ApiKey = APIKey,
        ApplicationName = "Pack239LakeGeneva",

      });

      Events events = await service.Events.List("pack239lakegeneva@gmail.com").ExecuteAsync();
      //// List events.
      //Events events = request.Execute();
      Console.WriteLine("Upcoming events:");
      if (events.Items != null && events.Items.Count > 0)
      {
        foreach (var eventItem in events.Items)
        {
          CalendarViewModel calendarEvent = new CalendarViewModel();

          if (!String.IsNullOrEmpty(eventItem.Start.DateTime.ToString()))
          {
            calendarEvent.start = (DateTime)eventItem.Start.DateTime;
          }

          if (!String.IsNullOrEmpty(eventItem.End.DateTime.ToString()))
          {
            calendarEvent.end = (DateTime)eventItem.End.DateTime;
          }

          calendarEvent.sequence = (int)eventItem.Sequence;
          calendarEvent.description = eventItem.Description;
          calendarEvent.location = eventItem.Location;
          calendarEvent.summary = eventItem.Summary;

          calendarEvents.Add(calendarEvent);

          //string when = eventItem.Start.DateTime.ToString();
          //if (String.IsNullOrEmpty(when))
          //{
          //  when = eventItem.Start.Date;
          //}
          //Console.WriteLine("{0} ({1})", eventItem.Summary, when);
        }
      }
      else
      {
        Console.WriteLine("No upcoming events found.");
      }



      return PartialView("Components/Calendar/Default", calendarEvents);
    }
  }
}
