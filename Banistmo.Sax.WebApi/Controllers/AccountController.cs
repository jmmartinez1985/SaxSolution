﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.WebApi.Providers;
using Banistmo.Sax.WebApi.Results;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System.Threading;
using System.Linq;
using System.Configuration;
using Banistmo.Sax.Services.Implementations.Business;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _appRoleManager;

        private readonly IUsuarioAreaService usuarioAreaService;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;
        private readonly IModuloRolService moduloRolService;
        private readonly ICatalogoService catalagoService;
        private readonly ILDAP directorioActivo;
        private readonly IAspNetUserRolesService objInjUserRol;


        public enum RegistryState
        {
            inactivo = 0,
            activo = 1,
            eliminado = 2
        }
        public AccountController()
        {
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            usuarioEmpresaService = usuarioEmpresaService ?? new UsuarioEmpresaService();
            moduloRolService = moduloRolService ?? new ModuloRolService();
            catalagoService = catalagoService ?? new CatalogoService();
            directorioActivo = directorioActivo ?? new LDAP();
            objInjUserRol = objInjUserRol ?? new AspNetUserRolesService();

        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, ApplicationRoleManager appRoleManager)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            _appRoleManager = appRoleManager;
        }

        public AccountController(IUsuarioAreaService svcusuarioAreaService,
            IUsuarioEmpresaService svcusuarioEmpresaService, IModuloRolService svcmoduloRolService, 
            ICatalogoService catServ, ILDAP dau, IAspNetUserRolesService userRole)
        {
            usuarioAreaService = svcusuarioAreaService;
            usuarioEmpresaService = svcusuarioEmpresaService;
            moduloRolService = svcmoduloRolService;
            catalagoService = catServ;
            directorioActivo = dau;
            objInjUserRol = userRole;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _appRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _appRoleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }



            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (Properties.Settings.Default.ambiente != "des")
            {
                var validaDA = directorioActivo.validaUsuarioLDAP(Properties.Settings.Default.userServiceDA, Properties.Settings.Default.passwordServiceDA, Properties.Settings.Default.loginIntranet, Properties.Settings.Default.dominioDa, model.userToRegister);
                if (validaDA.existe) //existe en directorio activo
                {
                    var user = new ApplicationUser()
                    {
                        FirstName = validaDA.nombreCompleto,
                        LastName = validaDA.nombreCompleto,
                        Level = 1,
                        Estatus = 1,
                        JoinDate = DateTime.Now,
                        Email = validaDA.mail,
                        EmailConfirmed = true,
                        UserName = validaDA.userNumber,
                        numColaborador = validaDA.numColaborador
                    };

                    var userfound = UserManager.Find(model.userToRegister, model.userToRegister);
                    if (userfound == null) //usuario no existe
                    {
                        IdentityResult result = await UserManager.CreateAsync(user, model.userToRegister);
                        if (!result.Succeeded)
                        {
                            if (((string[])result.Errors)[0].Contains("already taken"))
                            {
                                ((string[])result.Errors)[0] = "Usuario ya registrado en la aplicación";
                            }
                            return GetErrorResult(result);
                        }
                        else
                        {
                            return Ok(result);
                        }
                    }
                    else if (userfound.Estatus == 1) //usuario existe
                    {
                        return BadRequest("Usuario activo, Usuario activo en aplicación SAX");
                    }
                    else if (userfound.Estatus == 0)
                    {
                        return BadRequest("Usuario inactivo, Usuario existe inactivo en aplicación SAX");
                    }
                }
                else
                {
                    return BadRequest("Usuario no existe, Usuario no existe en directorio activo");
                }
            }
            else
            {
                var user = new ApplicationUser()
                {
                    FirstName = model.completeName,
                    LastName = model.completeName,
                    Level = 1,
                    Estatus = 1,
                    JoinDate = DateTime.Now,
                    Email = model.Mail,
                    EmailConfirmed = true,
                    UserName = model.userToRegister
                };
                var userfound = UserManager.Find(model.userToRegister, model.userToRegister);
                if (userfound == null) //usuario no existe
                {
                    IdentityResult result = await UserManager.CreateAsync(user, model.userToRegister);
                    if (!result.Succeeded)
                    {
                        if(((string[])result.Errors)[0].Contains("already taken"))
                        {
                            ((string[])result.Errors)[0] = "Usuario ya registrado en la aplicación";
                        }
                        return GetErrorResult(result);

                    }
                    else
                    {
                        return Ok(result);
                    }
                }
                else if (userfound.Estatus == 1) //usuario existe
                {
                    return BadRequest("Usuario activo, Usuario activo en aplicación SAX");
                }
                else if (userfound.Estatus == 0)
                {
                    return BadRequest("Usuario inactivo, Usuario existe inactivo en aplicación SAX");
                }
            }
            return Ok();
        }

        [Route("UpdateUserStatus"), HttpPost]
        public async Task<IHttpActionResult> UpdateUserStatus([FromBody] userparameter userParameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userFound = await UserManager.FindAsync(userParameter.userName, userParameter.userName);
            if (userParameter.estatus == 0 || userParameter.estatus == 2) 
            {
                string userId = User.Identity.GetUserId();
                var userRol = objInjUserRol.GetSingle(c => c.UserId == userId, includes: c => c.AspNetRoles);
                if (userRol == null)
                {
                    userFound.Estatus = userParameter.estatus;
                    IdentityResult result = await UserManager.UpdateAsync(userFound);
                    if (!result.Succeeded)
                    {
                        return BadRequest("No se pudo actualizar el status del usuario.");
                    }
                    else
                    {
                        return Ok(result);
                    }
                }
                else
                {
                    return BadRequest("No se puede desactivar el usuario porque tiene roles asignados.");
                }
            }
            else
            {
                userFound.Estatus = userParameter.estatus;
                IdentityResult result = await UserManager.UpdateAsync(userFound);
                if (!result.Succeeded)
                {
                    return BadRequest("No se pudo actualizar el status del usuario.");
                }
                else
                {
                    return Ok(result);
                }
            }            
        }


        // POST api/Account/RegisterUserDisabled
        //[AllowAnonymous]
        [Route("RegisterUserDisabled")]
        public async Task<IHttpActionResult> RegisterUserDisabled(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            

            //Se valida el ambiente
            if (Properties.Settings.Default.ambiente != "des")
            {
                var validaDA = directorioActivo.validaUsuarioLDAP(Properties.Settings.Default.userServiceDA, Properties.Settings.Default.passwordServiceDA, Properties.Settings.Default.loginIntranet, Properties.Settings.Default.dominioDa, model.userToRegister);
                if (validaDA.existe) //existe en directorio activo
                {
                    var user = new ApplicationUser()
                    {
                        FirstName = validaDA.nombreCompleto,
                        LastName = validaDA.nombreCompleto,
                        Level = 1,
                        Estatus = 1,
                        JoinDate = DateTime.Now,
                        Email = validaDA.mail,
                        EmailConfirmed = true,
                        UserName = validaDA.userNumber
                    };

                    var userfound = await UserManager.FindAsync(user.UserName, user.UserName);
                    if (userfound.Estatus == 1) //usuario activo
                    {
                        return BadRequest("Usuario activo, Usuario activo en aplicación SAX");
                    }
                    else if (userfound.Estatus == 0)//usuario inactivo
                    {
                        user.Estatus = 1;
                        IdentityResult result = await UserManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            return GetErrorResult(result);
                        }
                    }
                }
                else
                {
                    return BadRequest("Usuario no existe, Usuario no existe en directorio Activo");
                }
            }
            else
            {
                var userfound = await UserManager.FindAsync(model.userToRegister, model.userToRegister);
                if (userfound != null)
                {
                    if (userfound.Estatus == 1) //usuario activo
                    {
                        return BadRequest("Usuario activo, Usuario activo en aplicación SAX");
                    }
                    else if (userfound.Estatus == 0)//usuario inactivo
                    {
                        userfound.Estatus = 1;
                        IdentityResult result = await UserManager.UpdateAsync(userfound);
                        if (!result.Succeeded)
                        {
                            return GetErrorResult(result);
                        }
                    }
                }
                else
                {
                    return BadRequest("Usuario no existe, Usuario no existe en aplicación SAX");
                }
            }
            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }


        [Route("GetUserAttributes"), HttpGet]
        public async Task<IHttpActionResult> GetUserAttributes()
        {
            List<ExistingRole> listExistingRoles = new List<ExistingRole>();
            List<UsuarioAreaModel> listUsuarioArea = new List<UsuarioAreaModel>();
            List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
            List<ModuloRolModel> listModuloRol = new List<ModuloRolModel>();
            List<ApplicationRole> listRoles = new List<ApplicationRole>();
            UserAttributes attributes = new UserAttributes();
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            
            if (user == null)
            {
                return null;
            }
            var estatusList = await catalagoService.GetAllAsync(c => c.CA_TABLA == "sax_estatus", c => c.SAX_CATALOGO_DETALLE);

            foreach (var role in user.Roles)
            {
                var roleobject = await RoleManager.FindByIdAsync(role.RoleId);
                var castingroles = roleobject as ApplicationRole;
                if (roleobject != null)
                {
                    listRoles.Add(castingroles);
                    var moduloRoles = await moduloRolService.GetAllAsync(c => c.RL_ID_ROL == role.RoleId,
                        c => c.SAX_MODULO
                       );
                    foreach (var item in moduloRoles)
                    {
                        listModuloRol.Add(item);
                    }
                }
            }
            var listAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == user.Id,null, includes: c=> c.SAX_AREA_OPERATIVA);
            if (listAreas.Count > 0)
            {
                foreach (var area in listAreas)
                {
                    listUsuarioArea.Add(area);
                }
            }
            var listEmpresas = await usuarioEmpresaService.GetAllAsync(c => c.US_ID_USUARIO == user.Id, c => c.SAX_EMPRESA);
            if (listEmpresas.Count > 0)
            {
                foreach (var emp in listEmpresas)
                {
                    listUsuarioEmpresas.Add(emp);
                }
            }
            return Ok(new
            {
                Name = ((Banistmo.Sax.WebApi.Models.ApplicationUser)user).FirstName,
                Email = user.Email,
                Roles = listRoles.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_VALOR

                }),
                Areas = listUsuarioArea.Select(c => new
                {
                    Id = c.SAX_AREA_OPERATIVA.CA_ID_AREA,
                    Name = c.SAX_AREA_OPERATIVA .CA_COD_AREA.ToString() +'-' + c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_VALOR
                }),
                Empresas = listUsuarioEmpresas.Select(c => new
                {
                    Id = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    Name = c.SAX_EMPRESA .CE_COD_EMPRESA +'-'+c.SAX_EMPRESA.CE_NOMBRE,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_VALOR
                }),
                Modulos = listModuloRol.Select(c => new
                {
                    Id = c.SAX_MODULO.MO_ID_MODULO,
                    Name = c.SAX_MODULO.MO_MODULO,
                    Path = c.SAX_MODULO.MO_PATH,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_VALOR
                })
            });
        }

        [Route("DeleteUserByUserName")]
        public async Task<IHttpActionResult> DeleteUserByUserName(DeleteUserModel model)
        {
            try
            {
                var userfound = await UserManager.FindAsync(model.userName, model.userName);
                if (userfound != null)
                {
                    userfound.Estatus = 0;
                    IdentityResult result = await UserManager.UpdateAsync(userfound);
                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                    /*
                    if (userfound.Estatus == 1) //usuario activo
                    {
                        return BadRequest("Usuario activo, Usuario activo en aplicación SAX");
                    }
                    else if (userfound.Estatus == 0)//usuario inactivo
                    {
                        userfound.Estatus = 1;

                        IdentityResult result = await UserManager.UpdateAsync(userfound);
                        if (!result.Succeeded)
                        {
                            return GetErrorResult(result);
                        }
                    }
                    */
                }
                else
                {
                    return BadRequest("Usuario no existe, Usuario no existe en aplicación SAX");
                }
            }
            catch
            {
                return BadRequest("Usuario no existe, Usuario no existe en aplicación SAX");
            }
            return Ok();
    }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        public class userparameter
        {
            public string userName { get; set; }
            public short estatus { get; set; }

        }


        #endregion
    }
}
