import Cookies from "js-cookie";
import { message } from "antd";
import { useEffect } from "react";
import React from "react";
import { jwtDecode } from "jwt-decode";

// Định nghĩa kiểu cho payload của JWT
type JwtPayload = {
    role_id?: string;
    exp?: number;
};

interface PrivateRouteProps {
    children: React.ReactElement;
    allowedRoles?: string[]; // ví dụ: ["1", "2"] -> admin + staff
}

export default function PrivateRoute({ children, allowedRoles }: PrivateRouteProps) {
    const token = Cookies.get("AuthToken");

    useEffect(() => {
        if (!token) {
            message.warning("⚠️ Bạn chưa đăng nhập. Đang chuyển hướng...");
            setTimeout(() => (window.location.href = "/login"), 1500);
            return;
        }

        try {
            const decoded = jwtDecode<JwtPayload>(token);
            const userRole = decoded.role_id;

            // Nếu token hết hạn
            if (decoded.exp && decoded.exp * 1000 < Date.now()) {
                message.error("⏰ Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.");
                Cookies.remove("AuthToken");
                setTimeout(() => (window.location.href = "/login"), 1500);
                return;
            }

            // Nếu có allowedRoles và vai trò không hợp lệ
            if (allowedRoles && !allowedRoles.includes(userRole ?? "")) {
                message.error("🚫 Bạn không có quyền truy cập trang này!");
                // Gợi ý điều hướng theo vai trò
                if (userRole === "1") window.location.href = "/admin-home";
                else if (userRole === "2") window.location.href = "/staff-home";
                else if (userRole === "3") window.location.href = "/user-home";
                else window.location.href = "/login";
            }
        } catch (err) {
            console.error("Invalid token:", err);
            message.error("❌ Token không hợp lệ. Vui lòng đăng nhập lại.");
            Cookies.remove("AuthToken");
            setTimeout(() => (window.location.href = "/login"), 1500);
        }
    }, [token, allowedRoles]);

    // Nếu không có token hoặc đang bị redirect, không render gì
    if (!token) return null;

    return children;
}
