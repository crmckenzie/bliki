using System;
using System.Collections.Generic;
using System.Linq;

namespace Bliki.Domain
{
    public class WikiPage
    {
        public WikiPage()
        {
            this.Links= new List<WikiPageLink>();
        }
        public string Title { get; set; }

        public string Body { get; set; }

        public Guid Id { get; set; }
        public List<WikiPageLink> Links { get; set; }

        public string Render()
        {
            var body = this.Body;
            var result = Links.Aggregate(body, (current, link) => 
                link.Render(current));
            return result;
        }
    }
}