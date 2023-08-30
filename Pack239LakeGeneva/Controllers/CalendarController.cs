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

// IMPORTANT: If you create a new calendar, you need to call the RegisterCalendar method and update the ID in it to
// the ID of the new calendar that you created.  The Calendar ID is shown in Google Calendar at the bottom of the
// calendar setting page.

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
      //RegisterCalendars();
    
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
          case "Lions - Cub Scout Pack 239 Lake Geneva":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=OHI2dGNiZnR2dmRkZmQ3amNkOGJvOHVmMWdmaGg2MGhAaW1wb3J0LmNhbGVuZGFyLmdvb2dsZS5jb20";
            break;
          case "2-Tigers":
          case "Tigers - Cub Scout Pack 239 Lake Geneva":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=bDZzcThqcTJ1b2FvaGdoZ2o5dHRjMW8xdWV0cjA3cmhAaW1wb3J0LmNhbGVuZGFyLmdvb2dsZS5jb20";
            break;
          case "3-Wolves":
          case "Wolves - Cub Scout Pack 239 Lake Geneva":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=Y21rb3IxY2c2dWM5M2Z2Z24xMnFlMXVpOHVlcDNxNmpAaW1wb3J0LmNhbGVuZGFyLmdvb2dsZS5jb20";
            break;
          case "4-Bears":
          case "Bears - Cub Scout Pack 239 Lake Geneva":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=aXNma2Uydm1tc3IwcDBiazE4OG9tYmExYnVyYnZpM2ZAaW1wb3J0LmNhbGVuZGFyLmdvb2dsZS5jb20";
            break;
          case "5-Webelos":
          case "Webelos  - Cub Scout Pack 239 Lake Geneva":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=MGk3MWRxcmNlbDF1dHVvYmZ0b2ViNWE3YjBvaHVtam9AaW1wb3J0LmNhbGVuZGFyLmdvb2dsZS5jb20";
            break;
          case "6-Arrow of Light":
          case "AOLs - Cub Scout Pack 239 Lake Geneva":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=cXA5ZjRjc2x0aTZ1dnRuN3JsNXJkMDJtY2wwNGsxa3VAaW1wb3J0LmNhbGVuZGFyLmdvb2dsZS5jb20";
            break;
          case "Committee":
            currentCal.ShareUrl = "https://calendar.google.com/calendar?cid=dnBmcHU0bW5sdTRtdWJscWgxdHZlOXE2ZzhAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ";
            break;
        }
        
        if (calendar.Summary.IndexOf("-") >= 0)
        {
          int sequence;

          if (Int32.TryParse(calendar.Summary.Substring(0, calendar.Summary.IndexOf("-")), out sequence)) {
            currentCal.Sequence = sequence;
            currentCal.Summary = calendar.Summary.Substring(calendar.Summary.IndexOf("-") + 1);
          }
          else {
            currentCal.Sequence = GetCalendarSortOrder(calendar.Summary);
            currentCal.Summary = calendar.Summary.Substring(0, calendar.Summary.IndexOf("-") - 1).Trim();
          }

          if (currentCal.Summary.Equals("AOLs")){
            currentCal.Summary = "Arrow of Light";
          }
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
              calendarEvent.Calendar = calendar.Summary.Substring(0, calendar.Summary.IndexOf("-") - 1).Trim();
              calendarEvent.CalendarSort = GetCalendarSortOrder(calendar.Summary);
            }
            else
            {
              calendarEvent.Calendar = calendar.Summary;
            }

            if (calendarEvent.Calendar.Equals("AOLs")){
              calendarEvent.Calendar = "Arrow of Light";
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

    private int GetCalendarSortOrder(String calendarSummary) {
      switch (calendarSummary.Substring(0, calendarSummary.IndexOf("-")).Trim())
      {
        case "Lions":
          return 1;
        case "Tigers":
          return 2;
        case "Wolves":
          return 3;
        case "Bears":
          return 4;
        case "Webelos":
          return 5;
        case "AOLs":
          return 6;
      }

      return 0;
    }
    private void RegisterCalendars()
    {
       var calendarIdList = new List<String>();

      // This is the ID of a new calendar that we want to show on the site
      // The below IDs are the 2023-24 calendar IDs
      calendarIdList.Add("8r6tcbftvvddfd7jcd8bo8uf1gfhh60h@import.calendar.google.com"); // Lions
      calendarIdList.Add("l6sq8jq2uoaohghgj9ttc1o1uetr07rh@import.calendar.google.com"); // Tigers
      calendarIdList.Add("cmkor1cg6uc93fvgn12qe1ui8uep3q6j@import.calendar.google.com"); // Wolves
      calendarIdList.Add("isfke2vmmsr0p0bk188omba1burbvi3f@import.calendar.google.com"); // Bears
      calendarIdList.Add("0i71dqrcel1utuobftoeb5a7b0ohumjo@import.calendar.google.com"); // Webelos
      calendarIdList.Add("qp9f4cslti6uvtn7rl5rd02mcl04k1ku@import.calendar.google.com"); // Arrow of Light

      // Loop through calendar IDs and register them with our account
      foreach(String calendarId in calendarIdList) {
        CalendarListEntry cle = new CalendarListEntry();

        cle.Id = calendarId;

        CalendarListEntry cleNew = GetCalendarService().CalendarList.Insert(cle).Execute();
      }
    }
  }
}