using Google.Apis.Auth.OAuth2;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pack239LakeGeneva.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pack239LakeGeneva.Controllers
{
  public class SearchController : Controller
  {
    private IConfiguration _configuration;
    private IMemoryCache _cache;

    public SearchController(IConfiguration configuration, IMemoryCache cache)
    {
      _configuration = configuration;
      _cache = cache;
    }

    public IActionResult Index(string query)
    {
      ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

      var searchResultsList = GetSearchResultsList(query);

      return View(searchResultsList);
    }

    private CustomsearchService GetSearchService()
    {
      CustomsearchService service;

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.GoogleService, out service))
      {
        service = new CustomsearchService(new BaseClientService.Initializer()
        {
          ApiKey = _configuration["api_key"]
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

    private async Task<IList<Result>> GetSearchResultsList(string query)
    {
      IList<Result> results;

      var searchRequest = GetSearchService().Cse.List(query);
      searchRequest.Cx = "011808698875197557344:utxixcu6ama";

      var searchResults = await searchRequest.ExecuteAsync();

      results = searchResults.Items;

      return results;
    }
  }
}