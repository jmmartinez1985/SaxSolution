using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {

        private readonly IUserService userService;

        public UserController(IUserService usr)
        {
            userService = usr;
        }
        // GET: api/User
        public IHttpActionResult Get()
        {
            List<AspNetUserModel> user = userService.GetAll();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/User/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        public IHttpActionResult Post([FromBody] AspNetUserModel model)
        {
            userService.Insert(model, true);
            return Ok();
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }
    }
}
