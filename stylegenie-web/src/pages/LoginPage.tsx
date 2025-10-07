import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { authApi } from "../api/auth";
import { useAuth } from "../contexts/AuthContext";
import GoogleLoginButton from "../components/GoogleLoginButton";
import "../StyleCss/Pages/LoginPage.css";

export default function LoginPage() {
    const { login } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        document.body.classList.add("login-no-wrapper");
        return () => document.body.classList.remove("login-no-wrapper");
    }, []);

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [showPass, setShowPass] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const redirectByRoleId = (roleId?: number | null) => {
        switch (roleId) {
            case 1: navigate("/manager-account"); break;
            case 2: navigate("/manager-products"); break;
            case 3:
            default: navigate("/tryon"); break;
        }
    };

    const handleEmailLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        if (!email || !password) {
            setError("Vui lòng nhập email và mật khẩu.");
            return;
        }
        setSubmitting(true);
        try {
            const { token, user } = await authApi.loginWithEmail(email.trim(), password);
            login(token, user);
            redirectByRoleId(user.roleId);
        } catch (err: unknown) {
            const msg = err instanceof Error ? err.message : "Đăng nhập thất bại. Vui lòng thử lại.";
            setError(msg);
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className="login-page">
            <div className="login-container">
                <div className="login-card">
                    <div className="login-header">
                        <h1 className="login-title">Đăng nhập</h1>
                        <p className="login-subtitle">Chào mừng bạn quay trở lại với StyleGenie</p>
                    </div>

                    <form onSubmit={handleEmailLogin} className="login-form">
                        <div className="form-group">
                            <label htmlFor="email" className="form-label">Email</label>
                            <input
                                id="email"
                                type="email"
                                placeholder="Nhập email của bạn"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                className="form-input"
                                required
                                autoComplete="email"
                                inputMode="email"
                            />
                        </div>

                        <div className="form-group">
                            <label htmlFor="password" className="form-label">Mật khẩu</label>
                            <div className="password-input-wrapper">
                                <input
                                    id="password"
                                    type={showPass ? "text" : "password"}
                                    placeholder="Nhập mật khẩu của bạn"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    className="form-input password-input"
                                    required
                                    autoComplete="current-password"
                                />
                                <button
                                    type="button"
                                    className="password-toggle"
                                    onClick={() => setShowPass((s) => !s)}
                                    onMouseDown={(e) => e.preventDefault()}
                                >
                                    {showPass ? "Ẩn" : "Hiện"}
                                </button>
                            </div>
                        </div>

                        {error && (
                            <div className="error-message">
                                {error}
                            </div>
                        )}

                        <button 
                            type="submit" 
                            className="login-button"
                            disabled={submitting}
                        >
                            {submitting ? "Đang đăng nhập..." : "Đăng nhập"}
                        </button>
                    </form>

                    <div className="divider">
                        <div className="divider-line"></div>
                        <span className="divider-text">hoặc</span>
                        <div className="divider-line"></div>
                    </div>

                    <div className="google-login-wrapper">
                        <GoogleLoginButton
                            onLoginSuccess={({ token, user }) => {
                                login(token, user);
                                redirectByRoleId(user.roleId);
                            }}
                        />
                    </div>

                    <div className="login-footer">
                        <Link to="/forgot-password" className="forgot-password-link">
                            Quên mật khẩu?
                        </Link>
                        <div className="register-link">
                            <span>Chưa có tài khoản? </span>
                            <Link to="/register" className="register-link-text">
                                Đăng ký ngay
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}