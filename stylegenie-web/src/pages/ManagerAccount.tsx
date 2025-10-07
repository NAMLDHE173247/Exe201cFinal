import { useEffect, useState } from "react";
import { adminService, type AdminUserInfo, type UserListResponse } from "../api/adminservice";
import "../StyleCss/Pages/ManagerAccount.css";

export default function ManagerAccount() {
    // =========================
    // STATE QUẢN LÝ DỮ LIỆU
    // =========================
    const [users, setUsers] = useState<AdminUserInfo[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [successMsg, setSuccessMsg] = useState<string | null>(null);

    // Phân trang
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    // Thống kê tổng quan
    const [summary, setSummary] = useState<UserListResponse["statusSummary"] | null>(null);

    // Bộ lọc
    const [search, setSearch] = useState("");
    const [role, setRole] = useState("");
    const [isActive, setIsActive] = useState<string>("");
    const [isVerified, setIsVerified] = useState<string>("");

    // =========================
    // GỌI API LẤY DANH SÁCH
    // =========================
    const loadUsers = async () => {
        try {
            setLoading(true);
            const res = await adminService.getAllUsers({
                search: search || undefined,
                role: role || undefined,
                isActive: isActive === "" ? undefined : isActive === "true",
                isVerified: isVerified === "" ? undefined : isVerified === "true",
                page,
                pageSize: 10,
            });
            setUsers(res.data);
            setSummary(res.statusSummary);
            setTotalPages(res.totalPages);
        } catch (err) {
            console.error("❌ Lỗi khi tải dữ liệu:", err);
            setError("Không thể tải danh sách người dùng!");
        } finally {
            setLoading(false);
        }
    };

    // ✅ Tự động gọi API mỗi khi filter hoặc page thay đổi
    useEffect(() => {
        loadUsers();
    }, [page, search, role, isActive, isVerified]);

    // =========================
    // HÀM XỬ LÝ GIAO DIỆN
    // =========================
    const handleFilter = (e: React.FormEvent) => {
        e.preventDefault();
        setPage(1);
        // Không cần loadUsers() ở đây vì useEffect sẽ tự gọi
    };

    // ✅ Reset tất cả filter, tự động reload nhờ useEffect
    const handleClear = () => {
        setSearch("");
        setRole("");
        setIsActive("");
        setIsVerified("");
        setPage(1);
    };

    // =========================
    // HÀM CẬP NHẬT NGƯỜI DÙNG
    // =========================
    const showSuccess = (msg: string) => {
        setSuccessMsg(msg);
        setTimeout(() => setSuccessMsg(null), 2500);
    };

    const handleUpdateRole = async (id: number, newRole: string) => {
        await adminService.updateUserRole(id, newRole);
        showSuccess("✅ Cập nhật vai trò thành công!");
        loadUsers();
    };

    const handleUpdateActive = async (id: number, newStatus: boolean) => {
        await adminService.updateUserActive(id, newStatus);
        showSuccess("✅ Cập nhật trạng thái tài khoản thành công!");
        loadUsers();
    };

    const handleUpdateVerified = async (id: number, newStatus: boolean) => {
        await adminService.updateUserVerified(id, newStatus);
        showSuccess("✅ Cập nhật trạng thái xác minh thành công!");
        loadUsers();
    };

    // =========================
    // GIAO DIỆN
    // =========================
    return (
        <div className="manager-container">
            <h1 className="manager-title">📋 <span className="text-gradient">Quản lý người dùng</span></h1>

            {/* ✅ Thông báo cập nhật */}
            {successMsg && (
                <div className="alert alert-success" role="status">
                    {successMsg}
                </div>
            )}
            {error && (
                <div className="alert alert-error" role="alert">
                    {error}
                </div>
            )}

            {/* ✅ Thống kê tổng quan */}
            {summary && (
                <div className="summary-grid">
                    <div className="summary-card summary-active">
                        <p className="summary-title">Hoạt động</p>
                        <p className="summary-value">{summary.activeCount}</p>
                    </div>
                    <div className="summary-card summary-inactive">
                        <p className="summary-title">Bị khóa</p>
                        <p className="summary-value">{summary.inactiveCount}</p>
                    </div>
                    <div className="summary-card summary-verified">
                        <p className="summary-title">Đã xác minh</p>
                        <p className="summary-value">{summary.verifiedCount}</p>
                    </div>
                    <div className="summary-card summary-unverified">
                        <p className="summary-title">Chưa xác minh</p>
                        <p className="summary-value">{summary.unverifiedCount}</p>
                    </div>
                </div>
            )}

            {/* ✅ Bộ lọc */}
            <form onSubmit={handleFilter} className="filter-card">
                <div className="filter-row">
                    <input
                        type="text"
                        placeholder="🔍 Tìm theo email hoặc tên..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        className="input"
                    />

                    <select
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                        className="select"
                    >
                        <option value="">Tất cả vai trò</option>
                        <option value="Admin">Admin</option>
                        <option value="Staff">Staff</option>
                        <option value="User">User</option>
                    </select>

                    <select
                        value={isActive}
                        onChange={(e) => setIsActive(e.target.value)}
                        className="select"
                    >
                        <option value="">Trạng thái</option>
                        <option value="true">Đang hoạt động</option>
                        <option value="false">Bị khóa</option>
                    </select>

                    <select
                        value={isVerified}
                        onChange={(e) => setIsVerified(e.target.value)}
                        className="select"
                    >
                        <option value="">Xác minh</option>
                        <option value="true">Đã xác minh</option>
                        <option value="false">Chưa xác minh</option>
                    </select>

                    <button type="submit" className="btn btn-gradient">Lọc</button>
                    <button type="button" onClick={handleClear} className="btn btn-outline-gradient">🧹 Xóa bộ lọc</button>
                </div>
            </form>

            {/* ✅ Bảng người dùng */}
            {!loading && !error && (
                <div className="table-card">
                    <table className="table">
                        <thead className="thead">
                        <tr className="tr">
                            <th className="th">Email</th>
                            <th className="th">Họ và tên</th>
                            <th className="th">Tenant</th>
                            <th className="th">Role</th>
                            <th className="th">Active</th>
                            <th className="th">Verified</th>
                            <th className="th">Balance</th>
                            <th className="th">Ngày tạo</th>
                        </tr>
                        </thead>
                        <tbody>
                        {users.length === 0 ? (
                            <tr className="tr">
                                <td colSpan={8} className="td" style={{ textAlign: "center", color: "#6b7280", padding: "24px" }}>
                                    Không có người dùng nào.
                                </td>
                            </tr>
                        ) : (
                            users.map((u) => (
                                <tr key={u.id} className="tr">
                                    <td className="td">{u.email}</td>
                                    <td className="td">{u.fullName || "—"}</td>
                                    <td className="td">{u.tenantName}</td>

                                    {/* Dropdown Role */}
                                    <td className="td" style={{ textAlign: "center" }}>
                                        <select
                                            value={u.roleName}
                                            onChange={(e) => handleUpdateRole(u.id, e.target.value)}
                                            className="select-sm"
                                        >
                                            <option value="Admin">Admin</option>
                                            <option value="Staff">Staff</option>
                                            <option value="User">User</option>
                                        </select>
                                    </td>

                                    {/* Toggle Active */}
                                    <td className="td" style={{ textAlign: "center" }}>
                                        <select
                                            value={String(u.isActive)}
                                            onChange={(e) =>
                                                handleUpdateActive(u.id, e.target.value === "true")
                                            }
                                            className={`select-sm ${u.isActive ? "select-active" : "select-inactive"}`}
                                        >
                                            <option value="true">Đang hoạt động</option>
                                            <option value="false">Bị khóa</option>
                                        </select>
                                    </td>

                                    {/* Toggle Verified */}
                                    <td className="td" style={{ textAlign: "center" }}>
                                        <select
                                            value={String(u.isVerified)}
                                            onChange={(e) =>
                                                handleUpdateVerified(u.id, e.target.value === "true")
                                            }
                                            className={`select-sm ${u.isVerified ? "select-verified" : "select-unverified"}`}
                                        >
                                            <option value="true">Đã xác minh</option>
                                            <option value="false">Chưa xác minh</option>
                                        </select>
                                    </td>

                                    <td className="td" style={{ textAlign: "right" }}>{u.balance}</td>
                                    <td className="td">{new Date(u.createdAt).toLocaleString("vi-VN")}</td>
                                </tr>
                            ))
                        )}
                        </tbody>
                    </table>
                </div>
            )}

            {/* ✅ Loading */}
            {loading && (
                <p style={{ textAlign: "center", color: "#6b7280", padding: "40px 0" }}>
                    ⏳ Đang tải dữ liệu...
                </p>
            )}

            {/* ✅ Phân trang */}
            {!loading && totalPages > 1 && (
                <div className="pagination">
                    <button
                        onClick={() => setPage((p) => Math.max(1, p - 1))}
                        disabled={page === 1}
                        className="pagination-btn"
                    >
                        ⬅️ Trước
                    </button>
                    <span className="pagination-text">
                        Trang {page} / {totalPages}
                    </span>
                    <button
                        onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                        disabled={page === totalPages}
                        className="pagination-btn"
                    >
                        Tiếp ➡️
                    </button>
                </div>
            )}
        </div>
    );
}
