using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Banistmo.Sax.WebApi;
using System;
using System.Globalization;

namespace Banistmo.Sax.WebApi.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {


            //// Arrange
            //HomeController controller = new HomeController();

            //// Act
            //ViewResult result = controller.Index() as ViewResult;

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual("Home Page", result.ViewBag.Title);
        }

        [TestMethod]
        public void ValidFecha()
        {

            string dateFormat = "MMddyyyy";
            IFormatProvider culture = new CultureInfo("en-US", true);

            DateTime PA_FECHA_TRX = DateTime.ParseExact("11242016", dateFormat, culture);

            var valor = PA_FECHA_TRX.Date.ToString("yyyyMMdd") + 1.ToString().PadLeft(5, '0');
            //// Arrange
            //HomeController controller = new HomeController();

            //// Act
            //ViewResult result = controller.Index() as ViewResult;

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual("Home Page", result.ViewBag.Title);
        }

    }
}
