using System;
using System.Collections.Generic;

namespace Pack239LakeGeneva.Models
{
  public class Document
  {
    public int? Sequence { get; set; }
    public string FileExtension { get; set; }
    public string IconLink { get; set; }
    public string Id { get; set; }
    public string MimeType { get; set; }
    public string Name { get; set; }
    public IList<string> Parents { get; set; }
    public string ThumbnailLink { get; set; }
    public string WebContentLink { get; set; }
    public string WebViewLink { get; set; }
  }
}