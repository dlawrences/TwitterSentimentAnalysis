using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaAdapters
{
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;

    using LinqToTwitter;

    public static class TwitterAdapter
    {
        public static string ConsumerKey;

        public static string ConsumerSecret;

        public static async Task<List<Status>> SearchAsync(string query, int maxNumberOfResults)
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore =
                    new SessionStateCredentialStore()
                    {
                        ConsumerKey = ConsumerKey,
                        ConsumerSecret = ConsumerSecret,
                    }
            };

            await auth.AuthorizeAsync();

            string newQuery = string.Concat(query, "-filter:retweets AND -filter:replies");

            var twitterCtx = new TwitterContext(auth);

            var combinedSearchResults = new List<Status>();

            const int MaxTweetsToReturn = 100;

            ulong sinceID = 1;

            ulong maxID;

            List<Status> searchResults =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == newQuery &&
                       search.SinceID == sinceID &&
                       search.Count == MaxTweetsToReturn &&
                       search.SearchLanguage == "en"
                 select search.Statuses)
                .SingleOrDefault();

            var serializedResults = new StringBuilder();

            if (searchResults != null && searchResults.Count > 0)
            {
                combinedSearchResults.AddRange(searchResults);

                ulong previousMaxID = ulong.MaxValue;

                do
                {
                    maxID = searchResults.Min(result => result.ID) - 1;

                    //Debug.Assert(maxID < previousMaxID);

                    previousMaxID = maxID;

                    searchResults = 
                        (from search in twitterCtx.Search
                              where search.Type == SearchType.Search &&
                                    search.Query == newQuery &&
                                    search.SinceID == sinceID &&
                                    search.MaxID == maxID &&
                                    search.Count == MaxTweetsToReturn &&
                                    search.SearchLanguage == "en"
                         select search.Statuses)
                        .SingleOrDefault();

                    //List<Status> filteredResults =
                    //    (from result in searchResults
                    //     where result.Coordinates.Latitude != 0.00 && result.Coordinates.Longitude != 0.00
                    //     select result).ToList();

                    combinedSearchResults.AddRange(searchResults);
                } while (searchResults.Any() && combinedSearchResults.Count < maxNumberOfResults);

                //foreach (var result in combinedSearchResults)
                //{
                //    serializedResults.AppendFormat(
                //        "<blockquote class=\"twitter-tweet\"><p>{0}</p><p><a href=\"{1}\"><img src=\"{2}\"/>{3}</a> - {4}</p></blockquote>",
                //        result.Text,
                //        result.User.Url,
                //        result.User.ProfileImageUrl,
                //        result.User.ScreenNameResponse,
                //        result.CreatedAt);
                //}
            }

            return combinedSearchResults;
        }
    }
}
