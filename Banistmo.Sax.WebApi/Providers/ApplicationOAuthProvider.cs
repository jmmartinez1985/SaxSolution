using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;

using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly ILDAP directorioActivo;

        public ApplicationOAuthProvider(ILDAP dau)
        {
            directorioActivo = dau;
        }
        public ApplicationOAuthProvider(string publicClientId, ILDAP dau)
        {
            directorioActivo = dau;
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
            
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                       
            ApplicationUser user = null;

            //bypass de validación de directorio activo
            if (Properties.Settings.Default.ambiente != "des")
            {   //validacion directorio activo             
                var validaDA = directorioActivo.validaUsuarioLDAP(Properties.Settings.Default.userServiceDA, Properties.Settings.Default.passwordServiceDA, Properties.Settings.Default.loginIntranet,Properties.Settings.Default.dominioDa);
                if (validaDA.existe)
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                    if (user == null)
                    {
                        context.SetError("Usuario no existe", "Usuario no existe registrado aplicativo SAX.");
                        return;
                    }
                    else if (user.Estatus == 0)
                    {
                        context.SetError("Usuario inactivo", "Usuario inactivo en aplicativo SAX.");
                        return;
                    }
                }
                else
                {
                    context.SetError("Usuario no existe", "El usuario no existe en el directorio activo.");
                    return; 
                }
            }
            else 
            {               
                user = await userManager.FindAsync(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("Usuario no existe", "Usuario no existe registrado aplicativo SAX.");
                    return;
                }
                else if (user.Estatus == 0)
                {
                    context.SetError("Usuario inactivo", "Usuario inactivo en aplicativo SAX.");
                    return;
                }
            }
            
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}