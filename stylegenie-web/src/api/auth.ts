// src/api/auth.ts
import { apiRequest } from "../api/client";

export type RegisterUserRequest = {
    email: string;
    password: string;
    fullName?: string;
};

export type UserResponse = {
    id: number;
    email: string;
    fullName?: string | null;
    isVerified: boolean;
    isActive: boolean;
    tenantId: number;
    roleId?: number;
    role?: string | null;
    createdAt: string; // ISO string
};

export type LoginResponse = {
    token: string;
    user: UserResponse;
};

export const authApi = {
    register: (payload: RegisterUserRequest) =>
        apiRequest<UserResponse>("/api/RegisterNewAccount", {
            method: "POST",
            body: payload,
        }),

    loginWithEmail: (email: string, password: string) =>
        apiRequest<LoginResponse>("/api/Login/normal", {
            method: "POST",
            body: { email, password },
        }),

    // ✅ Đồng bộ backend: /api/Login/google, truyền { idToken }
    loginWithGoogle: (idToken: string) =>
        apiRequest<LoginResponse>("/api/Login/google", {
            method: "POST",
            body: { idToken },
        }),

    // ✅ Kiểm tra phương thức login cho email
    checkLoginMethod: (email: string) =>
        apiRequest<{ method: "google" | "normal" }>(
            "/api/Login/check-method?email=" + encodeURIComponent(email),
            { method: "GET" }
        ),

    // ✅ Xác thực email theo VerifiedController (query string)
    verifyEmail: (email: string, otp: string) =>
        apiRequest<{ ok?: boolean; message?: string }>(
            "/api/Verified/verify-otp?email=" + encodeURIComponent(email) + "&otp=" + encodeURIComponent(otp),
            { method: "POST", body: {} } // body rỗng; backend chỉ đọc query
        ),

    // ✅ Gửi lại OTP
    resendOtp: (email: string) =>
        apiRequest<{ ok?: boolean; message?: string }>(
            "/api/Verified/send-otp?email=" + encodeURIComponent(email),
            { method: "POST", body: {} }
        ),
};
