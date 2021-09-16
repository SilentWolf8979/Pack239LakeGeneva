using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Pack239LakeGeneva.Models;
using static Google.Apis.Calendar.v3.EventsResource.ListRequest;

namespace Pack239LakeGeneva.Controllers
{
  public class CalendarController : Controller
  {
    private readonly IMemoryCache _cache;

    public CalendarController(IMemoryCache cache)
    {
      _cache = cache;
    }

    public IActionResult Index()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View();
    }

    public IActionResult Error()
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



    public async Task<IActionResult> GetCalendars()
    {
      var calendarViewModel = new CalendarViewModel();
      var calendarList = new List<Models.Calendar>();

      var calendars = await GetCalendarList();

      foreach (var calendar in calendars.Items)
      {
        Models.Calendar currentCal = new Models.Calendar();

        currentCal.Id = calendar.Id;

        currentCal.ShareUrlEmbed = "https://calendar.google.com/calendar/embed?src=" + calendar.Id;
        currentCal.ShareUrlIcs = "https://calendar.google.com/calendar/ical/" + calendar.Id + "/public/basic.ics";

        switch (calendar.Summary)
        {
          case "pack239lakegeneva@gmail.com":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=cGFjazIzOWxha2VnZW5ldmFAZ21haWwuY29t";
            break;
          case "Indian Trails District":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=Z2U5dGh0YWcxdWlhdWdyZW1wbGcwMXV2dHNAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "Glacier's Edge Council":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=NW9rcmoxMm0ydjdnN2poZHNram0ydXI3M2tAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "1-Lions":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=bmN1ZThqY2UzZGFlbTlidmRhMGx2dGtsaXNAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "2-Tigers":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=ZmcxdWMyaWphNHAxYnZ2dmk5ZWVmYjFtdWdAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "3-Wolves":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=ZGlkaW0wcG1jNnE5aDZ1amdkcHA1M3ExNzRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "4-Bears":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=NDBvdDQ2bzh1bjFqM211OWRxbm1va2N0dGdAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "5-Webelos":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=czN0ZmNvdXFvMmVhNzhzY2FrdmY1c3FxbzRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "6-Arrow of Light":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=amJlbHJkaGQxNWdlbGMyNjcxMXNjNnQxaDRAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
          case "7-OldAOL":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=dHQxbTM0dnJpZXNhNXNocWZhNmc1bDNoY3NAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
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
          currentCal.Sequence = 97;
          currentCal.Summary = "Pack";
        }
        else if (calendar.Summary.Equals("Indian Trails District", StringComparison.OrdinalIgnoreCase))
        {
          currentCal.Sequence = 98;
          currentCal.Summary = "District";
        }
        else if (calendar.Summary.Equals("Glacier's Edge Council", StringComparison.OrdinalIgnoreCase))
        {
          currentCal.Sequence = 99;
          currentCal.Summary = "Council";
        }
        else
        {
          currentCal.Sequence = 96;
          currentCal.Summary = calendar.Summary;
        }

        calendarList.Add(currentCal);
      }

      calendarList = calendarList.OrderBy(x => x.Sequence).ToList();

      calendarViewModel.calendars = calendarList;

      return PartialView("Components/Calendar/Calendars", calendarViewModel);
    }

    public async Task<IActionResult> GetEvents()
    {
      var calendarViewModel = new CalendarViewModel();
      var calendarEvents = new List<CalendarEvent>();

      var calendars = await GetCalendarList();

      foreach (var calendar in calendars.Items)
      {
        Events events = await GetCalendarEvents(calendar.Id);

        foreach (var eventItem in events.Items)
        {
          CalendarEvent calendarEvent = new CalendarEvent();

          if (!String.IsNullOrEmpty(eventItem.Start.Date))
          {
            calendarEvent.Start = DateTime.Parse(eventItem.Start.Date);
            calendarEvent.End = calendarEvent.Start;
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
          calendarEvent.Location = eventItem.Location;
          calendarEvent.Summary = eventItem.Summary;
          calendarEvent.EventColor = calendar.BackgroundColor;
          calendarEvent.MapUrl = "https://www.google.com/maps/search/?api=1&query=" + HttpUtility.UrlEncode(eventItem.Location);

          if (!String.IsNullOrEmpty(eventItem.Description))
          {
            calendarEvent.Description = eventItem.Description.Replace("target=\"_blank\"", "target=\"_blank\" rel=\"noopener\" ");
          }
          else
          {
            calendarEvent.Description = eventItem.Description;
          }

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
            calendarEvent.CalendarSort = 97;
          }
          else if (calendar.Summary.Equals("Indian Trails District", StringComparison.OrdinalIgnoreCase))
          {
            calendarEvent.Calendar = "District";
            calendarEvent.CalendarSort = 98;
          }
          else if (calendar.Summary.Equals("Glacier's Edge Council", StringComparison.OrdinalIgnoreCase))
          {
            calendarEvent.Calendar = "Council";
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

      calendarEvents = calendarEvents.OrderBy(s => s.Start).ThenBy(c => c.CalendarSort).ToList();

      calendarViewModel.calendarEvents = calendarEvents;

      return PartialView("Components/Calendar/Events", calendarViewModel);
    }

    private CalendarService GetCalendarService()
    {
      CalendarService service;

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.GoogleService, out service))
      {
        // Key not in cache, so get data.
        var json = System.IO.File.ReadAllText("client_secrets.json");
        JObject cr = (JObject)JsonConvert.DeserializeObject(json);

        var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(cr.GetValue("client_email").ToString())
        {
          Scopes = new[] {
            CalendarService.Scope.Calendar
          }
        }.FromPrivateKey(cr.GetValue("private_key").ToString()));

        service = new CalendarService(new BaseClientService.Initializer()
        {
          HttpClientInitializer = credential
        });

        // Set cache options.
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(1));

        // Save data in cache.
        _cache.Set(CacheKeys.GoogleService, service, cacheEntryOptions);
      }

      return service;
    }

    private async Task<CalendarList> GetCalendarList()
    {
      CalendarList calendars;

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.Calendars, out calendars))
      {
        // Key not in cache, so get data.
        calendars = await GetCalendarService().CalendarList.List().ExecuteAsync();
        
        // Set cache options.
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(1));

        // Save data in cache.
        _cache.Set(CacheKeys.Calendars, calendars, cacheEntryOptions);
      }

      return calendars;
    }

    private async Task<Events> GetCalendarEvents(string calendarId)
    {
      Events events;

      // Look for cache key.
      if (!_cache.TryGetValue($"{CacheKeys.Events}_{calendarId}", out events))
      {
        // Key not in cache, so get data.
        var cal = GetCalendarService().Events.List(calendarId);

        cal.OrderBy = OrderByEnum.StartTime;
        cal.ShowDeleted = false;
        cal.SingleEvents = true;
        cal.TimeMin = DateTime.Today;

        events = await cal.ExecuteAsync();

        // Set cache options.
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromHours(1));

        // Save data in cache.
        _cache.Set($"{CacheKeys.Events}_{calendarId}", events, cacheEntryOptions);
      }

      return events;
    }

    private void RegisterCalendar()
    {
      CalendarListEntry cle = new CalendarListEntry();
      // This is the ID of a new calendar that we want to show on the site
      cle.Id = "fg1uc2ija4p1bvvvi9eefb1mug@group.calendar.google.com";
      CalendarListEntry cleNew = GetCalendarService().CalendarList.Insert(cle).Execute();
    }
  }
}