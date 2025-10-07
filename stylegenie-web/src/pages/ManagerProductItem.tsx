import { useEffect, useState } from "react";
import {
    Input,
    Select,
    Switch,
    Tag,
    Empty,
    Spin,
    Button,
    Table,
    Typography,
    Space,
    Popconfirm,
    message,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useNavigate } from "react-router-dom";
import { productItemService, type ProductItem } from "../api/productitemservice";
import "../StyleCss/Pages/ManagerProductItem.css";

export default function ManagerProductItem() {
    const navigate = useNavigate();

    // ----- UI state -----
    const [search, setSearch] = useState("");
    const [material, setMaterial] = useState<string | undefined>();
    const [category, setCategory] = useState<string | undefined>();
    const [isVipOnly, setIsVipOnly] = useState<boolean | undefined>();
    const [isActive, setIsActive] = useState<boolean | undefined>(true);

    const [materials, setMaterials] = useState<string[]>([]);
    const [categories, setCategories] = useState<string[]>([]);

    const [page, setPage] = useState(1);
    const pageSize = 12;

    const [rows, setRows] = useState<ProductItem[]>([]);
    const [total, setTotal] = useState(0);
    const [loading, setLoading] = useState(false);

    // ===== 1) Load dropdown (mount) =====
    useEffect(() => {
        productItemService.getFilters().then((res) => {
            setMaterials(res.materials ?? []);
            setCategories(res.categories ?? []);
        });
    }, []);

    // ===== 2) Load list khi filter/search/page thay đổi =====
    const fetchList = () => {
        setLoading(true);
        productItemService
            .getAllItems({
                search,
                material,
                category,
                isVipOnly,
                isActive,
                page,
                pageSize,
            })
            .then((res) => {
                setRows(res.data);
                setTotal(res.totalCount);
            })
            .finally(() => setLoading(false));
    };

    useEffect(() => {
        fetchList();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [search, material, category, isVipOnly, isActive, page]);

    // ===== 3) Handlers =====
    const onChangeSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPage(1);
        setSearch(e.target.value);
    };
    const onChangeMaterial = (v?: string) => {
        setPage(1);
        setMaterial(v);
    };
    const onChangeCategory = (v?: string) => {
        setPage(1);
        setCategory(v);
    };
    const onToggleVip = (checked: boolean) => {
        setPage(1);
        setIsVipOnly(checked);
    };
    const onToggleActive = (checked: boolean) => {
        setPage(1);
        setIsActive(checked);
    };
    const clearFilters = () => {
        setSearch("");
        setMaterial(undefined);
        setCategory(undefined);
        setIsVipOnly(undefined);
        setIsActive(true);
        setPage(1);
    };

    // ===== 4) Actions =====
    const handleSoftDelete = async (id: number) => {
        try {
            await productItemService.softDeleteItem(id);
            message.success("Đã vô hiệu hoá sản phẩm");
            fetchList();
        } catch (e: any) {
            message.error(e?.message || "Vô hiệu hoá thất bại");
        }
    };

    const handleHardDelete = async (id: number) => {
        try {
            await productItemService.hardDeleteItem(id);
            message.success("Đã xoá sản phẩm");
            // nếu đang ở trang cuối trống, có thể chỉnh page về 1 tuỳ ý
            fetchList();
        } catch (e: any) {
            message.error(e?.message || "Xoá sản phẩm thất bại");
        }
    };

    // ===== 5) Columns cho Table =====
    const columns: ColumnsType<ProductItem> = [
        {
            title: "Ảnh",
            dataIndex: "imageBase64",
            key: "image",
            width: 90,
            render: (src) =>
                src ? (
                    <img
                        src={src}
                        alt="thumb"
                        style={{ width: 64, height: 64, objectFit: "cover", borderRadius: 8 }}
                    />
                ) : (
                    <div style={{ width: 64, height: 64, background: "#f5f5f5", borderRadius: 8 }} />
                ),
        },
        {
            title: "Tên",
            dataIndex: "name",
            key: "name",
            width: 220,
            render: (v) => <Typography.Text strong>{v}</Typography.Text>,
        },
        {
            title: "Giá",
            dataIndex: "price",
            key: "price",
            width: 140,
            render: (v: number | null) =>
                v !== null ? `${v.toLocaleString()} đ` : <Typography.Text type="secondary">—</Typography.Text>,
        },
        {
            title: "Phong cách",
            dataIndex: "material",
            key: "material",
            width: 140,
            render: (v?: string) => (v ? <Tag>{v}</Tag> : <span>—</span>),
        },
        {
            title: "Category",
            dataIndex: "category",
            key: "category",
            width: 140,
            render: (v?: string) => (v ? <Tag>{v}</Tag> : <span>—</span>),
        },
        {
            title: "Trạng thái",
            key: "status",
            width: 160,
            render: (_, r) => (
                <Space size={6}>
                    {r.isActive ? <Tag color="green">Active</Tag> : <Tag>Inactive</Tag>}
                    {r.isVipOnly && <Tag color="magenta">VIP</Tag>}
                </Space>
            ),
        },
        {
            title: "Ngày tạo",
            dataIndex: "createdAt",
            key: "createdAt",
            width: 180,
            render: (v: string) => new Date(v).toLocaleString(),
        },
        {
            title: "Affiliate",
            dataIndex: "affiliateUrl",
            key: "affiliateUrl",
            width: 180,
            render: (url?: string | null) =>
                url ? (
                    <a href={url} target="_blank" rel="noreferrer">
                        Mở liên kết
                    </a>
                ) : (
                    <Typography.Text type="secondary">—</Typography.Text>
                ),
        },
        {
            title: "Sửa",
            key: "edit",
            fixed: "right",
            width: 90,
            render: (_, r) => (
                <Button size="small" onClick={() => navigate(`/manager-products/${r.id}/edit`)}>
                    Sửa
                </Button>
            ),
        },
        {
            title: "Vô hiệu hoá",
            key: "softDelete",
            fixed: "right",
            width: 120,
            render: (_, r) => (
                <Popconfirm
                    title="Vô hiệu hoá sản phẩm?"
                    okText="Vô hiệu hoá"
                    cancelText="Huỷ"
                    onConfirm={() => handleSoftDelete(r.id)}
                    disabled={!r.isActive}
                >
                    <Button size="small" disabled={!r.isActive}>
                        Vô hiệu hoá
                    </Button>
                </Popconfirm>
            ),
        },
        {
            title: "Xoá",
            key: "hardDelete",
            fixed: "right",
            width: 90,
            render: (_, r) => (
                <Popconfirm
                    title="Xoá vĩnh viễn sản phẩm?"
                    description="Hành động này không thể hoàn tác."
                    okText="Xoá"
                    okButtonProps={{ danger: true }}
                    cancelText="Huỷ"
                    onConfirm={() => handleHardDelete(r.id)}
                >
                    <Button size="small" danger>
                        Xoá
                    </Button>
                </Popconfirm>
            ),
        },
    ];

    return (
        <div className="page-wrapper">
            <div className="mpi-container">
                <div className="page-header">
                    <h2 className="page-title">Quản lý sản phẩm</h2>
                </div>
                {/* Thanh filter trên đầu */}
                <div className="mpi-filters card-surface">
                <Input
                    placeholder="Tìm theo tên / phong cách / category..."
                    value={search}
                    onChange={onChangeSearch}
                    allowClear
                />
                <Select
                    allowClear
                    placeholder="Phong cách (Material)"
                    value={material}
                    onChange={onChangeMaterial}
                    options={materials.map((m) => ({ value: m, label: m }))}
                />
                <Select
                    allowClear
                    placeholder="Category"
                    value={category}
                    onChange={onChangeCategory}
                    options={categories.map((c) => ({ value: c, label: c }))}
                />
                <div className="mpi-toggle">
                    VIP Only <Switch checked={!!isVipOnly} onChange={onToggleVip} />
                </div>
                <div className="mpi-toggle">
                    Active <Switch checked={!!isActive} onChange={onToggleActive} />
                </div>
                    <Button className="btn-outline-gradient" onClick={clearFilters}>Xoá lọc</Button>
                    <Button className="btn-gradient" onClick={fetchList}>Refresh</Button>
                </div>

                {/* Bảng dữ liệu */}
                {loading ? (
                    <div className="mpi-center">
                        <Spin />
                    </div>
                ) : rows.length === 0 ? (
                    <Empty description="Không có sản phẩm phù hợp" />
                ) : (
                    <Table<ProductItem>
                        rowKey="id"
                        columns={columns}
                        dataSource={rows}
                        pagination={{
                            current: page,
                            pageSize,
                            total,
                            showSizeChanger: false,
                            onChange: (p) => setPage(p),
                        }}
                        scroll={{ x: 1250 }}
                        size="middle"
                    />
                )}
            </div>
        </div>
    );
}
