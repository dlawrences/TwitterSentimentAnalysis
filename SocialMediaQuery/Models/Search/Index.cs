using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMediaQuery.Models.Search
{
    public class Index
    {
        public List<DataSource> DataSources { get; set; }

        public List<int> NumberOfResults { get; set; }
    }
}