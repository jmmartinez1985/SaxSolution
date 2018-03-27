using Banistmo.Sax.Services.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class UserUtil : IUserUtil
    {
        public string getUserFullName(AspNetUsers userList, string userID)
        {
            string userName = string.Empty;
            if (userID != null)
            {
                userName = userList.FirstName + " " + userList.LastName;
            }
            else
            {
                return null;
            }
            return userName;
        }
  
        public string getUserCode (AspNetUsers userList, string userID)
        {
            string userName = string.Empty;
            if (userID != null)
            {
                userName = userList.UserName;
            }
            else
            {
                return null;
            }
            return userName;
        }
    }
}
