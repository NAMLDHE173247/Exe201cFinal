import React from "react";
import { useAuth } from "../../../../contexts/AuthContext";
import { usePayment } from "../../../../contexts/PaymentContext";

const pricingPlans = [
    {
        id: 1,
        name: "Basic",
        description: "Dành cho người mới bắt đầu",
        price: "0đ",
        period: "/Tháng",
        buttonText: "Dùng miễn phí",
        variant: "outline" as const,
        features: [
            "Cơ bản (nhận diện sơ bộ)",
            "Tối đa 3 items/ngày",
            "Lưu tối đa 3 outfit",
            "Thử đồ ảo cơ bản",
            "Color analysis đơn giản",
            "Watemark khi chia sẻ",
            "Có quảng cáo",
        ],
    },
    {
        id: 2,
        name: "Advance",
        description: "Dành cho người phổ thông thích cá nhân hóa",
        price: "119.000đ",
        period: "/Tháng",
        buttonText: "Nâng cấp ngay",
        variant: "default" as const,
        features: [
            "Tối đa 15 items/ngày",
            "Truy cập toàn bộ kho đồ (1000+ items)",
            "Cá nhân hóa theo dáng người, màu da & phong cách",
            "Lưu trữ đến 25 outfit trong tủ đồ cá nhân",
            "Phân tích màu sắc chuyên nghiệp",
            "Tải ảnh HD, không watermark",
            "Trải nghiệm không quảng cáo",
            "AI Styling được cá nhân hóa",
            "Tham gia cộng đồng VIP độc quyền",
            "Hỗ trợ ưu tiên 24/7",
        ],
    },
    {
        id: 3,
        name: "Premium",
        description: "Dành cho fashionista, stylist, influencer, ...",
        price: "349.000đ",
        period: "/Tháng",
        buttonText: "Nâng cấp ngay",
        variant: "default" as const,
        features: [
            "Kho đồ mở rộng, cập nhật trend hàng tuần",
            "Ưu tiên outfit theo mùa & sự kiện",
            "Phân tích nâng cao, gợi ý thông minh theo hành vi & sở thích",
            "Tối đa 50 items/ngày",
            "Lưu outfit không giới hạn",
            "Phân loại outfit theo chủ đề & bộ sưu tập",
            "Hỗ trợ AI cá nhân hóa chuyên sâu",
            "Tham gia workshop độc quyền",
        ],
    },
];

export const ProductShowcaseSection = () => {
    const { user } = useAuth();
    const { createPayment, isProcessing } = usePayment();

    const handlePayment = async (planId: number) => {
        if (!user) {
            alert("⚠️ Vui lòng đăng nhập trước khi nâng cấp gói!");
            return;
        }

        await createPayment({
            userId: user.id,
            planId,
            cancelUrl: "http://localhost:5173/payment/cancel",
            returnUrl: "http://localhost:5173/payment/success",
        });
    };

    return (
        <section className="pricing">
            <header className="pricing-header">
                <h3 className="subtitle-gradient">Bảng giá</h3>
                <h2 className="pricing-title">Các gói dịch vụ nâng cấp</h2>
            </header>

            <div className="pricing-grid">
                {pricingPlans.map((plan) => (
                    <div key={plan.id} className="pricing-card">
                        <h3 className="plan-name subtitle-gradient-lg">{plan.name}</h3>
                        <p className="plan-desc">{plan.description}</p>

                        <div className="plan-price-row">
                            <span className="plan-price subtitle-gradient-lg">{plan.price}</span>
                            <span className="plan-period subtitle-gradient">{plan.period}</span>
                        </div>

                        {plan.subPrice && <p className="plan-subprice">{plan.subPrice}</p>}

                        <button
                            disabled={isProcessing}
                            onClick={() => handlePayment(plan.id)}
                            className={
                                plan.variant === "outline"
                                    ? "btn btn-outline-gradient w-full"
                                    : "btn btn-gradient w-full"
                            }
                        >
                            {isProcessing ? "Đang xử lý..." : plan.buttonText}
                        </button>

                        <div className="plan-features">
                            {plan.features.map((feature, index) => (
                                <div key={index} className="feature-line">
                                    <span className="check">✓</span>
                                    <span className="feature-text">{feature}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
};

export default ProductShowcaseSection;
