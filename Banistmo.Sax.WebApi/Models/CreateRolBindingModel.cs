using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class CreateRoleBindingModel
    {

        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Role Description")]
        public string Description { get; set; }

        //[Required]
        public int Estatus { get; set; }


    }

    public class EditRoleBindingModel
    {
        public string Id { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Role Description")]
        public string Description { get; set; }

        //[Required]
        public int Estatus { get; set; }

    }

    public class UsersInRoleModel
    {

        public string Id { get; set; }
        public List<string> EnrolledUsers { get; set; }
        public List<string> RemovedUsers { get; set; }
    }

    public class ModuloInRole
    {
        //ID Role
        public List<ModuloRolModel> AddModuloRolModel { get; set; }
    }


    public class UsuariosInAreas
    {
        public List<UsuarioAreaModel> EnrolledUsers { get; set; }
        public List<UsuarioAreaModel> RemovedUsers { get; set; }
    }

    public class UsuariosInEmpresas
    {
        public List<UsuarioEmpresaModel> EnrolledUsers { get; set; }
        public List<UsuarioEmpresaModel> RemovedUsers { get; set; }
    }

    public class ExistingRole
    {

        public string Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int Estatus { get; set; }

    }

    public  class UserAttributes
    {
        public List<UsuarioEmpresaModel> Empresas { get; set; }
        public List<UsuarioAreaModel> Areas { get; set; }
        public List<ExistingRole> Roles { get; set; }

        public List<ModuloRolModel> ModulosRol { get; set; }



    }

}