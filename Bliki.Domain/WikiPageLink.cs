using System;

namespace Bliki.Domain
{
    public class WikiPageLink
    {
        private readonly WikiPage _page;

        public WikiPageLink(WikiPage page)
        {
            _page = page;
        }

        public Guid PageId { get; set; }
        public string DisplayText { get; set; }
        public string Title { get; set; }
        public string OriginalLinkText { get; set; }
        public Guid Id { get; set; }

        public void ReplaceLinkTextInPageBody()
        {
            _page.Body = _page.Body.Replace(OriginalLinkText, NormalizedLinkText());
        }

        public string Render(string body)
        {
            return body.Replace(OriginalLinkText, Render());
        }

        public string Render()
        {
            if (string.IsNullOrWhiteSpace(DisplayText) || Title == DisplayText)
            {
                return $"[{Title}]";
            }

            return $"[{DisplayText}]({Title})";
        }

        public string NormalizedLinkText()
        {
            if (PageId != Guid.Empty)
                return $"{{{{{Id}}}}}";

            return OriginalLinkText;
        }
    }
}