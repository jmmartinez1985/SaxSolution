using System;
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

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, ApplicationRoleManager appRoleManager)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            _appRoleManager = appRoleManager;


        }

        public AccountController(IUsuarioAreaService svcusuarioAreaService,
            IUsuarioEmpresaService svcusuarioEmpresaService, IModuloRolService svcmoduloRolService, ICatalogoService catServ, ILDAP dau)
        {
            usuarioAreaService = svcusuarioAreaService;
            usuarioEmpresaService = svcusuarioEmpresaService;
            moduloRolService = svcmoduloRolService;
            catalagoService = catServ;
            directorioActivo = dau;
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
            //AGREGAR VALIDACION PARA CUANDO EL USUARIO YA EXISTE
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Mail,
                EmailConfirmed = true,
                Level = 1,
                FirstName = model.FirstName,
                LastName = model.LastName,
                JoinDate = DateTime.Now
            };

            // Agregar aquí las otras reglas de negocio para la creacion
            /*
            var validaDA = directorioActivo.validaUsuarioLDAP(model.UserName, model.Password,Properties.Settings.Default.loginIntranet,Properties.Settings.Default.dominioDa);
            if (Properties.Settings.Default.ambiente != "des")
            {
                if (validaDA.existe)
                {
                    IdentityResult result = await UserManager.CreateAsync(user, model.UserName);
                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                }
                else
                {
                    return BadRequest();
                }
            }*/
            
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


        [Route("GetUserAttributes")]
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
            var listAreas = await usuarioAreaService.GetAllAsync(c => c.US_ID_USUARIO == user.Id);
            if (listAreas.Count > 0)
            {
                foreach (var area in listAreas)
                {
                    listUsuarioArea.Add(area);
                }
            }
            var listEmpresas = await usuarioEmpresaService.GetAllAsync(c => c.US_ID_USUARIO == user.Id);
            if (listEmpresas.Count > 0)
            {
                foreach (var emp in listEmpresas)
                {
                    listUsuarioEmpresas.Add(emp);
                }
            }
            return Ok(new
            {
                //UserID = user.Id,
                Roles = listRoles.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_VALOR

                }),
                Areas = listUsuarioArea.Select(c => new
                {
                    Id = c.SAX_AREA_OPERATIVA.CA_COD_AREA,
                    Name = c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_VALOR
                }),
                Empresas = listUsuarioEmpresas.Select(c => new
                {
                    Id = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    Name = c.SAX_EMPRESA.CE_NOMBRE,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_VALOR
                }),
                Modulos = listModuloRol.Select(c => new
                {
                    Id = c.SAX_MODULO.MO_ID_MODULO,
                    Name = c.SAX_MODULO.MO_MODULO,
                    IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_ESTATUS,
                    Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_VALOR
                })
            });
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

        #endregion
    }
}
