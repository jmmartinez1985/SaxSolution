﻿using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Banistmo.Sax.WebApi;
using Banistmo.Sax.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Banistmo.Sax.WebApi.Tests.Controllers
{
    [TestClass]
    public class EventosControllerTest
    {
        [TestMethod]
        public void prubaGetEventosController()
        {
            // Arrange
            EventosController controller = new EventosController();

            // Act
            ViewResult result = controller.Get() as ViewResult;

            // Assert
        }
    }
}