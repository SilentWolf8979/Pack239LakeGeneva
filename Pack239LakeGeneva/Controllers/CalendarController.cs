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
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Pack239LakeGeneva.Controllers
{
  public class CalendarController : Controller
  {
    private IConfiguration _configuration;

    public CalendarController(IConfiguration Configuration)
    {
      _configuration = Configuration;
    }

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
      var calendarViewModel = new CalendarViewModel();
      var calendarList = new List<string>();
      var calendarEvents = new List<CalendarEvent>();

      UserCredential credential;
      using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
      {
        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
          GoogleClientSecrets.Load(stream).Secrets,
          new[] { CalendarService.Scope.Calendar },
          "user",
          CancellationToken.None,
          new FileDataStore("Pack239LakeGeneva.Controllers"));
      }

      var service = new CalendarService(new BaseClientService.Initializer()
      {
        ApiKey = _configuration["GoogleAPIKey"],
        ApplicationName = "Pack239LakeGeneva",
        HttpClientInitializer = credential
      });

      var calendars = await service.CalendarList.List().ExecuteAsync();

      foreach (var calendar in calendars.Items)
      {
        Events events = await service.Events.List(calendar.Id).ExecuteAsync();

        foreach (var eventItem in events.Items)
        {
          CalendarEvent calendarEvent = new CalendarEvent();

          if (calendarList.IndexOf(calendar.Summary) < 0)
          {
            calendarList.Add(calendar.Summary);
          }

          if (!String.IsNullOrEmpty(eventItem.Start.DateTime.ToString()))
          {
            calendarEvent.Start = (DateTime)eventItem.Start.DateTime;
          }

          if (!String.IsNullOrEmpty(eventItem.End.DateTime.ToString()))
          {
            calendarEvent.End = (DateTime)eventItem.End.DateTime;
          }

          calendarEvent.Sequence = (int)eventItem.Sequence;
          calendarEvent.Description = eventItem.Description;
          calendarEvent.Location = eventItem.Location;
          calendarEvent.Summary = eventItem.Summary;
          calendarEvent.EventColor = calendar.BackgroundColor;
          calendarEvent.MapUrl = "https://www.google.com/maps/search/?api=1&query=" + HttpUtility.UrlEncode(eventItem.Location);

          if (eventItem.Location.IndexOf(",") >= 0)
          {
            calendarEvent.ShortLocation = eventItem.Location.Substring(0, eventItem.Location.IndexOf(","));
          }
          else
          {
            calendarEvent.ShortLocation = eventItem.Location;
          }

          if (calendar.Summary.Equals("Pack239LakeGeneva@gmail.com", StringComparison.OrdinalIgnoreCase))
          {
            calendarEvent.Calendar = "Pack";
          }
          else
          {
            calendarEvent.Calendar = calendar.Summary;
          }

          calendarEvents.Add(calendarEvent);
        }
      }

      calendarList = calendarList.Select(x => x.Replace("Pack239LakeGeneva@gmail.com", "Pack", StringComparison.OrdinalIgnoreCase)).ToList();
      calendarEvents.Sort((e1, e2) => DateTime.Compare(e1.Start, e2.Start));

      calendarViewModel.calendars = calendarList;
      calendarViewModel.calendarEvents = calendarEvents;

      return PartialView("Components/Calendar/Default", calendarViewModel);
    }
  }
}
