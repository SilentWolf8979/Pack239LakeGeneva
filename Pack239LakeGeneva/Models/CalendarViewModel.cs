using System.Collections.Generic;

namespace Pack239LakeGeneva.Models
{
  public class CalendarViewModel
  {
    public List<string> calendars { get; set; }
    public List<CalendarEvent> calendarEvents { get; set; }
  }
}