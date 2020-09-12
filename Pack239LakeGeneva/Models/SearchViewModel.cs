using System.Collections.Generic;

namespace Pack239LakeGeneva.Models
{
  public class SearchViewModel
  {
    public List<SearchResult> searchResults { get; set; }
    public string searchQuery { get; set; }
    public long? totalResuls { get; set; }
  }
}