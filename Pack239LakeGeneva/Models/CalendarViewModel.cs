using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pack239LakeGeneva.Models
{
  public class CalendarViewModel
  {
    public int Sequence { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
  }
}