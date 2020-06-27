using System;
using System.Linq;
using System.Security.Principal;
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
            IRepository<Order> orders = new MockContent<Order>();

            IRepository<Customer> customers = new MockContent<Customer>();

            var httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);
            var controller = new BasketController(basketService,orderService,customers);
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
            IRepository<Order> orders = new MockContent<Order>();
            IRepository<Customer> customers = new MockContent<Customer>();

            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 15.00m });
            products.Insert(new Product() { Id = "3", Price = 12.00m });

            Basket basket = new Basket();

            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 5 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "3", Quantity = 2 });

            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);

            var controller = new BasketController(basketService,orderService,customers);

            var httpContext = new MockHttpContext();

            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            var result = controller.BasketSummary() as PartialViewResult;

            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            Assert.AreEqual(9, basketSummary.BasketCount);
            Assert.AreEqual(119.00m, basketSummary.BasketTotal);

        }

        [TestMethod]
        public void CanCheckOUtAndCreateOrder()
        {
            IRepository<Customer> customers = new MockContent<Customer>();

            IRepository<Product> products = new MockContent<Product>();
            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 20.00m });

            IRepository<Basket> baskets = new MockContent<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2, BasketId = basket.Id });
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 1, BasketId = basket.Id });

            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);

            IRepository<Order> orders = new MockContent<Order>();
            IOrderService orderService = new OrderService(orders);

            customers.Insert(new Customer() { Id = "1", Email = "voltsrage@hotmail.com", ZipCode = "90000" });

            IPrincipal FakeUser = new GenericPrincipal(new GenericIdentity("voltsrage@gmail.com", "Forms"),null);

            var controller = new BasketController(basketService, orderService,customers);
            var httpContext = new MockHttpContext();
            httpContext.User = FakeUser;
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            Order order = new Order();
            controller.Checkout(order);

            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            //Order orderInRep = orders.Find(order.Id);
            //Assert.AreEqual(2, orderInRep.OrderItems.Count);
        }
    }
}
