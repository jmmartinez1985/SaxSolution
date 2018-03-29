using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
using System.Linq.Expressions;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;



namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class LDAP : ILDAP
    {
        //string loginIntranet = Properties.Settings.Default.loginIntranet;
        private RemoteCertificateValidationCallback AddressOf { get; set; }

        public UsuarioLDAPModel validaUsuarioLDAP(string usuario, string contraseña, string loginintra, string usuarioNuevoValidar = null)
        {
            UsuarioLDAPModel userDA = new UsuarioLDAPModel();
            try
            {
                if (ValidaIntranet(usuario, contraseña, loginintra, userDA) == 200)
                {
                    ValidaUsuario(usuario, userDA, usuarioNuevoValidar);
                }
            }
            catch (Exception ex)
            {
                userDA.existe = false;
                userDA.error = "Nombre de usuario o contraseña incorrectos. Inténtelo de nuevo. " + ex.Message;
            }
            return userDA;
        }
        private int ValidaIntranet(string usuario, string contraseña, string loginintra, UsuarioLDAPModel userDA)
        {
            int iStatus = 0;
            try
            {
                HttpWebRequest request_ = (HttpWebRequest)WebRequest.Create(loginintra);
                request_.PreAuthenticate = true;

                var cc = new CredentialCache();
                cc.Add(new Uri(loginintra), "NTLM", new NetworkCredential(usuario, contraseña, "Bancolombia"));
                request_.Credentials = cc;

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

                HttpWebResponse response_ = (HttpWebResponse)request_.GetResponse();
                iStatus = (int)response_.StatusCode;
            }
            catch (PrincipalServerDownException PriSerDwnEx)
            {
                userDA.existe = false;
                userDA.error = "Error al intentar conectarse a intranet " + PriSerDwnEx.Message + "  " + PriSerDwnEx.InnerException.Message;
            }
            catch (Exception ex)
            {
                userDA.existe = false;
                userDA.error = "Error al intentar conectarse a intranet " + ex.Message;
            }
            return iStatus;
        }
        private static void ValidaUsuario(string usuario, UsuarioLDAPModel userDA, string usuarioNuevoValidar = null)
        {
            try
            {
                userDA.userNumber = (System.String.IsNullOrEmpty(usuarioNuevoValidar) ? usuario : usuarioNuevoValidar);
                //'Creamos un objeto DirectoryEntry para conectarnos al directorio activo
                var adsRoot = new DirectoryEntry("LDAP://" + Environment.GetEnvironmentVariable("USERDOMAIN"));
                //'Creamos un objeto DirectorySearcher para hacer una búsqueda en el directorio activo
                var adsSearch = new DirectorySearcher(adsRoot);
                //'Ponemos como filtro para que busque el usuario 
                adsSearch.Filter = "samAccountName=" + userDA.userNumber;
                //'Extraemos la primera coincidencia
                if (adsSearch.FindOne() != null)
                {
                    SearchResult oResult = adsSearch.FindOne();
                    var mailProperty = oResult.Properties["mail"];

                    //'Obtenemos el objeto de ese usuario
                    var usuarioDirectorio = oResult.GetDirectoryEntry();
                    userDA.nombreCompleto = usuarioDirectorio.Name;
                    userDA.mail = mailProperty != null ? mailProperty[0].ToString() : "";
                    userDA.existe = true;
                    //'Obtenemos la lista de SID de los grupos a los que pertenece
                    usuarioDirectorio.RefreshCache();
                }
            }
            catch (Exception ex)
            {
                userDA.existe = false;
                userDA.error = "Nombre de usuario o contraseña incorrectos. Inténtelo de nuevo. " + ex.Message;
            }
        }
        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            throw new NotImplementedException();
        }
    }

}
