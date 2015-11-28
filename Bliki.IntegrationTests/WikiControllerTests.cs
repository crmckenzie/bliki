using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bliki.Controllers;
using Bliki.Domain;
using Bliki.Models.Wiki;
using NUnit.Framework;

namespace Bliki.IntegrationTests
{
    [TestFixture]
    class WikiControllerTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.PageService = new PageService();
            this.Subject = new WikiController(this.PageService);
        }

        public WikiController Subject { get; set; }

        [Test]
        public void Index_WhenPageDoesNotExist()
        {

            //When
            var result = this.Subject.Index("Some page");

            // Then
            Assert.That(result, Is.InstanceOf<ViewResult>());

            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.InstanceOf<WikiPageViewModel>());

            var viewModel = viewResult.Model as WikiPageViewModel;
            Assert.That(viewModel.Title, Is.EqualTo("Some page"));
            Assert.That(viewModel.Body, Is.Null.Or.Empty);
            Assert.That(viewModel.PageId, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public void Index_WhenPageExists()
        {
            //Given
            var wikiPage = new WikiPage()
            {
                Title = "Some page",
                Body = "Some body text",
            };
            this.PageService.Save(wikiPage);

            // When
            var result = this.Subject.Index("Some page");
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.InstanceOf<WikiPageViewModel>());

            var viewModel = viewResult.Model as WikiPageViewModel;
            Assert.That(viewModel.Title, Is.EqualTo(wikiPage.Title));
            Assert.That(viewModel.Body, Is.EqualTo(wikiPage.Body));
            Assert.That(viewModel.PageId, Is.EqualTo(wikiPage.Id));

        }

        [Test]
        public void Index_PageIsRendered()
        {
            //Given
            var wikiPage = new WikiPage()
            {
                Title = "Some page",
                Body = "Some body text {{548183d0-ab34-446b-ac01-1d415dd34793}}",
            };
            wikiPage.Links = new List<WikiPageLink>()
            {
                new WikiPageLink(wikiPage)
                {
                    DisplayText = "Display Text",
                    Title = "Some other page",
                    PageId = Guid.NewGuid(),
                    Id = new Guid("548183d0-ab34-446b-ac01-1d415dd34793"),
                    OriginalLinkText = "[[Some other page]]"
                }
            };

            this.PageService.Save(wikiPage);

            // When
            var result = this.Subject.Index("Some page");
            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as WikiPageViewModel;

            Assert.That(viewModel.Body, Is.EqualTo("Some body text [Display Text](Some other page)"));
        }

        public PageService PageService { get; set; }
    }




}
