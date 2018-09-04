using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Pack239LakeGeneva.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using static Google.Apis.Calendar.v3.EventsResource.ListRequest;

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
      var calendarList = new List<Models.Calendar>();
      var calendarEvents = new List<CalendarEvent>();

      var json = System.IO.File.ReadAllText("client_secrets.json");
      JObject cr = (JObject)JsonConvert.DeserializeObject(json);
 
      var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.GetValue("client_email").ToString())
      {
        Scopes = new[] {
            CalendarService.Scope.Calendar
        }
      }.FromPrivateKey(cr.GetValue("private_key").ToString()));

      var service = new CalendarService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential
      });

      var calendars = await service.CalendarList.List().ExecuteAsync();

      foreach (var calendar in calendars.Items)
      {
        var cal = service.Events.List(calendar.Id);

        cal.MaxResults = 3;
        cal.OrderBy = OrderByEnum.StartTime;
        cal.ShowDeleted = false;
        cal.SingleEvents = true;
        cal.TimeMin = DateTime.Today;

        Models.Calendar currentCal = new Models.Calendar();

        currentCal.Id = calendar.Id;

        currentCal.ShareUrlEmbed = "https://calendar.google.com/calendar/embed?src=" + calendar.Id;
        currentCal.ShareUrlIcs = "https://calendar.google.com/calendar/ical/" + calendar.Id + "/public/basic.ics";

        switch (calendar.Summary)
        {
          case "pack239lakegeneva@gmail.com":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=cGFjazIzOWxha2VnZW5ldmFAZ21haWwuY29t";
            break;
          case "1-Lions":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=NDBvdDQ2bzh1bjFqM211OWRxbm1va2N0dGdAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "2-Tigers":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=czN0ZmNvdXFvMmVhNzhzY2FrdmY1c3FxbzRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "3-Wolves":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=amJlbHJkaGQxNWdlbGMyNjcxMXNjNnQxaDRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "4-Bears":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=dHQxbTM0dnJpZXNhNXNocWZhNmc1bDNoY3NAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "5-Webelos":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=bmFzNW9uY2hwc3Y0aHVyN2pzc29vcWRhMzRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "6-Arrow of Light":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=ZGlkaW0wcG1jNnE5aDZ1amdkcHA1M3ExNzRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "Committee":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=dnBmcHU0bW5sdTRtdWJscWgxdHZlOXE2ZzhAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
        }
        

        if (calendar.Summary.IndexOf("-") >= 0)
        {
          currentCal.Sequence = Int32.Parse(calendar.Summary.Substring(0, calendar.Summary.IndexOf("-")));
          currentCal.Summary = calendar.Summary.Substring(calendar.Summary.IndexOf("-") + 1);
        }
        else if (calendar.Summary.Equals("Pack239LakeGeneva@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
          currentCal.Sequence = 99;
          currentCal.Summary = "Pack";

          cal.MaxResults = 5;
        }
        else
        {
          currentCal.Sequence = 98;
          currentCal.Summary = calendar.Summary;
        }

        calendarList.Add(currentCal);

       
        
        Events events = await cal.ExecuteAsync();

        foreach (var eventItem in events.Items)
        {
          CalendarEvent calendarEvent = new CalendarEvent();

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

          if ((!String.IsNullOrEmpty(eventItem.Location)) && (eventItem.Location.IndexOf(",") >= 0))
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
            calendarEvent.CalendarSort = 99;
          }
          else
          {
            if (calendar.Summary.Contains("-"))
            {
              calendarEvent.Calendar = calendar.Summary.Substring(calendar.Summary.IndexOf("-") + 1);
              calendarEvent.CalendarSort = Int32.Parse(calendar.Summary.Substring(0, calendar.Summary.IndexOf("-")));
            }
            else
            {
              calendarEvent.Calendar = calendar.Summary;
            }
          }

          calendarEvent.EventColor = calendarEvent.Calendar.Replace(" ", "");

          calendarEvents.Add(calendarEvent);
        }
      }

      calendarList = calendarList.OrderBy(x => x.Sequence).ToList();
      calendarEvents = calendarEvents.OrderBy(s => s.Start).ThenBy(c => c.CalendarSort).ToList();

      calendarViewModel.calendars = calendarList;
      calendarViewModel.calendarEvents = calendarEvents;

      return PartialView("Components/Calendar/Default", calendarViewModel);
    }
  }
}