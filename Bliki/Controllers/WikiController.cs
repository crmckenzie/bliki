using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bliki.Domain;
using Bliki.Models.Wiki;
using MarkdownSharp;

namespace Bliki.Controllers
{
    public class WikiController : Controller
    {
        private readonly PageService _pageService;
        private readonly Markdown _markdown;

        public WikiController(PageService pageService, Markdown markdown)
        {
            _pageService = pageService;
            _markdown = markdown;
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
                var transform = _markdown.Transform(wikiPage.Body);
                var model = new WikiPageViewModel()
                {
                    Body = transform,
                    Title = wikiPage.Title,
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

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentOutOfRangeException("Page title is required.");

            var wikiPage = this._pageService.Get(id);
            var model = new WikiPageViewModel()
            {
                Body = wikiPage.Body,
                Title = wikiPage.Title,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(WikiPageViewModel viewModel)
        {
            var page = new WikiPage()
            {
                Body = viewModel.Body,
                Title = viewModel.Title,
            };

            this._pageService.Save(page);

            return RedirectToAction("Index", new {id = page.Title});
        }

    }
}