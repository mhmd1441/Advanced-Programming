namespace TaskManagementMvc.Services
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request);
        Task DeleteAsync(string endpoint);
    }
}
