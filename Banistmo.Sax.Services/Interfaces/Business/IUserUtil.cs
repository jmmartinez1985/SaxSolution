using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IUserUtil
    {
        String getUserFullName(AspNetUsers userList, string userID);
        String getUserCode(AspNetUsers userList, string userID);
    }
}
