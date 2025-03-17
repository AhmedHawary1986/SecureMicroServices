using Duende.IdentityModel.Client;

namespace Movies.Client.HttpHandlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ClientCredentialsTokenRequest _clientCredentialsTokenRequest;

        public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest clientCredentialsTokenRequest)
        {
            _httpClientFactory = httpClientFactory;
            _clientCredentialsTokenRequest = clientCredentialsTokenRequest;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("IDPClient");
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(_clientCredentialsTokenRequest);
            if(tokenResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }
            request.SetBearerToken(tokenResponse.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
