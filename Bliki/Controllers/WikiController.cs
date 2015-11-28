using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bliki.Domain;
using Bliki.Models.Wiki;

namespace Bliki.Controllers
{
    public class WikiController : Controller
    {
        private readonly PageService _pageService;

        public WikiController(PageService pageService)
        {
            _pageService = pageService;
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var action = this.RedirectToAction("");
            action.RouteValues["id"] = actionName;
            action.ExecuteResult(this.ControllerContext);
        }

        // GET: Wiki
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var wikiPage = this._pageService.Get(id);
                var model = new WikiPageViewModel()
                {
                    Body = wikiPage.Render(),
                    Title = wikiPage.Title,
                    PageId = wikiPage.Id
                };
                return View(model);
            }

            var emptyModel = new WikiPageViewModel()
            {
                Title = id,
                PageId = Guid.Empty,
            };
            return View(emptyModel);
        }


    }
}