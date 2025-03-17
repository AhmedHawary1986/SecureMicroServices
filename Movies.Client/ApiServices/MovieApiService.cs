using Movies.Client.Models;
using Duende.IdentityModel.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movies.Client.ApiServices
{
    public class MovieApiService : IMovieApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovieApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<Movie> CreateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> GetMovie(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            Task<IEnumerable<Movie>> moviesList = GetMoviesFromAPIFrommHttpClientFactory();
            return await moviesList;
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Movie>> GetMoviesFromAPI()
        {
            var apiCredentials = new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5005/connect/token",
                ClientId = "movieClient",
                ClientSecret = "secret",
                Scope = "movieAPI"
            };

            var client = new HttpClient();
            var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if (discoveryDocument.IsError)
            {

                return null;
            }
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiCredentials);

            if (tokenResponse.IsError)
            {

                return null;
            }
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:5001/api/movies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            //movies.Add(new Movie
            //{
            //    Id = 1,
            //    Genre = "Drama",
            //    Title = "The Shawshank Redemption",
            //    Rating = "9.3",
            //    ImageUrl = "images/src",
            //    ReleaseDate = new DateTime(1994, 5, 5),
            //    Owner = "alice"
            //});

            return await Task.FromResult(movies);
        }

        public async Task<IEnumerable<Movie>> GetMoviesFromAPIFrommHttpClientFactory()
        {
           var httpClient = _httpClientFactory.CreateClient("MoviesClient");
           var request = new HttpRequestMessage(HttpMethod.Get, "/movies");
            var response = await httpClient.SendAsync(request,HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            return await Task.FromResult(movies);
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            var idpClient =  _httpClientFactory.CreateClient("IDPClient");
            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();
            if(metaDataResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the discovery document");
            };
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoResponse = await idpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });

            if(userInfoResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the user information");
            }
            var userInfoDictionary = new Dictionary<string, string>();

            foreach(var claim in userInfoResponse.Claims)
            {
                userInfoDictionary.Add(claim.Type, claim.Value);
            }

            return new UserInfoViewModel(userInfoDictionary);
     

           
        }
    }
}
