using System;
using System.Collections.Generic;

namespace Pack239LakeGeneva.Models
{
  public class DocumentViewModel
  {
    public List<Tuple<string, string>> breadcrumbs { get; set; }
    public List<Document> documents { get; set; }
  }
}