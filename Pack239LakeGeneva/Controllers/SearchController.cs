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

    public IActionResult Index()
    {
      return View();
    }

    // This is commented out as we're using the Google-provided search box and search results

    //public async Task<IActionResult> Index(string query, int pageNumber)
    //{
    //  var searchViewModel = new SearchViewModel();
    //  var searchResults = new List<SearchResult>();

    //  ViewData["CurrentUrl"] = Request.Scheme + "://" + Request.Host.Value + Request.Path.Value;

    //  var searchResultsList = await GetSearchResultsList(query, pageNumber);

    //  foreach (var searchResultItem in searchResultsList.Items)
    //  {
    //    SearchResult searchResult = new SearchResult();

    //    searchResult.FormattedUrl = searchResultItem.FormattedUrl;
    //    searchResult.HtmlSnippet = searchResultItem.HtmlSnippet;
    //    searchResult.HtmlTitle = searchResultItem.HtmlTitle;
    //    searchResult.HtmlTitle = searchResultItem.Title;

    //    searchResults.Add(searchResult);
    //  }

    //  searchViewModel.searchResults = searchResults;
    //  searchViewModel.searchQuery = query;
    //  searchViewModel.totalResuls = searchResultsList.SearchInformation.TotalResults;

    //  return View(searchViewModel);
    //}

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

    private async Task<Search> GetSearchResultsList(string query, int pageNumber)
    {
      Search searchResults;

      var searchRequest = GetSearchService().Cse.List(query);
      searchRequest.Cx = "011808698875197557344:utxixcu6ama";

      if (pageNumber == 0)
      {
        searchRequest.Start = 1;
      }
      else
      {
        searchRequest.Start = pageNumber * 10 - 9;
      }

      searchResults = await searchRequest.ExecuteAsync();

      return searchResults;
    }
  }
}