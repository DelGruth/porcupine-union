using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserAccessSystem.Contract.Clients;

public class ApiClient(HttpClient httpClient)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.Preserve,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string CLIENT_NAME = "UserAccessApi";

    public async Task<Response<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var httpResponse = await httpClient.GetAsync(endpoint);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return new Response<T>(
                    ErrorCode.UnexpectedError,
                    $"HTTP error: {httpResponse.StatusCode}"
                );
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<Response<T>>(content, _jsonOptions);
            return response;
        }
        catch (JsonException ex)
        {
            return new Response<T>(
                ErrorCode.UnexpectedError,
                $"JSON deserialization error: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new Response<T>(ErrorCode.UnexpectedError, ex.Message);
        }
    }

    public async Task<Response<T>> PostAsync<T>(string endpoint, object request)
    {
        try
        {
            var httpResponse = await httpClient.PostAsJsonAsync(endpoint, request, _jsonOptions);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return new Response<T>(
                    ErrorCode.UnexpectedError,
                    $"HTTP error: {httpResponse.StatusCode}"
                );
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<Response<T>>(content, _jsonOptions);
            return response;
        }
        catch (Exception ex)
        {
            return new Response<T>(ErrorCode.UnexpectedError, ex.Message);
        }
    }

    public async Task<Response<T>> DeleteAsync<T>(string endpoint)
    {
        try
        {
            var httpResponse = await httpClient.DeleteAsync(endpoint);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return new Response<T>(
                    ErrorCode.UnexpectedError,
                    $"HTTP error: {httpResponse.StatusCode}"
                );
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<Response<T>>(content, _jsonOptions);
            return response;
        }
        catch (Exception ex)
        {
            return new Response<T>(ErrorCode.UnexpectedError, ex.Message);
        }
    }
}
