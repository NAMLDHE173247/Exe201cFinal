namespace StyleGenie.Application.Credentials
{
    public interface ICredentialStore
    {
        Task SetAsync(long? tenantId, string provider, string keyName, string secretPlain, string? updatedBy);
        Task<string?> GetAsync(long? tenantId, string provider, string keyName);

        // Thêm phương thức SetKeyAsync
        Task SetKeyAsync(string provider, string keyName, string secretPlain, int credits, string updatedBy);

        Task<string?> GetApiKeyAsync();
    }
}
