using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FuncionariosAPIService.Services
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                //çasfjdçjsfllçfjlkf
                string authenticationToken = actionContext.Request
                    .Headers.Authorization.Parameter;
                //macoratti:numsey
                string decodedAuthenticationToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authenticationToken));

                string[] usernamePassordArray = decodedAuthenticationToken.Split(':');

                string username = usernamePassordArray[0];
                string password = usernamePassordArray[1];

                if (Login(username, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(username), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request
                       .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }

        public bool Login(string username, string password)
        {
            if ((username == "trade2up_usr") && (password == "R8u4ZuHMoAFcuLL"))
            {
                return true;
            }
            else if((username == "flag_usr") && (password == "B50PrwImgfqx2cs"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Trade2UPAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                //çasfjdçjsfllçfjlkf
                string authenticationToken = actionContext.Request
                    .Headers.Authorization.Parameter;
                //macoratti:numsey
                string decodedAuthenticationToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authenticationToken));

                string[] usernamePassordArray = decodedAuthenticationToken.Split(':');

                string username = usernamePassordArray[0];
                string password = usernamePassordArray[1];

                if (Login(username, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(username), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request
                       .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }

        public bool Login(string username, string password)
        {
            if ((username == "flag_usr") && (password == "B50PrwImgfqx2cs"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}