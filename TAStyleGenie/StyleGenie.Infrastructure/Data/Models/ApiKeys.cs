namespace StyleGenie.Infrastructure.Data.Models
{
    public class ApiKeys  // Đổi tên từ ApiKey thành ApiKeys cho thống nhất
    {
        public long Id { get; set; }  // Identity column
        public string ApiKey { get; set; } = default!;  // Mã hóa API key
        public int Credits { get; set; } = 0;  // Credits (số lượt sử dụng)
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Thời gian cập nhật
        public string? UpdatedBy { get; set; }  // Người cập nhật key
    }
}
