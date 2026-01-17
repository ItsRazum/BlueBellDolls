namespace BlueBellDolls.Bot.Types
{
    public class TinyApiClientBase(IHttpClientFactory httpClientFactory)
    {
        protected HttpClient HttpClient { get; } = httpClientFactory.CreateClient(Constants.BlueBellDollsHttpClientName);
    }
}