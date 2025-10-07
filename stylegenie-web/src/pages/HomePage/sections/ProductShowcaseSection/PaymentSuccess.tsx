import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { CheckCircleFilled, LoadingOutlined } from "@ant-design/icons";
import { paymentApi } from "../../../../api/payment";

const PaymentSuccess: React.FC = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const [loading, setLoading] = useState(true);
    const [verified, setVerified] = useState(false);
    const [message, setMessage] = useState("");

    useEffect(() => {
        const verifyPayment = async () => {
            const orderCode = Number(searchParams.get("orderCode"));
            const status = searchParams.get("status") || "PAID";

            if (!orderCode) {
                setMessage("Không tìm thấy mã đơn hàng!");
                setLoading(false);
                return;
            }

            try {
                const res = await paymentApi.checkReturnStatus(orderCode, status);
                if (res.status === "PAID") {
                    setVerified(true);
                    setMessage("Thanh toán thành công 🎉");
                } else {
                    setVerified(false);
                    setMessage("Giao dịch không thành công.");
                }
            } catch (err) {
                console.error(err);
                setVerified(false);
                setMessage("Không thể xác nhận giao dịch. Vui lòng thử lại.");
            } finally {
                setLoading(false);
            }
        };

        verifyPayment();
    }, [searchParams]);

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-green-100 via-white to-green-200 p-6 text-center">
            {loading ? (
                <>
                    <LoadingOutlined style={{ fontSize: 60, color: "#52c41a" }} spin />
                    <h2 className="text-xl mt-4 text-gray-600">Đang xác nhận giao dịch...</h2>
                </>
            ) : verified ? (
                <>
                    <CheckCircleFilled style={{ fontSize: 80, color: "#52c41a" }} />
                    <h1 className="text-3xl font-bold mt-4 text-green-700">{message}</h1>
                    <p className="text-gray-600 mt-2">
                        Cảm ơn bạn đã nâng cấp gói dịch vụ. Tài khoản của bạn đã được cập nhật.
                    </p>
                    <div className="mt-6">
                        <button
                            onClick={() => navigate("/")}
                            className="btn btn-gradient text-white px-6 py-3 rounded-full shadow-lg hover:scale-105 transition-transform duration-200"
                        >
                            ⬅️ Trở về Trang chủ
                        </button>
                    </div>
                </>
            ) : (
                <>
                    <h1 className="text-3xl font-bold mt-4 text-red-700">{message}</h1>
                    <button
                        onClick={() => navigate("/")}
                        className="btn btn-outline-gradient px-6 py-3 rounded-full mt-6"
                    >
                        ⬅️ Quay lại Trang chủ
                    </button>
                </>
            )}
        </div>
    );
};

export default PaymentSuccess;
