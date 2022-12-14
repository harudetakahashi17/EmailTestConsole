using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace EmailTestConsole
{
    public class O365MailEngine
    { 
        const string ClientId = "Your Client ID";
        const string TenantId = "Your Tenant ID";
        const string ClientSecret = "Your Client Secret";
        private string[] Scope = { "https://graph.microsoft.com/.default" };

        //MSAL
        private IConfidentialClientApplication app;
        const string Authority = "https://login.microsoftonline.com/";

        public O365MailEngine()
        {
            app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithClientSecret(ClientSecret)
                .WithAuthority(Authority + TenantId)
                .Build();
        }

        public async Task<string> SendMessage()
        {
            try
			{
                #region ADAL
                //var graphClient = GetAccessTokenADAL(TenantId, ClientId, ClientSecret, Scope);
                #endregion

                #region MSAL
                var graphClient = await GetAuthenticationMSAL();
                #endregion

                var message = new Message
				{
					Subject = "Meet for lunch?",
					Body = new ItemBody
					{
						ContentType = BodyType.Html,
						Content = "The new cafeteria is open."
					},
					ToRecipients = new List<Recipient>()
					{
						new Recipient
						{
							EmailAddress = new EmailAddress
							{
								Address = "example@yopmail.com"
							}
						}
					},
					CcRecipients = new List<Recipient>()
					{
						new Recipient
						{
							EmailAddress = new EmailAddress
							{
								Address = "example@gmail.com"
							}
						}
					},
					From = new Recipient
                    {
						EmailAddress = new EmailAddress
                        {
							Address = "example@yourcompany.com"
						}
                    }
				};

				var saveToSentItems = false;

				await graphClient.Users["{Object ID / User}"] // Object ID looks like this d1234a56-7890-12bc-ae12-0d12345d6c78, User like this example@domain.com
                    .SendMail(message, saveToSentItems)
					.Request()
					.PostAsync();

				return "Success To Send Mail";
			}
            catch (Exception ex)
			{
				return $"Exception: {ex.Message}";
			}
		}

		public async Task<IUserMessagesCollectionPage> GetInbox()
        {
            #region ADAL
            //         var graphClient = GetAccessTokenADAL(TenantId, ClientId, ClientSecret, Scope);

            //var inbox = await graphClient.Users["{Object ID / User}"].Messages
            //			.Request().Top(10)
            //			.Select("sender,subject,bodyPreview,from")
            //			.GetAsync();
            #endregion

            #region MSAL
            var graphClient = await GetAuthenticationMSAL();

            var inbox = await graphClient.Users["{Object ID / User}"].Messages
                        .Request().Top(10)
                        .Select("id,subject,bodyPreview,from") // for more about message properties https://learn.microsoft.com/en-us/graph/api/resources/message?view=graph-rest-1.0
                        .GetAsync();
            #endregion

            return inbox;
		}

        private GraphServiceClient GetAccessTokenADAL(string tenantId, string clientId, string clientSecret, string[] scopes)
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
			return graphClient;
        }

        public async Task<GraphServiceClient> GetAuthenticationMSAL()
        {
            if (app == null)
            {
                app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithClientSecret(ClientSecret)
                .WithAuthority(Authority + TenantId)
                .Build();
            }

            var authResult = await app.AcquireTokenForClient(Scope)
                .WithTenantId(TenantId)
                .ExecuteAsync()
                .ConfigureAwait(false);

            var authProvider = new DelegateAuthenticationProvider(req =>
            {
                req.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                return Task.FromResult(0);
            });

            var graphClient = new GraphServiceClient(authProvider);

            return graphClient;
        }
    }
}
