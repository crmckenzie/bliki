using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bliki.Controllers;
using Bliki.Domain;
using Bliki.Models.Wiki;
using MarkdownSharp;
using NSubstitute;
using NUnit.Framework;

namespace Bliki.IntegrationTests
{
    [TestFixture]
    class WikiControllerTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.PageService = Substitute.For<PageService>();
            this.Subject = new WikiController(this.PageService, new Markdown());
        }

        public WikiController Subject { get; set; }

        [Test]
        public void Index()
        {
            // Given
            this.PageService.Get("Some page").Returns(new WikiPage()
            {
                Title = "Some Title",
                Body = "Some body",
            });

            //When
            var result = this.Subject.Index("Some page");

            // Then
            Assert.That(result, Is.InstanceOf<ViewResult>());

            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.InstanceOf<WikiPageViewModel>());

            var viewModel = viewResult.Model as WikiPageViewModel;
            Assert.That(viewModel.Title, Is.EqualTo("Some Title"));
            Assert.That(viewModel.Body, Is.EqualTo("Some body"));
        }


        [Test]
        public void Edit()
        {
            //Given
            var wikiPage = new WikiPage()
            {
                Title = "Some page",
                Body = "Some body text",
            };
            this.PageService.Get("Some page").Returns(wikiPage);

            // When
            var result = this.Subject.Edit("Some page");
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.InstanceOf<WikiPageViewModel>());

            var viewModel = viewResult.Model as WikiPageViewModel;
            Assert.That(viewModel.Title, Is.EqualTo(wikiPage.Title));
            Assert.That(viewModel.Body, Is.EqualTo(wikiPage.Body));
        }

        [Test]
        public void Edit_Post()
        {
            //Given
            var viewModel = new WikiPageViewModel()
            {
                Body = "Some body text",
                Title = "Some title",
            };

            // When
            var result = this.Subject.Edit(viewModel);

            // Then
            this.PageService.Received().Save(Arg.Is<WikiPage>(page => page.Body == viewModel.Body && page.Title == viewModel.Title));
            Assert.That(result, Is.InstanceOf<RedirectToRouteResult>());
            var redirect = (result as RedirectToRouteResult);
            Assert.That(redirect.RouteName, Is.EqualTo("Index"));
            Assert.That(redirect.RouteValues["id"], Is.EqualTo("Some title"));
        }


        public PageService PageService { get; set; }
    }

}
