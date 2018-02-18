using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//SA: JMMB
namespace Banistmo.Sax.Services.Models
{
    public class UserModel
    {
        public int USR_Id { get; set; }

        [MaxLength(30, ErrorMessage = "Must be Maximum 30 Characters")]
        public string USR_Name { get; set; }
        public Nullable<System.DateTime> USR_RegistrationDate { get; set; }
    }
}
