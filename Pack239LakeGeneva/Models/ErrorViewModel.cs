using System;

namespace Pack239LakeGeneva.Models
{
  public class ErrorViewModel
  {
    public string RequestId { get; set; }
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public string ErrorHeadline { get; set; }
    public string ErrorText { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
  }
}