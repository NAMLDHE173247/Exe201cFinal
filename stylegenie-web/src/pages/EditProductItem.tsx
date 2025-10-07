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
    Typography,
    Checkbox,
} from "antd";
import type { UploadProps, SelectProps } from "antd";
import { useNavigate, useParams } from "react-router-dom";
import { productItemService } from "../api/productitemservice";
import "../StyleCss/Pages/EditProductItem.css";

export default function EditProductItem() {
    const { id } = useParams<{ id: string }>();
    const productId = Number(id);
    const [form] = Form.useForm();
    const navigate = useNavigate();

    const [materials, setMaterials] = useState<string[]>([]);
    const [categories, setCategories] = useState<string[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    const [file, setFile] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [removeImage, setRemoveImage] = useState(false);

    // =============================
    // 1️⃣ Load filters + item detail
    // =============================
    useEffect(() => {
        if (!productId) return;

        Promise.all([
            productItemService.getFilters(),
            productItemService.getItem(productId),
        ])
            .then(([flt, item]) => {
                setMaterials(flt.materials ?? []);
                setCategories(flt.categories ?? []);

                form.setFieldsValue({
                    name: item.name,
                    price: item.price ?? undefined,
                    material: item.material ?? undefined,
                    category: item.category ?? undefined,
                    affiliateUrl: item.affiliateUrl ?? undefined,
                    isVipOnly: item.isVipOnly,
                    isActive: item.isActive,
                });

                setPreview(item.imageBase64 ?? null);
            })
            .catch((e) =>
                message.error(e?.message || "Không tải được dữ liệu sản phẩm")
            )
            .finally(() => setLoading(false));
    }, [productId, form]);

    // =============================
    // 2️⃣ Upload config
    // =============================
    const uploadProps: UploadProps = {
        multiple: false,
        accept: "image/*",
        beforeUpload: (f) => {
            setFile(f);
            const url = URL.createObjectURL(f);
            setPreview(url);
            setRemoveImage(false);
            return false; // chặn upload tự động
        },
        onRemove: () => {
            setFile(null);
            setPreview(null);
            return true;
        },
        maxCount: 1,
    };

    // =============================
    // 3️⃣ Select search + nhập mới
    // =============================
    const filterOption: SelectProps["filterOption"] = (input, option) => {
        const label = (option?.label ?? "").toString().toLowerCase();
        return label.includes(input.toLowerCase());
    };

    // Cho phép thêm mới Material/Category
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

    // =============================
    // 4️⃣ Submit update
    // =============================
    const onSubmit = async () => {
        try {
            const values = await form.validateFields();
            setSaving(true);

            await productItemService.updateItem(productId, {
                name: values.name,
                price:
                    values.price !== undefined &&
                    values.price !== null &&
                    values.price !== ""
                        ? Number(values.price)
                        : null,
                material: values.material,
                category: values.category,
                affiliateUrl: values.affiliateUrl,
                isVipOnly: !!values.isVipOnly,
                isActive: !!values.isActive,
                imageFile: file ?? undefined,
                removeImage,
            });

            message.success("Đã cập nhật sản phẩm!");
            navigate("/manager-products");
        } catch (err: unknown) {
            if (
                typeof err === "object" &&
                err !== null &&
                "errorFields" in err
            )
                return; // lỗi validate
            const msg =
                err instanceof Error ? err.message : "Cập nhật thất bại";
            message.error(msg);
        } finally {
            setSaving(false);
        }
    };

    // =============================
    // 5️⃣ Loading
    // =============================
    if (loading) {
        return (
            <div className="page-wrapper">
                <div
                    className="card-surface"
                    style={{ maxWidth: 900, margin: "16px auto", padding: 16 }}
                >
                    <Typography.Text>Đang tải dữ liệu...</Typography.Text>
                </div>
            </div>
        );
    }

    // =============================
    // 6️⃣ UI chính
    // =============================
    return (
        <div className="page-wrapper">
            <Card
                className="card-surface"
                title={`Sửa sản phẩm #${productId}`}
                style={{ maxWidth: 900, margin: "16px auto" }}
            >
                <Form
                    form={form}
                    layout="vertical"
                    onFinish={onSubmit}
                    className="form-grid"
                >
                    {/* --- Tên sản phẩm --- */}
                    <Form.Item
                        label="Tên sản phẩm"
                        name="name"
                        rules={[{ required: true, message: "Nhập tên sản phẩm" }]}
                    >
                        <Input placeholder="Tên sản phẩm" />
                    </Form.Item>

                    {/* --- Giá + Active --- */}
                    <Space size={16} className="form-row-2">
                        <Form.Item label="Giá (đ)" name="price">
                            <InputNumber
                                min={0}
                                step={1000}
                                style={{ width: "100%" }}
                                placeholder="Ví dụ: 250000"
                                formatter={(v) => {
                                    if (typeof v === "number")
                                        return v.toLocaleString();
                                    const s = (v ?? "").toString();
                                    if (!s) return "";
                                    const digits = s.replace(/[^\d.-]/g, "");
                                    return digits.replace(
                                        /\B(?=(\d{3})+(?!\d))/g,
                                        ","
                                    );
                                }}
                                parser={((displayValue: string | undefined) => {
                                    const s = (displayValue ?? "")
                                        .toString()
                                        .replace(/,/g, "")
                                        .replace(/[^\d.-]/g, "");
                                    const n = Number(s);
                                    return Number.isNaN(n) ? 0 : n;
                                }) as unknown as (
                                    displayValue: string | undefined
                                ) => 0}
                            />
                        </Form.Item>

                        <Form.Item
                            label="Active"
                            name="isActive"
                            valuePropName="checked"
                        >
                            <Switch />
                        </Form.Item>
                    </Space>

                    {/* --- Material & Category --- */}
                    <Space size={16} className="form-row-2">
                        <Form.Item label="Phong cách (Material)" name="material">
                            <Select
                                mode="combobox"               // ✅ Cho phép nhập mới
                                allowClear
                                showSearch
                                placeholder="Chọn hoặc nhập phong cách"
                                options={materials.map((m) => ({
                                    value: m,
                                    label: m,
                                }))}
                                filterOption={filterOption}
                                onSearch={handleMaterialSearch}
                            />
                        </Form.Item>

                        <Form.Item label="Category" name="category">
                            <Select
                                mode="combobox"               // ✅ Cho phép nhập mới
                                allowClear
                                showSearch
                                placeholder="Chọn hoặc nhập category"
                                options={categories.map((c) => ({
                                    value: c,
                                    label: c,
                                }))}
                                filterOption={filterOption}
                                onSearch={handleCategorySearch}
                            />
                        </Form.Item>
                    </Space>

                    {/* --- Affiliate URL --- */}
                    <Form.Item label="Affiliate URL" name="affiliateUrl">
                        <Input placeholder="https://..." />
                    </Form.Item>

                    {/* --- VIP Only --- */}
                    <Form.Item
                        label="VIP Only"
                        name="isVipOnly"
                        valuePropName="checked"
                    >
                        <Switch />
                    </Form.Item>

                    {/* --- Ảnh sản phẩm --- */}
                    <Form.Item label="Ảnh sản phẩm">
                        {preview && (
                            <div style={{ marginBottom: 8 }}>
                                <img
                                    src={preview}
                                    alt="preview"
                                    style={{
                                        width: 180,
                                        height: 180,
                                        objectFit: "cover",
                                        borderRadius: 8,
                                    }}
                                />
                            </div>
                        )}
                        <Upload.Dragger {...uploadProps}>
                            <p className="ant-upload-drag-icon">📷</p>
                            <p className="ant-upload-text">
                                Kéo thả hoặc bấm để chọn ảnh mới (tuỳ chọn)
                            </p>
                        </Upload.Dragger>
                        <div style={{ marginTop: 8 }}>
                            <Checkbox
                                checked={removeImage}
                                onChange={(e) =>
                                    setRemoveImage(e.target.checked)
                                }
                            >
                                Xoá ảnh hiện tại
                            </Checkbox>
                        </div>
                    </Form.Item>

                    {/* --- Nút hành động --- */}
                    <Form.Item>
                        <Space>
                            <Button
                                className="btn-gradient"
                                type="primary"
                                htmlType="submit"
                                loading={saving}
                            >
                                Lưu thay đổi
                            </Button>
                            <Button
                                className="btn-outline-gradient"
                                onClick={() => navigate("/manager-products")}
                            >
                                Huỷ
                            </Button>
                        </Space>
                    </Form.Item>
                </Form>
            </Card>
        </div>
    );
}
