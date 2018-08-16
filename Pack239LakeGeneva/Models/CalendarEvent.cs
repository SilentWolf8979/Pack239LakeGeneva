using System;

namespace Pack239LakeGeneva.Models
{
  public class CalendarEvent
  {
    public int? Sequence { get; set; }
    public string Location { get; set; }
    public string ShortLocation { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Calendar { get; set; }
    public int? CalendarSort { get; set; }
    public string EventColor { get; set; }
    public string MapUrl { get; set; }
  }
}