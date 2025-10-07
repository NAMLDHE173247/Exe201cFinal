import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { CloseCircleFilled, LoadingOutlined } from "@ant-design/icons";
import { paymentApi } from "../../../../api/payment";

const PaymentCancel: React.FC = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const [loading, setLoading] = useState(true);
    const [message, setMessage] = useState("");

    useEffect(() => {
        const cancelPayment = async () => {
            const orderCode = Number(searchParams.get("orderCode"));
            const status = searchParams.get("status") || "FAILED";

            if (!orderCode) {
                setMessage("Không tìm thấy mã giao dịch!");
                setLoading(false);
                return;
            }

            try {
                const res = await paymentApi.checkReturnStatus(orderCode, status);
                if (res.status === "FAILED") {
                    setMessage("Thanh toán bị hủy hoặc thất bại ❌");
                } else {
                    setMessage("Trạng thái không xác định, vui lòng kiểm tra lại tài khoản.");
                }
            } catch (err) {
                console.error(err);
                setMessage("Không thể xác nhận trạng thái thanh toán.");
            } finally {
                setLoading(false);
            }
        };

        cancelPayment();
    }, [searchParams]);

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-red-100 via-white to-red-200 p-6 text-center">
            {loading ? (
                <>
                    <LoadingOutlined style={{ fontSize: 60, color: "#ff4d4f" }} spin />
                    <h2 className="text-xl mt-4 text-gray-600">Đang xác nhận trạng thái...</h2>
                </>
            ) : (
                <>
                    <CloseCircleFilled style={{ fontSize: 80, color: "#ff4d4f" }} />
                    <h1 className="text-3xl font-bold mt-4 text-red-700">{message}</h1>
                    <p className="text-gray-600 mt-2">
                        Nếu bạn gặp vấn đề khi thanh toán, vui lòng thử lại hoặc liên hệ hỗ trợ.
                    </p>
                    <div className="mt-6">
                        <button
                            onClick={() => navigate("/")}
                            className="btn btn-outline-gradient px-6 py-3 rounded-full shadow hover:scale-105 transition-transform duration-200"
                        >
                            ⬅️ Quay lại Trang chủ
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default PaymentCancel;
