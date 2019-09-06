using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesAdapters
{
    public class TweetSentiment
    {
        public string Id { get; set; }
        public string TweetText { get; set;}
        public Nullable<double> Score { get; set; }
        public string KeyPhrases { get; set; }

        public TweetSentiment(string id, string tweetText)
        {
            Id = id;
            TweetText = tweetText;
        }
    }
}
