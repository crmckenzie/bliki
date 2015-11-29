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
        }

    }
}