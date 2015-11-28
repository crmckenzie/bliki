using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bliki.Models.Wiki
{
    public class WikiPageViewModel
    {
        public string Body { get; set; }
        public Guid PageId { get; set; }
        public string Title { get; set; }
    }
}