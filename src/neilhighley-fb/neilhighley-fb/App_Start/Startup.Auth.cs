using System;
using System.Configuration;
using System.Management.Instrumentation;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using neilhighley_fb.Models;

namespace neilhighley_fb
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            string XmlSchemaString = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
            if (ConfigurationManager.AppSettings.Get("Facebook:AppId").Length > 0)
            {
                /* Facebook Auth is in separate non-git config file
            * <?xml version="1.0"?>
             <appSettings>
               <add key="Facebook:AppId" value="xxxxxx" />
               <add key="Facebook:AppSecret" value="xxxxxx" />
               <add key="Facebook:AppNamespace" value="" />
             </appSettings>
           */

                var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
                {
                    AppId = ConfigurationManager.AppSettings.Get("Facebook:AppId"),
                    AppSecret = ConfigurationManager.AppSettings.Get("Facebook:AppSecret"),
                    Provider = new FacebookAuthenticationProvider()
                    {
                        OnAuthenticated = (context) =>
                        {
                            var c = context as FacebookAuthenticatedContext;
                            c.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token",
                                c.AccessToken, XmlSchemaString, "Facebook"));
                            //  context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, XmlSchemaString, "Facebook"));
                            foreach (var x in c.User)
                            {
                                var claimType = string.Format("urn:facebook:{0}", x.Key);
                                string claimValue = x.Value.ToString();
                                if (!c.Identity.HasClaim(claimType, claimValue))
                                    c.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue,
                                        XmlSchemaString, "Facebook"));

                            }

                            return Task.FromResult(0);
                        }
                    }

                };
                var scopes = new[] { "user_status", "email", "user_friends", "user_about_me", "read_stream", "publish_actions" };
                foreach (var s in scopes)
                {
                    facebookOptions.Scope.Add(s);
                }


                app.UseFacebookAuthentication(facebookOptions);

            }

        }
    }
   
        
}