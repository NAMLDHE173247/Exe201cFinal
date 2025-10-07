import Cookies from "js-cookie";

// =================== CẤU HÌNH CƠ BẢN ===================
export const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5082";

// =================== HÀM XỬ LÝ PHẢN HỒI ===================
async function handle<T>(res: Response): Promise<T> {
    const text = await res.text();

    // ❌ Nếu phản hồi lỗi (400–500)
    if (!res.ok) {
        try {
            const err = JSON.parse(text);
            throw new Error(err?.message ?? text);
        } catch {
            throw new Error(text || res.statusText);
        }
    }

    // ✅ Nếu phản hồi rỗng
    if (!text) return {} as T;

    // ✅ Nếu có JSON
    try {
        return JSON.parse(text) as T;
    } catch {
        return text as unknown as T;
    }
}

// =================== HÀM CHÍNH GỌI API ===================
export async function apiRequest<T>(
    path: string,
    options?: { method?: string; body?: any; headers?: Record<string, string> }
): Promise<T> {
    const { method = "GET", body, headers } = options ?? {};

    // ✅ Lấy token từ cookie (do AuthContext lưu)
    const token = Cookies.get("AuthToken");

    // ✅ Chuẩn hóa header
    const finalHeaders: HeadersInit = {
        "Content-Type": "application/json",
        ...(headers ?? {}),
    };

    // ✅ Gắn Authorization nếu có token
    if (token) {
        finalHeaders["Authorization"] = `Bearer ${token}`;
    }

    // ✅ Gửi request
    const res = await fetch(`${API_BASE}${path}`, {
        method,
        headers: finalHeaders,
        body:
            body && typeof body !== "string"
                ? JSON.stringify(body)
                : body, // cho phép body là FormData hoặc JSON string
    });

    // ⚠️ Kiểm tra token hết hạn
    if (res.status === 401) {
        console.warn("⚠️ Token hết hạn hoặc không hợp lệ. Vui lòng đăng nhập lại.");
    }

    return handle<T>(res);
}

// =================== TIỆN ÍCH NGẮN GỌN ===================
export async function apiGet<T>(path: string): Promise<T> {
    return apiRequest<T>(path, { method: "GET" });
}

export async function apiPost<T>(path: string, body: any): Promise<T> {
    return apiRequest<T>(path, { method: "POST", body });
}

export async function apiPut<T>(path: string, body: any): Promise<T> {
    return apiRequest<T>(path, { method: "PUT", body });
}

export async function apiDelete<T>(path: string): Promise<T> {
    return apiRequest<T>(path, { method: "DELETE" });
}
