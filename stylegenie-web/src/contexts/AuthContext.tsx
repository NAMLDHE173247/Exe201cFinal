import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import type { UserResponse } from "../api/auth";
import Cookies from "js-cookie"; // ✅ Dùng cookie để lưu token an toàn

// ======================= KIỂU DỮ LIỆU =======================
type AuthContextType = {
    user: UserResponse | null;
    token: string | null;
    login: (token: string, user: UserResponse) => void;
    logout: () => void;
};

// ======================= CONTEXT KHỞI TẠO =======================
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// ======================= PROVIDER CHÍNH =======================
export const AuthProvider: React.FC<React.PropsWithChildren> = ({ children }) => {
    const [user, setUser] = useState<UserResponse | null>(null);
    const [token, setToken] = useState<string | null>(null);

    // ✅ Hàm login: lưu token + user vào cookie / session
    const login = (t: string, u: UserResponse) => {
        setToken(t);
        setUser(u);
        localStorage.setItem("authToken", t);
        Cookies.set("AuthToken", t, { expires: 1 }); // 1 ngày
        sessionStorage.setItem("AuthUser", JSON.stringify(u));
    };

    // ✅ Hàm logout: xóa token & dữ liệu user
    const logout = () => {
        setToken(null);
        setUser(null);
        Cookies.remove("AuthToken");
        localStorage.removeItem("authToken");
        sessionStorage.clear();
    };

    // ✅ Khi load lại trang → khôi phục token và user
    useEffect(() => {
        const existingToken = Cookies.get("AuthToken");
        const storedUser = sessionStorage.getItem("AuthUser");
        if (existingToken) setToken(existingToken);
        if (storedUser) {
            try {
                const parsed = JSON.parse(storedUser) as UserResponse;
                setUser(parsed);
            } catch {
                sessionStorage.removeItem("AuthUser");
            }
        }
    }, []);

    const value = useMemo<AuthContextType>(() => ({ user, token, login, logout }), [user, token]);

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// ✅ Custom hook để sử dụng ở mọi nơi
export const useAuth = (): AuthContextType => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
    return ctx;
};
