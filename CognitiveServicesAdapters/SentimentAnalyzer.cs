using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CognitiveServicesAdapters
{
    public static class SentimentAnalyzer
    {
        /// <summary>
        /// Container for subscription credentials. Make sure to enter your valid key.
        /// </summary>
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public static string _apiKey;

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }

            public ApiKeyServiceClientCredentials(string incomingApiKey)
            {
                _apiKey = incomingApiKey;
            }
        }

        public static List<MultiLanguageInput> ProcessToProperList(List<TweetSentiment> inputList)
        {
            List<MultiLanguageInput> multiLanguageInputs = new List<MultiLanguageInput>();

            foreach (var i in inputList)
            {
                multiLanguageInputs.Add(new MultiLanguageInput("en", i.Id, i.TweetText));
            }

            return multiLanguageInputs;
        }

        public static async Task<List<TweetSentiment>> AnalyzeAsync(List<TweetSentiment> inputList, string incomingApiKey)
        {
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials(incomingApiKey));
            client.AzureRegion = AzureRegions.Westeurope;

            if (inputList.Count() >= 100)
            {
                List<MultiLanguageInput> multiLanguageInputs = ProcessToProperList(inputList);

                try
                {

                    SentimentBatchResult batchResult = client.SentimentAsync(new MultiLanguageBatchInput(multiLanguageInputs)).Result;
                    KeyPhraseBatchResult batchResult2 = client.KeyPhrasesAsync(new MultiLanguageBatchInput(multiLanguageInputs)).Result;

                    foreach (var d in batchResult.Documents)
                    {
                        var tweet = inputList.Find(i => i.Id == d.Id);
                        tweet.Score = d.Score;
                    }

                    foreach (var d in batchResult2.Documents)
                    {
                        var tweet = inputList.Find(i => i.Id == d.Id);
                        tweet.KeyPhrases = String.Join(",", d.KeyPhrases);
                    }
                }

                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException.ToString());
                }
            }

            else {
                inputList.ForEach(delegate (TweetSentiment action)
                {
                    action.Score = 0.5;
                });
            }

            return inputList;
        }
    }
}
