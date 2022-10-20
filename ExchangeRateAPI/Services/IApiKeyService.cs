namespace ExchangeRateAPI.Services
{
    public interface IApiKeyService
    {
        Task<string> GenerateApiKeyAsync();
    }
}