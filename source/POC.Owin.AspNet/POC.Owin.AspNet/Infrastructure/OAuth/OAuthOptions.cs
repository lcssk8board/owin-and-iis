using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System;

namespace POC.Owin.AspNet.Infrastructure
{
    public class OAuthOptions : OAuthAuthorizationServerOptions
    {
        public OAuthOptions()
        {
            TokenEndpointPath = new PathString("/token");
            AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60);
            AccessTokenFormat = new JwtFormat(this);
            Provider = new OAuthProvider();
            AllowInsecureHttp = true;
        }
    }
}