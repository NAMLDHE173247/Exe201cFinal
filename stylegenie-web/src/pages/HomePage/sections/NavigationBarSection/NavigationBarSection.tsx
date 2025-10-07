import { Link, useNavigate } from "react-router-dom"; // Thêm useNavigate
import styles from "./NavigationBarSection.module.css";
import Button from "../../components/Button";
import { useAuth } from "../../../../contexts/AuthContext";
import Cookies from "js-cookie";

export const NavigationBarSection = () => {
    const { user, logout } = useAuth(); // Lấy user và logout từ AuthContext
    const navigate = useNavigate(); // Sử dụng useNavigate để điều hướng

       const navigationItems = [
        { label: "Trang chủ", to: "/" },
        { label: "Thử đồ ảo", to: "/tryon" },
        { label: "Phân tích cá nhân hóa", to: "/skin" },
        { label: "Tủ đồ", to: "/user-home" },
        { label: "Góp VIP", to: "/affiliate" },
    ];



    // Mục menu riêng cho Admin
    const adminItems = [

        { label: "Quản lý tài khoản", to: "/manager-account" },
        { label: "Quản lý sản phẩm", to: "/manager-products" },
        { label: "Thêm sản phẩm ", to: "/manager-products/add" },
        { label: "Cập nhật api key ", to: "/admin/apikey" },

    ];

    // Mục menu riêng cho Staff
    const staffItems = [
        { label: "Trang chủ", to: "/" },
        { label: "Quản lý sản phẩm", to: "/manager-products" },
    ];

    // Hàm xử lý đăng xuất và chuyển hướng về trang chủ
    const handleLogout = () => {
        logout(); // Gọi logout từ context để xóa token và thông tin người dùng
        Cookies.remove("AuthToken"); // Xóa cookie
        navigate("/"); // Chuyển hướng về trang chủ sau khi đăng xuất
    };

    return (
        <header className={styles.wrapper}>
            <nav className={styles.nav}>
                {/* Logo + Brand */}
                <div className={styles.brand}>
                    <img
                        className={styles.brandLogo}
                        alt="StyleGenie Logo"
                        src="https://c.animaapp.com/mg6w6o8vAvJ0o7/img/group-1-1.png"
                    />
                    <div className={styles.brandName}>StyleGenie</div>
                </div>

                {/* Menu Links */}
                <div className={styles.links}>
                    {navigationItems.map((item, index) => (
                        <Link key={index} to={item.to} className={styles.link}>
                            {item.label}
                        </Link>
                    ))}

                    {/* Hiển thị các mục dành riêng cho Admin */}
                    {user?.roleId === 1 &&
                        adminItems.map((item, index) => (
                            <Link key={index} to={item.to} className={styles.link}>
                                {item.label}
                            </Link>
                        ))}

                    {/* Hiển thị các mục dành riêng cho Staff */}
                    {user?.roleId === 2 &&
                        staffItems.map((item, index) => (
                            <Link key={index} to={item.to} className={styles.link}>
                                {item.label}
                            </Link>
                        ))}
                </div>

                {/* Actions */}
                <div className={styles.actions}>
                    {!user ? (
                        <>
                            <Link to="/login">
                                <Button variant="outline-gradient">Đăng nhập</Button>
                            </Link>
                            <Link to="/register">
                                <Button>Dùng thử miễn phí</Button>
                            </Link>
                        </>
                    ) : (
                        <Button onClick={handleLogout}>Đăng xuất</Button> // Nút đăng xuất và điều hướng về trang chủ
                    )}
                </div>
            </nav>
        </header>
    );
};

export default NavigationBarSection;
