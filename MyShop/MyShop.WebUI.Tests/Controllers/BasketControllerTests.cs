using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Model;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            //setup
            IRepository<Basket> baskets = new MockContent<Basket>();
            IRepository<Product> products = new MockContent<Product>();

            var httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(products, baskets);
            var controller = new BasketController(basketService);
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(),controller);

            //Act
            //basketService.AddToBasket(httpContext, "1");
            controller.AddToBasket("1");

            Basket basket = baskets.Collection().FirstOrDefault();

            //Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            //setup
            IRepository<Basket> baskets = new MockContent<Basket>();
            IRepository<Product> products = new MockContent<Product>();

            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 15.00m });
            products.Insert(new Product() { Id = "3", Price = 12.00m });

            Basket basket = new Basket();

            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 5 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "3", Quantity = 2 });

            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);

            var controller = new BasketController(basketService);

            var httpContext = new MockHttpContext();

            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            var result = controller.BasketSummary() as PartialViewResult;

            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            Assert.AreEqual(9, basketSummary.BasketCount);
            Assert.AreEqual(119.00m, basketSummary.BasketTotal);

        }
    }
}
