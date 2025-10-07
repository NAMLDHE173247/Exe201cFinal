import { useEffect, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import Cookies from "js-cookie";
import { adminService } from "../api/adminservice";

interface ApiKeyResponse {
    id: number;
    apiKey: string;
    credits: number;
    updatedAt: string;
}

export default function ApiKeyUpdate() {
    const [apiKey, setApiKey] = useState("");
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState("");
    const [lastUpdated, setLastUpdated] = useState<string | null>(null);

    const navigate = useNavigate(); // ✅ để điều hướng

    // 🔹 Kiểm tra token khi vào trang
    useEffect(() => {
        const token = Cookies.get("AuthToken");
        if (!token) {
            setMessage("⚠️ Bạn chưa đăng nhập. Đang chuyển hướng...");
            setTimeout(() => navigate("/login"), 2000); // ⏳ sau 2 giây về login
            return;
        }

        // Nếu có token → gọi API
        adminService
            .getCurrentApiKey()
            .then((res: ApiKeyResponse) => {
                setApiKey(res.apiKey);
                setLastUpdated(res.updatedAt);
            })
            .catch(() => setMessage("❌ Không thể tải API Key hiện tại."));
    }, [navigate]);

    // 🔹 Gửi yêu cầu cập nhật
    const handleUpdate = async () => {
        if (!apiKey.trim()) {
            setMessage("⚠️ API Key không được để trống.");
            return;
        }

        setLoading(true);
        try {
            const res: ApiKeyResponse = await adminService.updateApiKey(apiKey);
            setMessage("✅ Cập nhật thành công!");
            setLastUpdated(res.updatedAt);
        } catch {
            setMessage("❌ Lỗi khi cập nhật API Key.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div
            style={{
                maxWidth: 500,
                margin: "60px auto",
                padding: 24,
                border: "1px solid #ddd",
                borderRadius: 12,
                backgroundColor: "#fff",
                boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
            }}
        >
            <h2 style={{ textAlign: "center" }}>🔐 Cập nhật API Key</h2>

            {message && (
                <p style={{ marginTop: 16, textAlign: "center", color: "#444" }}>{message}</p>
            )}

            {!Cookies.get("AuthToken") ? (
                <p style={{ textAlign: "center", marginTop: 12 }}>
                    <Link to="/login" style={{ color: "#1677ff" }}>
                        → Đăng nhập ngay
                    </Link>
                </p>
            ) : (
                <>
                    <label style={{ display: "block", marginTop: 16 }}>API Key hiện tại</label>
                    <input
                        value={apiKey}
                        onChange={(e) => setApiKey(e.target.value)}
                        placeholder="Nhập API Key mới..."
                        style={{
                            width: "100%",
                            padding: "10px",
                            marginTop: "6px",
                            border: "1px solid #ccc",
                            borderRadius: "6px",
                            fontFamily: "monospace",
                        }}
                    />

                    <button
                        onClick={handleUpdate}
                        disabled={loading}
                        style={{
                            width: "100%",
                            marginTop: 24,
                            padding: "10px 16px",
                            border: "none",
                            borderRadius: "6px",
                            backgroundColor: "#1677ff",
                            color: "#fff",
                            fontWeight: "bold",
                            cursor: "pointer",
                        }}
                    >
                        {loading ? "Đang cập nhật..." : "Cập nhật API Key"}
                    </button>

                    {lastUpdated && (
                        <p style={{ marginTop: 8, textAlign: "center", color: "#888" }}>
                            Cập nhật lần cuối: {new Date(lastUpdated).toLocaleString()}
                        </p>
                    )}
                </>
            )}
        </div>
    );
}
