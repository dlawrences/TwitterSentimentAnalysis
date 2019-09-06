﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialMediaQuery.Controllers
{
    using System.Web.Routing;

    using SocialMediaQuery.Models;
    using SocialMediaQuery.Models.Search;

    public class SearchController : Controller
    {
        private DEV_DB_LICEntities2 db = new DEV_DB_LICEntities2();

        public ActionResult Index()
        {
            var model = new Index();

            model.DataSources = db.DataSources.ToList();

            return this.View(model);
        }

        public ActionResult RouteToController(string query, int dataSourceId, int maxNumberOfResults)
        {

            switch (dataSourceId)
            {
                case 1:
                    return this.RedirectToAction("TwitterSearch", "Results", new { query = query, maxNumberOfResults = maxNumberOfResults, dataSourceId = dataSourceId});
                /*case 2:
                    return this.RedirectToAction("InstagramAuth", "Results", new { query = query });
                case 3:
                    return this.RedirectToAction("GooglePlusSearch", "Results", new { query = query });
                case 4:
                    return this.RedirectToAction("YouTubeSearch", "Results", new { query = query });*/
            }

            return this.RedirectToAction("Index");
        }
    }
}