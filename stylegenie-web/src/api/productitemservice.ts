import { apiRequest } from "../api/client";

// ====== Types ======
export type ProductItem = {
    id: number;
    name: string;
    price: number | null;        // backend: decimal?
    material?: string | null;    // Phong cách
    category?: string | null;    // Category
    affiliateUrl?: string | null;
    isVipOnly: boolean;
    isActive: boolean;
    createdAt: string;
    imageBase64?: string | null; // đã có prefix data:image/*
};

export type GroupCount = { category?: string; material?: string; count: number };

export type ProductStatusSummary = {
    totalItems: number;
    activeCount: number;
    inactiveCount: number;
    vipCount: number;
    normalCount: number;
    byCategory: { category: string; count: number }[];
    byMaterial: { material: string; count: number }[];
};

export type ProductListResponse = {
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    statusSummary: ProductStatusSummary;
    data: ProductItem[];
    filters?: { materials: string[]; categories: string[] };
};

export type FilterResponse = {
    materials: string[];
    categories: string[];
};

// ✅ DTO tạo mới
export type CreateItemDto = {
    name: string;
    price?: number | null;
    material?: string;
    category?: string;
    affiliateUrl?: string;
    isVipOnly?: boolean;
    imageFile?: File | null;
};

// ✅ DTO trả về khi xoá
export type DeleteResponse = { ok: boolean; id: number; status?: string };

// ====== Service ======
export const productItemService = {
    // GET /api/ManagerProduct/filters
    getFilters: () =>
        apiRequest<FilterResponse>("/api/ManagerProduct/filters", { method: "GET" }),

    // GET /api/ManagerProduct
    getAllItems: (params?: {
        search?: string;
        material?: string;
        category?: string;
        isVipOnly?: boolean;
        isActive?: boolean;
        page?: number;
        pageSize?: number;
        includeFilters?: boolean;
    }) => {
        const qs = new URLSearchParams();
        if (params?.search) qs.set("search", params.search);
        if (params?.material) qs.set("material", params.material);
        if (params?.category) qs.set("category", params.category);
        if (params?.isVipOnly !== undefined) qs.set("isVipOnly", String(params.isVipOnly));
        if (params?.isActive !== undefined) qs.set("isActive", String(params.isActive));
        qs.set("page", String(params?.page ?? 1));
        qs.set("pageSize", String(params?.pageSize ?? 12));
        if (params?.includeFilters) qs.set("includeFilters", "true");

        const url = "/api/ManagerProduct" + (qs.toString() ? `?${qs.toString()}` : "");
        return apiRequest<ProductListResponse>(url, { method: "GET" });
    },

    // GET /api/ManagerProduct/{id}
    getItem: (id: number) =>
        apiRequest<ProductItem>(`/api/ManagerProduct/${id}`, { method: "GET" }),

    // POST /api/ManagerProduct (multipart/form-data)
    createItem: async (payload: CreateItemDto) => {
        const fd = new FormData();
        fd.append("name", payload.name ?? "");
        if (payload.price !== undefined && payload.price !== null) fd.append("price", String(payload.price));
        if (payload.material) fd.append("material", payload.material);
        if (payload.category) fd.append("category", payload.category);
        if (payload.affiliateUrl) fd.append("affiliateUrl", payload.affiliateUrl);
        fd.append("isVipOnly", String(!!payload.isVipOnly));
        if (payload.imageFile) fd.append("imageFile", payload.imageFile);

        const token =
            (typeof window !== "undefined" && (localStorage.getItem("token") || sessionStorage.getItem("token"))) ||
            undefined;

        const res = await fetch("/api/ManagerProduct", {
            method: "POST",
            body: fd,
            headers: token ? { Authorization: `Bearer ${token}` } : undefined,
            credentials: "include",
        });

        if (!res.ok) throw new Error((await res.text().catch(() => "")) || `HTTP ${res.status}`);
        return (await res.json()) as ProductItem;
    },

    // PUT /api/ManagerProduct/{id} (multipart/form-data)
    updateItem: async (
        id: number,
        payload: {
            name?: string;
            price?: number | null;
            material?: string;
            category?: string;
            affiliateUrl?: string;
            isVipOnly?: boolean;
            isActive?: boolean;
            imageFile?: File | null;
            removeImage?: boolean;
        }
    ) => {
        const fd = new FormData();
        if (payload.name !== undefined) fd.append("name", payload.name);
        if (payload.price !== undefined && payload.price !== null) fd.append("price", String(payload.price));
        if (payload.material !== undefined) fd.append("material", payload.material ?? "");
        if (payload.category !== undefined) fd.append("category", payload.category ?? "");
        if (payload.affiliateUrl !== undefined) fd.append("affiliateUrl", payload.affiliateUrl ?? "");
        if (payload.isVipOnly !== undefined) fd.append("isVipOnly", String(payload.isVipOnly));
        if (payload.isActive !== undefined) fd.append("isActive", String(payload.isActive));
        if (payload.removeImage) fd.append("removeImage", "true");
        if (payload.imageFile) fd.append("imageFile", payload.imageFile);

        const token =
            (typeof window !== "undefined" && (localStorage.getItem("token") || sessionStorage.getItem("token"))) ||
            undefined;

        const res = await fetch(`/api/ManagerProduct/${id}`, {
            method: "PUT",
            body: fd,
            headers: token ? { Authorization: `Bearer ${token}` } : undefined,
            credentials: "include",
        });

        if (!res.ok) throw new Error((await res.text().catch(() => "")) || `HTTP ${res.status}`);
        return (await res.json()) as ProductItem;
    },

    // DELETE /api/ManagerProduct/{id}  — Soft delete (IsActive=false)
    softDeleteItem: (id: number) =>
        apiRequest<DeleteResponse>(`/api/ManagerProduct/${id}`, { method: "DELETE" }),

    // DELETE /api/ManagerProduct/{id}/hard — Hard delete (xoá hẳn)
    hardDeleteItem: (id: number) =>
        apiRequest<DeleteResponse>(`/api/ManagerProduct/${id}/hard`, { method: "DELETE" }),
};
