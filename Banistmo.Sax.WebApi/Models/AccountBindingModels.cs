﻿using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Banistmo.Sax.WebApi.Models
{
    // Models used as parameters to AccountController actions.
    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        //[Required]
        //[Display(Name = "UserName")]
        //[StringLength(50, ErrorMessage = "The {0} must be at least {8} characters long.", MinimumLength = 8)]
        //public string UserName { get; set; }

        //[Required]
        [Display(Name = "mail")]
        public string Mail { get; set; }

        //[Required]
        [Display(Name = "completeName")]
        public string completeName { get; set; }

        ////[Required]
        //[Display(Name = "LastName")]
        //public string LastName { get; set; }

        ////[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        [Required]
        [Display(Name = "usertToRegister")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string userToRegister { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class DeleteUserModel
    {
        [Required]
        public string userName { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
