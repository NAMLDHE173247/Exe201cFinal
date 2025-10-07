using Microsoft.EntityFrameworkCore;
using StyleGenie.Application.Credentials;
using StyleGenie.Application.Security;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Infrastructure.Persistence
{
    public sealed class EfCredentialStore : ICredentialStore
    {
        private readonly TryOnDbContext _db;
        private readonly ICrypto _crypto;

        public EfCredentialStore(TryOnDbContext db, ICrypto crypto)
        {
            _db = db;
            _crypto = crypto;
        }

        // Lưu API key vào bảng ApiKeys
        public async Task SetAsync(long? tenantId, string provider, string keyName, string secretPlain, string? updatedBy)
        {
            var cipher = _crypto.Encrypt(secretPlain);  // Mã hóa key

            var row = await _db.ApiCredentials.FirstOrDefaultAsync(x =>
                x.TenantId == tenantId && x.Provider == provider && x.KeyName == keyName);

            if (row == null)
            {
                row = new ApiCredential
                {
                    TenantId = tenantId,
                    Provider = provider,
                    KeyName = keyName,
                    SecretCipher = cipher,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = updatedBy
                };
                _db.ApiCredentials.Add(row);
            }
            else
            {
                row.SecretCipher = cipher;
                row.UpdatedAt = DateTime.UtcNow;
                row.UpdatedBy = updatedBy;
            }
            await _db.SaveChangesAsync();
        }

        // Đọc và giải mã key từ bảng ApiCredentials
        public async Task<string?> GetAsync(long? tenantId, string provider, string keyName)
        {
            var row = await _db.ApiCredentials.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Provider == provider && x.KeyName == keyName)
                     ?? await _db.ApiCredentials.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.TenantId == null && x.Provider == provider && x.KeyName == keyName);

            return row == null ? null : _crypto.Decrypt(row.SecretCipher);
        }

        // Implement phương thức SetKeyAsync để lưu hoặc cập nhật API key toàn hệ thống
        public async Task SetKeyAsync(string provider, string keyName, string secretPlain, int credits, string updatedBy)
        {
            // Lưu trực tiếp key chưa mã hóa vào DB
            var existing = await _db.ApiKeys.FirstOrDefaultAsync(x => x.ApiKey == secretPlain);

            if (existing != null)
            {
                // Cập nhật key nếu đã tồn tại
                existing.ApiKey = secretPlain;  // Lưu key chưa mã hóa
                existing.Credits = credits;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = updatedBy;

                _db.ApiKeys.Update(existing);
            }
            else
            {
                // Thêm mới key nếu chưa có
                var entity = new ApiKeys
                {
                    ApiKey = secretPlain,  // Lưu key chưa mã hóa
                    Credits = credits,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = updatedBy
                };

                _db.ApiKeys.Add(entity);
            }

            await _db.SaveChangesAsync();
        }


        // Đọc và giải mã key từ bảng ApiKeys
        public async Task<string?> GetApiKeyAsync()
        {
            // Lấy key từ bảng ApiKeys (chưa mã hóa)
            var apiKeyRecord = await _db.ApiKeys.FirstOrDefaultAsync();

            if (apiKeyRecord == null)
                return null;

            // Trả về key chưa mã hóa
            return apiKeyRecord.ApiKey;
        }

    }
}
