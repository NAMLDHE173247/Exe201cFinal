import { useEffect, useState } from "react";
import {
    Form,
    Input,
    InputNumber,
    Select,
    Switch,
    Upload,
    Button,
    Space,
    message,
    Card,
} from "antd";
import type { UploadProps, SelectProps } from "antd";
import { useNavigate } from "react-router-dom";
import Cookies from "js-cookie"; // ✅ Thêm dòng này
import { productItemService, type FilterResponse } from "../api/productitemservice";
import "../StyleCss/Pages/AddProductItem.css";

export default function AddProductItem() {
    const [form] = Form.useForm();
    const navigate = useNavigate();

    const [materials, setMaterials] = useState<string[]>([]);
    const [categories, setCategories] = useState<string[]>([]);
    const [file, setFile] = useState<File | null>(null);
    const [submitting, setSubmitting] = useState(false);

    // ✅ 0️⃣ Kiểm tra token khi load trang
    useEffect(() => {
        const token = Cookies.get("AuthToken");
        if (!token) {
            message.warning("⚠️ Bạn chưa đăng nhập. Đang chuyển hướng...");
            setTimeout(() => navigate("/login"), 2000);
            return;
        }
    }, [navigate]);

    // 1️⃣ Load dropdown
    useEffect(() => {
        productItemService.getFilters().then((res: FilterResponse) => {
            setMaterials(res.materials ?? []);
            setCategories(res.categories ?? []);
        });
    }, []);

    // 2️⃣ Upload config
    const uploadProps: UploadProps = {
        multiple: false,
        accept: "image/*",
        beforeUpload: (f) => {
            setFile(f);
            return false;
        },
        onRemove: () => {
            setFile(null);
        },
        maxCount: 1,
    };

    // 3️⃣ Lọc và cho phép nhập mới
    const filterOption: SelectProps["filterOption"] = (input, option) => {
        const label = (option?.label ?? "").toString().toLowerCase();
        return label.includes(input.toLowerCase());
    };

    const handleMaterialSearch = (input: string) => {
        if (input && !materials.find((m) => m.toLowerCase() === input.toLowerCase())) {
            setMaterials((prev) => [...prev, input]);
        }
    };

    const handleCategorySearch = (input: string) => {
        if (input && !categories.find((c) => c.toLowerCase() === input.toLowerCase())) {
            setCategories((prev) => [...prev, input]);
        }
    };

    // 4️⃣ Submit
    const onSubmit = async () => {
        try {
            const values = await form.validateFields();
            setSubmitting(true);

            await productItemService.createItem({
                name: values.name,
                price: values.price ?? null,
                material: values.material,
                category: values.category,
                affiliateUrl: values.affiliateUrl,
                isVipOnly: !!values.isVipOnly,
                imageFile: file ?? undefined,
            });

            message.success("✅ Đã thêm sản phẩm!");
            navigate("/manager-products");
        } catch (err: unknown) {
            if (typeof err === "object" && err !== null && "errorFields" in err) return;
            const msg = err instanceof Error ? err.message : "❌ Thêm sản phẩm thất bại";
            message.error(msg);
        } finally {
            setSubmitting(false);
        }
    };

    const onReset = () => {
        form.resetFields();
        setFile(null);
    };

    return (
        <div className="page-wrapper">
            <Card
                className="card-surface"
                title="Thêm sản phẩm"
                style={{ maxWidth: 900, margin: "16px auto" }}
            >
                <Form
                    form={form}
                    layout="vertical"
                    initialValues={{ isVipOnly: false }}
                    onFinish={onSubmit}
                >
                    <Form.Item
                        label="Tên sản phẩm"
                        name="name"
                        rules={[{ required: true, message: "Nhập tên sản phẩm" }]}
                    >
                        <Input placeholder="Ví dụ: áo, quần, fullset..." />
                    </Form.Item>

                    <Space size={16} className="form-row-2">
                        <Form.Item label="Giá (đ)" name="price">
                            <InputNumber
                                min={0}
                                step={1000}
                                style={{ width: "100%" }}
                                placeholder="Ví dụ: 250000"
                            />
                        </Form.Item>

                        <Form.Item label="VIP Only" name="isVipOnly" valuePropName="checked">
                            <Switch />
                        </Form.Item>
                    </Space>

                    <Space size={16} className="form-row-2">
                        <Form.Item label="Phong cách (Material)" name="material">
                            <Select
                                allowClear
                                showSearch
                                placeholder="Chọn hoặc nhập phong cách"
                                options={materials.map((m) => ({ value: m, label: m }))}
                                filterOption={filterOption}
                                onSearch={handleMaterialSearch}
                            />
                        </Form.Item>

                        <Form.Item label="Category" name="category">
                            <Select
                                allowClear
                                showSearch
                                placeholder="Chọn hoặc nhập category"
                                options={categories.map((c) => ({ value: c, label: c }))}
                                filterOption={filterOption}
                                onSearch={handleCategorySearch}
                            />
                        </Form.Item>
                    </Space>

                    <Form.Item label="Affiliate URL" name="affiliateUrl">
                        <Input placeholder="https://..." />
                    </Form.Item>

                    <Form.Item label="Ảnh sản phẩm">
                        <Upload.Dragger {...uploadProps}>
                            <p className="ant-upload-drag-icon">📷</p>
                            <p className="ant-upload-text">Kéo thả hoặc bấm để chọn ảnh</p>
                        </Upload.Dragger>
                    </Form.Item>

                    <Form.Item>
                        <Space>
                            <Button
                                className="btn-gradient"
                                type="primary"
                                htmlType="submit"
                                loading={submitting}
                            >
                                Lưu sản phẩm
                            </Button>
                            <Button className="btn-outline-gradient" onClick={onReset}>
                                Làm mới
                            </Button>
                            <Button
                                className="btn-outline-gradient"
                                onClick={() => navigate("/manager-products")}
                            >
                                Về danh sách
                            </Button>
                        </Space>
                    </Form.Item>
                </Form>
            </Card>
        </div>
    );
}
