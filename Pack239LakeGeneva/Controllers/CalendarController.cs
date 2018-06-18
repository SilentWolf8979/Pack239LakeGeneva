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
      var calendarEvents = new List<CalendarViewModel>();

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
        if (calendar.Summary.IndexOf("Holiday", StringComparison.OrdinalIgnoreCase) < 0)
        {
          Events events = await service.Events.List(calendar.Id).ExecuteAsync();

          foreach (var eventItem in events.Items)
          {
            CalendarViewModel calendarEvent = new CalendarViewModel();

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

            calendarEvents.Add(calendarEvent);
          }
        }
      }

      calendarEvents.Sort((e1, e2) => DateTime.Compare(e1.Start, e2.Start));

      return PartialView("Components/Calendar/Default", calendarEvents);
    }
  }
}
