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

        private string UserSender = "User Ogranization Email";
        private string Recipient = "Recipient email";
        private string CCRecipient = "Recipient email";

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
					Subject = "Hello World Program",
					Body = new ItemBody
					{
						ContentType = BodyType.Html,
						Content = "<center><h1>Hello World</h1></center>"
					},
					ToRecipients = new List<Recipient>()
					{
						new Recipient
						{
							EmailAddress = new EmailAddress
							{
								Address = Recipient
							}
						}
					},
					CcRecipients = new List<Recipient>()
					{
						new Recipient
						{
							EmailAddress = new EmailAddress
							{
								Address = CCRecipient
							}
						}
					},
					From = new Recipient
                    {
						EmailAddress = new EmailAddress
                        {
							Address = UserSender
						}
                    }
				};

				var saveToSentItems = false;

				await graphClient.Users[UserSender] // Object ID looks like this d1234a56-7890-12bc-ae12-0d12345d6c78, User like this example@domain.com
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
            try
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

                var inbox = await graphClient.Users[Recipient].Messages
                            .Request().Top(1)
                            .Select("id,subject,bodyPreview,from") // for more about message properties https://learn.microsoft.com/en-us/graph/api/resources/message?view=graph-rest-1.0
                            .GetAsync();
                #endregion

                return inbox;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with exception: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
		}

        public async Task<string> DeleteMessage(string id)
        {
            try
            {
                #region MSAL
                var graphClient = await GetAuthenticationMSAL();

                await graphClient.Users[Recipient].Messages[id]
                            .Request()
                            .DeleteAsync();

                return "Successfully deleted";
                #endregion
            }
            catch (Exception ex)
            {
                return "Failed to delete. Exception: " + ex.Message;
            }
        }

        private GraphServiceClient GetAccessTokenADAL(string tenantId, string clientId, string clientSecret, string[] scopes)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("Error with exception: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }

        private async Task<GraphServiceClient> GetAuthenticationMSAL()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("Error with exception: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                return null;
            }
        }
    }
}
