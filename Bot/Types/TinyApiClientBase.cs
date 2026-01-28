using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Types
{
    public class TinyApiClientBase(IHttpClientFactory httpClientFactory)
    {
        protected HttpClient HttpClient { get; } = httpClientFactory.CreateClient(Constants.BlueBellDollsHttpClientName);

        protected async Task<ServiceResult> FromResponse(HttpResponseMessage response, CancellationToken token = default)
        {
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return new ServiceResult((int)response.StatusCode, "NoContent");

                return new((int)response.StatusCode, null);
            }

            string errorText = "Сервер не ответил или отклонил запрос.";

            try
            {
                var errorDto = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: token);
                if (!string.IsNullOrEmpty(errorDto?.Message))
                {
                    errorText = errorDto.Message;
                }
            }
            catch
            {
                errorText = $"Ошибка сервера: {response.ReasonPhrase} ({(int)response.StatusCode})";
            }

            return new((int)response.StatusCode, errorText);
        }

        protected async Task<ServiceResult<T>> FromResponse<T>(HttpResponseMessage response, CancellationToken token = default) where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return new ServiceResult<T>((int)response.StatusCode, "NoContent", null);

                return new((int)response.StatusCode, null, await response.Content.ReadFromJsonAsync<T>(cancellationToken: token));
            }

            string errorText = "Сервер не ответил или отклонил запрос.";

            try
            {
                var errorDto = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: token);
                if (!string.IsNullOrEmpty(errorDto?.Message))
                {
                    errorText = errorDto.Message;
                }
            }
            catch
            {
                errorText = $"Ошибка сервера: {response.ReasonPhrase} ({(int)response.StatusCode})";
            }

            return new((int)response.StatusCode, errorText, null);
        }
    }
}