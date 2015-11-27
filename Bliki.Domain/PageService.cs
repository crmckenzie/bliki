using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bliki.Domain
{
    public class PageService
    {
        public PageService()
        {
            this.PageStore = new Dictionary<string, WikiPage>();
        }

        WikiPage GetPageByTitle (string title)
        {
            if (PageStore.ContainsKey(title))
            {
                return PageStore[title];
            }
            return new WikiPage()
            {
                Title = title
            };
        }

        Dictionary<string, WikiPage> PageStore { get; set; }

        public WikiPage Get(string title)
        {
            return GetPageByTitle(title);
        }

        public void Save(WikiPage page)
        {
            page.Id = Guid.NewGuid();
            page.Links = IdentifyWikiLinks(page);
            ReplaceLinkText(page);
            this.PageStore[page.Title] = page;
        }

        private void ReplaceLinkText(WikiPage page)
        {
            foreach (var link in page.Links)
            {
                link.ReplaceLinkTextInPageBody();
            }
            UpdateLinksInOtherPages(page);
        }

        private void UpdateLinksInOtherPages(WikiPage page)
        {
            var otherPages = this.PageStore.Values.Except(new[] {page});
            var matchingLinks = otherPages.SelectMany(p => p.Links)
                .Where(row => row.PageId == Guid.Empty)
                .Where(row => row.Title == page.Title)
                ;

            matchingLinks.ToList().ForEach(link =>
            {
                link.PageId = page.Id;
                link.ReplaceLinkTextInPageBody();
            });
        }

        private List<WikiPageLink> IdentifyWikiLinks(WikiPage page)
        {
            var options = RegexOptions.Multiline 
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnoreCase
                ;

            var regex = new Regex(@"\[\[(\w|\s|\|)+\]\]", options);
            var matches = regex.Match(page.Body ?? "");
            if (!matches.Success) return new List<WikiPageLink>();

            var query = from match in matches.Groups.Cast<Group>()
                let text = ParseText(match.Value)
                let title = ParseTitle(match.Value)
                select new WikiPageLink(page)
                {
                    Id = Guid.NewGuid(),
                    DisplayText = text,
                    Title = title,
                    PageId = GetPageIdByTitle(title),
                    OriginalLinkText = match.Value
                };

            var results =  query.ToList();
            return results;
        }

        private string ParseText(string value)
        {
            var token = value.Split('|').First();
            return token.Replace("[", "").Replace("]", "");
        }
        private string ParseTitle(string value)
        {
            var token = value.Split('|').Last();
            return token.Replace("[", "").Replace("]", "");
        }

        private Guid GetPageIdByTitle(string title)
        {
            var page = GetPageByTitle(title);
            return page.Id;
        }
    }
}