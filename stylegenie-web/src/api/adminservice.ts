import { apiRequest } from "../api/client";
import Cookies from "js-cookie";


// Kiểu dữ liệu của một người dùng trong danh sách
export type AdminUserInfo = {
    id: number;
    email: string;
    fullName: string | null;
    tenantId: number;
    tenantName: string;
    roleName: string;
    isActive: boolean;
    isVerified: boolean;
    balance: number;
    createdAt: string;
};

// Thông tin thống kê trạng thái
export type UserStatusSummary = {
    totalUsers: number;
    activeCount: number;
    inactiveCount: number;
    verifiedCount: number;
    unverifiedCount: number;
    byRole: { role: string; count: number }[];
};

// Toàn bộ response trả về từ API
export type UserListResponse = {
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    statusSummary: UserStatusSummary;
    data: AdminUserInfo[];
};

export const adminService = {
    // ✅ Lấy danh sách người dùng (có filter, search, pagination)
    getAllUsers: (params?: {
        search?: string;
        role?: string;
        isActive?: boolean;
        isVerified?: boolean;
        page?: number;
        pageSize?: number;
    }) => {
        const query = new URLSearchParams();
        if (params?.search) query.append("search", params.search);
        if (params?.role) query.append("role", params.role);
        if (params?.isActive !== undefined) query.append("isActive", String(params.isActive));
        if (params?.isVerified !== undefined) query.append("isVerified", String(params.isVerified));
        if (params?.page) query.append("page", String(params.page));
        if (params?.pageSize) query.append("pageSize", String(params.pageSize));
        const url = "/api/adminmanageraccount" + (query.toString() ? "?" + query.toString() : "");
        return apiRequest<UserListResponse>(url, { method: "GET" });
    },

    // ✅ Cập nhật Role
    updateUserRole: (id: number, newRole: string) =>
        apiRequest(`/api/adminmanageraccount/${id}/role`, {
            method: "PUT",
            body: newRole,
            headers: { "Content-Type": "application/json" },
        }),
    // ✅ Cập nhật trạng thái hoạt động (Frontend MỚI)
    updateUserActive: (id: number, isActive: boolean) =>
        apiRequest(`/api/adminmanageraccount/${id}/active`, {
            method: "PUT",
            // 💡 Gửi chuỗi "true" hoặc "false"
            body: JSON.stringify(isActive),
            headers: { "Content-Type": "application/json" },
        }),

// ✅ Cập nhật trạng thái xác minh (Frontend MỚI)
    updateUserVerified: (id: number, isVerified: boolean) =>
        apiRequest(`/api/adminmanageraccount/${id}/verified`, {
            method: "PUT",
            // 💡 Gửi chuỗi "true" hoặc "false"
            body: JSON.stringify(isVerified),
            headers: { "Content-Type": "application/json" },
        }),




    // ✅ Cập nhật API Key
    updateApiKey: (newKey: string) => {
        if (!Cookies.get("AuthToken")) throw new Error("Unauthorized: Please login first.");
        return apiRequest("/api/ManagerApiKey/update", {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: { apiKey: newKey },
        });
    },


// ✅ Lấy API Key hiện tại
    getCurrentApiKey: () =>
        apiRequest("/api/ManagerApiKey/current", {
            method: "GET",
        }),



};

