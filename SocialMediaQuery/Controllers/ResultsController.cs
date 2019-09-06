using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SocialMediaQuery.Controllers
{
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Threading.Tasks;

    using InstaSharp;

    using SocialMediaAdapters;
    using SocialMediaQuery.Models;
    using SocialMediaQuery.Models.Results;
    using CognitiveServicesAdapters;
    using System.Configuration;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using Microsoft.PowerBI.Api.V1;
    using System.Net;
    using System.IO;

    public class ResultsController : AsyncController
    {

        [ActionName("TwitterSearch")]
        public async Task<ActionResult> TwitterSearchAsync(string query, int maxNumberOfResults, int dataSourceId)
        {
            TwitterAdapter.ConsumerKey = System.Web.HttpContext.Current.Application["TwitterConsumerKey"].ToString();
            TwitterAdapter.ConsumerSecret = System.Web.HttpContext.Current.Application["TwitterConsumerSecret"].ToString();

            var results = await TwitterAdapter.SearchAsync(query, maxNumberOfResults);

            var json = JsonConvert.SerializeObject(results);

            DEV_DB_LICEntities2 db = new DEV_DB_LICEntities2();

            UserSearch usearch = new UserSearch() { QueryText = query, DataSourceId = dataSourceId, JsonResult = json, CreatedOn = DateTime.Now};

            db.UserSearches.Add(usearch);

            /*var model = new TwitterSearch() { Query = query, ResultsXml = results };*/

            db.SaveChanges();

            db.Database.ExecuteSqlCommand("usp_ImportTweetsBySearchId @param1", new SqlParameter("param1", usearch.Id));
             
            var resultsList = db.SearchResults.Where(r => r.UserSearchId == usearch.Id);

            List<TweetSentiment> tweetSentiments = new List<TweetSentiment>();

            foreach (var r in resultsList)
            {
                tweetSentiments.Add(new TweetSentiment(r.Id.ToString(),r.TweetText));
            }

            string apiKey = System.Web.HttpContext.Current.Application["CognitiveServicesKey"].ToString();

             var sentimentResults = await SentimentAnalyzer.AnalyzeAsync(tweetSentiments, apiKey);

            foreach (var s in sentimentResults)
            {
                var recordToUpdate = db.SearchResults.SingleOrDefault(r => r.Id.ToString() == s.Id);
                if (recordToUpdate != null)
                {
                    recordToUpdate.Score = s.Score;
                    recordToUpdate.KeyPhrases = s.KeyPhrases;
                }
            }

            db.SaveChanges();

            return this.RedirectToAction("Details", "UserSearches", new { id = usearch.Id });
        }

        [ActionName("InstagramAuth")]
        public async Task<ActionResult> InstagramAuthAsync(string query)
        {
            var consumerKey = System.Web.HttpContext.Current.Application["InstagramClientKey"].ToString();
            var consumerSecret = System.Web.HttpContext.Current.Application["InstagramClientSecret"].ToString();
            var redirecturl = this.Request.Url.AbsoluteUri.Replace("InstagramAuth", "InstagramSearch");
            redirecturl = redirecturl.Replace(query, "?query=" + query);
            InstagramAdapter.InstagramConfig = new InstagramConfig(
                consumerKey,
                consumerSecret,
                redirecturl,
                string.Empty);

            return this.Redirect(InstagramAdapter.GetLoginLink());
        }
        
        [ActionName("InstagramSearch")]
        public async Task<ActionResult> InstagramSearchAsync(string query, string code)
        {
            var model = new InstagramSearch() { Query = query, ResultsXml = string.Empty };

            if (string.IsNullOrEmpty(query))
            {
                return this.View(model);
            }

            query = query.Replace(" ", string.Empty);
            
            var oauthResponse = InstagramAdapter.GetOAuthResponse(code);
            
            if (oauthResponse == null || oauthResponse.User == null || oauthResponse.AccessToken == null)
            {
                return this.View(model);
            }

            if (query.StartsWith("lat:", StringComparison.InvariantCulture))
            {
                if (!query.Contains(',') || !query.ToLower().Contains("lat:") || !query.ToLower().Contains("lon:"))
                {
                    return this.View(model);
                }

                var coordinates = query.Split(',');
                var lat = Convert.ToDouble(coordinates[0].Replace("lat:", string.Empty), CultureInfo.InvariantCulture);
                var lon = Convert.ToDouble(coordinates[1].Replace("lon:", string.Empty), CultureInfo.InvariantCulture);
                model.ResultsXml = await InstagramAdapter.SearchAsync(lat, lon, oauthResponse);
            }
            else
            {
                model.ResultsXml = await InstagramAdapter.TagsAsync(query, oauthResponse);
            }

            return this.View(model);
        }

        public ActionResult GooglePlusSearch(string query)
        {
            var apiKey = System.Web.HttpContext.Current.Application["GoogleAPIKey"].ToString();
            GoogleAdapter.ApiKey = apiKey;

            var model = new GooglePlusSearch() { Query = query, ResultsXml = string.Empty };

            if (string.IsNullOrEmpty(query))
            {
                return this.View(model);
            }

            model.ResultsXml = GoogleAdapter.SearchGooglePlus(query);

            return this.View(model);
        }

        public ActionResult YouTubeSearch(string query)
        {
            var apiKey = System.Web.HttpContext.Current.Application["GoogleAPIKey"].ToString();
            GoogleAdapter.ApiKey = apiKey;

            var model = new YouTubeSearch() { Query = query, ResultsXml = string.Empty };

            if (string.IsNullOrEmpty(query))
            {
                return this.View(model);
            }

            model.ResultsXml = GoogleAdapter.SearchYouTube(query);

            return this.View(model);
        }

        public ActionResult RedirectToReport()
        {
            return Redirect("https://app.powerbi.com/groups/me/reports/5ff3f7d9-ee08-444e-a669-5be0f50aa674/ReportSection976e31ccf6b26e138cf5");
        }
    }
}