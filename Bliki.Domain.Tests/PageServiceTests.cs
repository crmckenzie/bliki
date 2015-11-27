using System;
using System.Linq;
using NUnit.Framework;

namespace Bliki.Domain.Tests
{
    // indexes
    // page ids and titles
    // page ids and link ids
    // update page body on Get instead of save
    // perhaps add last edit time to page and link to help optimize this behavior


    [TestFixture]
    public class PageServiceTests
    {
        [SetUp]
        public void BeforeEach()
        {
            Subject = new PageService();
        }

        public PageService Subject { get; set; }

        [Test]
        public void RequestNewPage()
        {
            // When
            var page = Subject.Get("NewPage");

            // Then
            Assert.That(page, Is.Not.Null);
            Assert.That(page.Title, Is.EqualTo("NewPage"));
            Assert.That(page.Body, Is.Null.Or.Empty);
            Assert.That(page.Id, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public void SavePage()
        {
            // Given
            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"This is a wiki page body";

            // When
            this.Subject.Save(page);
            var result = this.Subject.Get(page.Title);

            // Then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(page.Title));
            Assert.That(result.Body, Is.EqualTo(page.Body));
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void SavePageWithLinkToAnotherNewPage()
        {
            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Another Page]]
                ";

            // When
            this.Subject.Save(page);

            // Then
            Assert.That(page.Links, Has.Count.EqualTo(1));
            Assert.That(page.Body, Does.Contain("[[Another Page]]"));

            var link = page.Links.First();
            Assert.That(link.Title, Is.EqualTo("Another Page"));
            Assert.That(link.DisplayText, Is.EqualTo("Another Page"));
            Assert.That(link.PageId, Is.EqualTo(Guid.Empty));
            Assert.That(link.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void SavePageWithLinkAndTextToAnotherNewPage()
        {
            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Display Text|Another Page]]
                ";

            // When
            this.Subject.Save(page);

            // Then
            Assert.That(page.Links, Has.Count.EqualTo(1));

            var link = page.Links.First();
            Assert.That(link.Title, Is.EqualTo("Another Page"));
            Assert.That(link.DisplayText, Is.EqualTo("Display Text"));
            Assert.That(link.PageId, Is.EqualTo(Guid.Empty));
            Assert.That(link.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void SavePageWithLinkToExistingPage()
        {
            var otherPage = Subject.Get("Another Page");
            this.Subject.Save(otherPage);

            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Another Page]]
                ";

            // When
            this.Subject.Save(page);

            // Then
            Assert.That(page.Links, Has.Count.EqualTo(1));

            var link = page.Links.First();
            Assert.That(link.Title, Is.EqualTo("Another Page"));
            Assert.That(link.DisplayText, Is.EqualTo("Another Page"));
            Assert.That(link.PageId, Is.EqualTo(otherPage.Id));

            Assert.That(page.Body, Does.Not.Contain("[[Another Page]]"));
            Assert.That(page.Body, Does.Contain($"{{{{{link.Id}}}}}"));
        }

        [Test]
        public void SavePageWithLinkWithDisplayTextToExistingPage()
        {
            var otherPage = Subject.Get("Another Page");
            this.Subject.Save(otherPage);

            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Display Text|Another Page]]
                ";

            // When
            this.Subject.Save(page);

            // Then
            Assert.That(page.Links, Has.Count.EqualTo(1));

            var link = page.Links.First();
            Assert.That(link.Title, Is.EqualTo("Another Page"));
            Assert.That(link.DisplayText, Is.EqualTo("Display Text"));
            Assert.That(link.PageId, Is.EqualTo(otherPage.Id));

            Assert.That(page.Body, Does.Not.Contain("[[Another Page]]"));
            Assert.That(page.Body, Does.Contain($"{{{{{link.Id}}}}}"));
        }

        [Test]
        public void SavePageWithLinkToPageThatComesIntoExistence()
        {
            // Given
            var firstPage = Subject.Get("NewPage");
            firstPage.Title = "This is a wiki page";
            firstPage.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Another Page]]
                ";
            this.Subject.Save(firstPage);

            var otherPage = Subject.Get("Another Page");
            this.Subject.Save(otherPage);

            // When
            var result = Subject.Get(firstPage.Title);
            var link = result.Links.First();

            Assert.That(result.Links, Has.Count.EqualTo(1));
            Assert.That(result.Body, Does.Not.Contain("[[Another Page]]"));
            Assert.That(result.Body, Does.Contain($"{{{{{link.Id}}}}}"));
        }

        [Test]
        public void RenderPageWithLinkToNewPage()
        {
            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Another Page]]
                ";
            this.Subject.Save(page);

            // When
            var content = page.Render();

            // Then
            Assert.That(content, Is.EqualTo(@"
                    This is a wiki page body
                    And it contains a link to [Another Page]
                "));
        }

        [Test]
        public void RenderPageWithLinkWithDisplayTextToNewPage()
        {
            var page = Subject.Get("NewPage");
            page.Title = "This is a wiki page";
            page.Body = @"
                    This is a wiki page body
                    And it contains a link to [[Another Page|Display Text]]
                ";
            this.Subject.Save(page);

            // When
            var content = page.Render();

            // Then
            Assert.That(content, Is.EqualTo(@"
                    This is a wiki page body
                    And it contains a link to [Another Page](Display Text)
                "));
        }
    }
}