using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pack239LakeGeneva.Models
{
  public class CalendarViewModel
  {
    public int sequence { get; set; }
    public string location { get; set; }
    public string description { get; set; }
    public string summary { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
  }
}