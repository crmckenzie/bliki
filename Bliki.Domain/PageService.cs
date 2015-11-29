using System;
using System.Collections.Generic;

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

        public virtual WikiPage Get(string title)
        {
            return GetPageByTitle(title);
        }

        public virtual void Save(WikiPage page)
        {
            this.PageStore[page.Title] = page;
        }
    }
}