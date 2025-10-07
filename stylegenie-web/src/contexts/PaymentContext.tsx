import React, { createContext, useContext, useMemo, useState } from "react";
import { paymentApi, type CreatePaymentRequest, type CreatePaymentResponse, type TransactionResponse } from "../api/payment";

// Kiểu dữ liệu PaymentContext
type PaymentContextType = {
  currentPlanId: number | null;              // Gói đang chọn
  transaction: TransactionResponse | null;   // Giao dịch hiện tại (nếu có)
  isProcessing: boolean;                     // Trạng thái đang tạo thanh toán
  checkoutUrl: string | null;                // URL checkout PayOS
  createPayment: (req: CreatePaymentRequest) => Promise<void>; // Hàm gọi API thanh toán
  clearPayment: () => void;                  // Reset trạng thái thanh toán
};

// Tạo context
const PaymentContext = createContext<PaymentContextType | undefined>(undefined);

export const PaymentProvider: React.FC<React.PropsWithChildren> = ({ children }) => {
  const [currentPlanId, setCurrentPlanId] = useState<number | null>(null);
  const [transaction, setTransaction] = useState<TransactionResponse | null>(null);
  const [isProcessing, setIsProcessing] = useState(false);
  const [checkoutUrl, setCheckoutUrl] = useState<string | null>(null);

  // Hàm tạo thanh toán (gọi backend)
  const createPayment = async (req: CreatePaymentRequest) => {
    try {
      setIsProcessing(true);
      setCurrentPlanId(req.planId);

      const res: CreatePaymentResponse = await paymentApi.createPayment(req);
      if (res.checkoutUrl) {
        setCheckoutUrl(res.checkoutUrl);
        // 👉 Chuyển hướng trực tiếp đến PayOS
        window.location.href = res.checkoutUrl;
      }
    } catch (err: any) {
      console.error("Lỗi khi tạo thanh toán:", err);
      alert("Không thể tạo thanh toán, vui lòng thử lại!");
    } finally {
      setIsProcessing(false);
    }
  };

  // Reset trạng thái thanh toán
  const clearPayment = () => {
    setCurrentPlanId(null);
    setTransaction(null);
    setCheckoutUrl(null);
    setIsProcessing(false);
  };

  // Cung cấp context cho toàn app
  const value = useMemo<PaymentContextType>(
    () => ({ currentPlanId, transaction, isProcessing, checkoutUrl, createPayment, clearPayment }),
    [currentPlanId, transaction, isProcessing, checkoutUrl]
  );

  return <PaymentContext.Provider value={value}>{children}</PaymentContext.Provider>;
};

// Hook custom để sử dụng ở bất kỳ component nào
export const usePayment = (): PaymentContextType => {
  const ctx = useContext(PaymentContext);
  if (!ctx) throw new Error("usePayment must be used inside PaymentProvider");
  return ctx;
};
