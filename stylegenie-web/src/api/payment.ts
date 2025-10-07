import { apiRequest } from "../api/client";

export type CreatePaymentRequest = {
    userId: number;
    planId: number;
    cancelUrl: string;
    returnUrl: string;
};

export type CreatePaymentResponse = {
    code: string;
    message: string;
    checkoutUrl: string;
    transactionId: number;
    planName: string;
    price: number;
};

export type TransactionResponse = {
    id: number;
    userId: number;
    planId: number;
    amount: number;
    status: string;
    payOsPaymentLinkId?: string | null;
    payOsCheckoutUrl?: string | null;
    createdAt: string;
};

export const paymentApi = {
    // ✅ Tạo thanh toán mới (PayOS link)
    createPayment: (payload: CreatePaymentRequest) =>
        apiRequest<CreatePaymentResponse>("/api/PayOS/create", {
            method: "POST",
            body: payload,
        }),

    // ✅ Danh sách giao dịch
    getTransactions: () =>
        apiRequest<TransactionResponse[]>("/api/PayOS/list", {
            method: "GET",
        }),

    // ✅ Gọi webhook test thủ công (nếu cần)
    testWebhook: (body: any) =>
        apiRequest("/api/PayOS/webhook", {
            method: "POST",
            body,
        }),

    // ✅ Kiểm tra trạng thái trả về (PAID / FAILED)
    checkReturnStatus: (orderCode: number, status: string) =>
        apiRequest<{ message: string; status: string }>(
            `/api/PayOS/return?orderCode=${orderCode}&status=${status}`,
            { method: "GET" }
        ),

    // ✅ Xác nhận thanh toán thành công (frontend gọi khi PayOS redirect về success)
    confirmSuccess: (orderCode: number) =>
        apiRequest<{ message: string; status: string }>(
            `/api/PayOS/return?orderCode=${orderCode}&status=PAID`,
            { method: "GET" }
        ),

    // ✅ Xác nhận thanh toán bị hủy hoặc thất bại
    confirmCancel: (orderCode: number) =>
        apiRequest<{ message: string; status: string }>(
            `/api/PayOS/return?orderCode=${orderCode}&status=FAILED`,
            { method: "GET" }
        ),
};
