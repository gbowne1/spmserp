using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Practice.Domain.Abstract;
using Practice.Domain.Entities;
using PracticeWeb.WebUI.Controllers;
using PracticeWeb.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace spmserp.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Can_Go_User_Create()
        {
            MyMock myMock = new MyMock();
            var controller = new AdminController(myMock.ProductRepository.Object, myMock.UserManager.Object);

            ViewResult result = controller.UserCreate() as ViewResult;

            Assert.AreEqual("~/Views/Home/CreateUser.cshtml", result.ViewName);
        }

        [TestMethod]
        public void Can_Create_User()
        {
            var myMock = new MyMock();
            //不確認CreateAsync功能
            myMock.UserManager.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            CreateAccountModel userInfo = new CreateAccountModel
            {
                Name = "UserName",
                Email = "user@example.com",
                Password = "12sd45er78",
                PasswordConfirm = "12sd45er78",
                Phone = "0123456789",
                CityID = 2,
                CountryID = 3,
                ShippingAddress = "999"
            };
            var controller = new AdminController(myMock.ProductRepository.Object, myMock.UserManager.Object);

            var result = controller.UserCreate(userInfo) as Task<ActionResult>;
            var viewresult = result.Result;

            Assert.AreEqual("UserIndex", (viewresult as RedirectToRouteResult).RouteValues["action"]);
        }

        [TestMethod]
        public void Cannot_Create_User_When_ModelState_Invalid()
        {
            var myMock = new MyMock();
            CreateAccountModel userInfo = new CreateAccountModel
            {
                Name = "UserName",
                Email = "user@example.com",
                Password = "12sd45er78",
                PasswordConfirm = "12sd45er78",
                Phone = "0123456789",
                CityID = 2,
                CountryID = 3,
                ShippingAddress = "999"
            };
            var controller = new AdminController(myMock.ProductRepository.Object, myMock.UserManager.Object);
            controller.ViewData.ModelState.AddModelError("error", "error");

            var result = controller.UserCreate(userInfo) as Task<ActionResult>;
            var viewresult = result.Result as ViewResult;

            Assert.AreEqual("~/Views/Home/CreateUser.cshtml", viewresult.ViewName);
        }
    }
}