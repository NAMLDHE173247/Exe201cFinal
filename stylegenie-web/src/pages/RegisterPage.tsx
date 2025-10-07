import { useMemo, useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { authApi } from "../api/auth";
import type { RegisterUserRequest } from "../api/auth";
import GoogleLoginButton from "../components/GoogleLoginButton";
import "../StyleCss/Pages/RegisterPage.css";

/** RULES chỉ để HIỂN THỊ (không chặn submit) */
const PASSWORD_RULES = {
    minLen: 8,
    requireLower: true,
    requireUpper: true,
    requireDigit: true,
    requireSpecial: true,
    noSpaces: true,
};

function checkPassword(pw: string) {
    const hasMinLen   = pw.length >= PASSWORD_RULES.minLen;
    const hasLower    = !PASSWORD_RULES.requireLower || /[a-z]/.test(pw);
    const hasUpper    = !PASSWORD_RULES.requireUpper || /[A-Z]/.test(pw);
    const hasDigit    = !PASSWORD_RULES.requireDigit || /\d/.test(pw);
    const hasSpecial  = !PASSWORD_RULES.requireSpecial || /[^A-Za-z0-9]/.test(pw);
    const hasNoSpaces = !PASSWORD_RULES.noSpaces || !/\s/.test(pw);
    return { hasMinLen, hasLower, hasUpper, hasDigit, hasSpecial, hasNoSpaces };
}

function getErrorMessage(e: unknown) {
    if (e instanceof Error) return e.message;
    try { return JSON.stringify(e); } catch { return String(e); }
}

export default function RegisterPage() {
    const navigate = useNavigate();

    useEffect(() => {
        document.body.classList.add("register-no-wrapper");
        return () => document.body.classList.remove("register-no-wrapper");
    }, []);

    const [form, setForm] = useState<RegisterUserRequest>({ email: "", password: "", fullName: "" });
    const [showPass, setShowPass] = useState(false);
    const [showPwRules, setShowPwRules] = useState(false);
    const [loading, setLoading] = useState(false);

    // trạng thái sau đăng ký
    const [isRegistered, setIsRegistered] = useState(false);
    const [registeredEmail, setRegisteredEmail] = useState<string>("");

    // verify state
    const [verifyCode, setVerifyCode] = useState("");
    const [verifying, setVerifying] = useState(false);
    const [verifyOk, setVerifyOk] = useState<string | null>(null);
    const [verifyErr, setVerifyErr] = useState<string | null>(null);

    // lỗi form register
    const [error, setError] = useState<string | null>(null);

    // lỗi Google (email trùng, 409 từ backend)
    const [googleError, setGoogleError] = useState<string | null>(null);

    const emailLooksValid = useMemo(() => /\S+@\S+\.\S+/.test(form.email), [form.email]);
    const pwCheck = useMemo(() => checkPassword(form.password), [form.password]);

    const onChange = <K extends keyof RegisterUserRequest>(key: K, value: RegisterUserRequest[K]) =>
        setForm((s) => ({ ...s, [key]: value }));

    const canSubmit = emailLooksValid && !!form.password && !loading;

    const submit = async (ev: React.FormEvent<HTMLFormElement>) => {
        ev.preventDefault();
        setError(null);

        if (!emailLooksValid) { setError("Email không hợp lệ."); return; }
        if (!form.password)   { setError("Vui lòng nhập mật khẩu."); return; }

        try {
            setLoading(true);
            const data = await authApi.register({
                email: form.email.trim(),
                password: form.password,
                fullName: form.fullName?.trim() || undefined,
            });
            // chuyển sang bước xác thực
            setIsRegistered(true);
            setRegisteredEmail(data.email);
            setVerifyCode(""); setVerifyOk(null); setVerifyErr(null);
        } catch (e: unknown) {
            const msg = getErrorMessage(e) || "Đăng ký thất bại";
            setError(msg);
        } finally {
            setLoading(false);
        }
    };

    const handleVerify = async (ev: React.FormEvent) => {
        ev.preventDefault();
        setVerifyOk(null);
        setVerifyErr(null);

        if (!verifyCode.trim()) {
            setVerifyErr("Vui lòng nhập mã xác thực.");
            return;
        }

        try {
            setVerifying(true);
            const email = (registeredEmail || form.email).trim();
            await authApi.verifyEmail(email, verifyCode.trim());
            setVerifyOk("✅ Xác thực email thành công! Đang chuyển sang trang đăng nhập...");
            setVerifyErr(null);

            // 👉 chuyển sang trang login sau 2 giây
            setTimeout(() => navigate("/login"), 2000);
        } catch (e: unknown) {
            setVerifyErr(getErrorMessage(e) || "Xác thực thất bại. Vui lòng thử lại.");
        } finally {
            setVerifying(false);
        }
    };

    const ruleItem = (ok: boolean, text: string) => (
        <li key={text} className={`password-rule ${ok ? 'valid' : 'invalid'}`}>
            <span aria-hidden>{ok ? "✅" : "❌"}</span>
            <span>{text}</span>
        </li>
    );

    // Hàm xử lý đăng ký bằng Google và chuyển hướng sau khi thành công
    const handleGoogleLoginSuccess = () => {
        setGoogleError(null);
        setIsRegistered(true);
        setVerifyOk("✅ Đăng ký qua Google thành công! Chuyển sang trang đăng nhập...");
        setTimeout(() => navigate("/login"), 2000); // Sau 2 giây chuyển hướng đến trang đăng nhập
    };

    return (
        <div className="register-page">
            <div className="register-container">
                <div className="register-card">
                    <div className="register-header">
                        <h1 className="register-title">Đăng ký tài khoản</h1>
                        <p className="register-subtitle">Tạo tài khoản mới để bắt đầu sử dụng StyleGenie</p>
                    </div>

                    {!isRegistered ? (
                        // ========== FORM ĐĂNG KÝ ==========
                        <form onSubmit={submit} className="register-form">
                            <div className="form-group">
                                <label htmlFor="fullName" className="form-label">Họ tên</label>
                                <input
                                    id="fullName"
                                    className="form-input"
                                    placeholder="Nhập họ tên của bạn (không bắt buộc)"
                                    value={form.fullName ?? ""}
                                    onChange={(e) => onChange("fullName", e.target.value)}
                                    autoComplete="name"
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="email" className="form-label">Email</label>
                                <input
                                    id="email"
                                    className="form-input"
                                    placeholder="Nhập email của bạn"
                                    type="email"
                                    value={form.email}
                                    onChange={(e) => onChange("email", e.target.value)}
                                    required
                                    autoComplete="email"
                                    inputMode="email"
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="password" className="form-label">Mật khẩu</label>
                                <div
                                    className="password-input-wrapper"
                                    onMouseEnter={() => setShowPwRules(true)}
                                    onMouseLeave={() => setShowPwRules(false)}
                                >
                                    <input
                                        id="password"
                                        className="form-input password-input"
                                        placeholder="Nhập mật khẩu của bạn"
                                        type={showPass ? "text" : "password"}
                                        value={form.password}
                                        onChange={(e) => onChange("password", e.target.value)}
                                        required
                                        autoComplete="new-password"
                                        onFocus={() => setShowPwRules(true)}
                                        onBlur={() => setShowPwRules(false)}
                                        aria-describedby="pw-rules"
                                    />
                                    <button
                                        type="button"
                                        className="password-toggle"
                                        aria-label={showPass ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                                        onClick={() => setShowPass((s) => !s)}
                                        onMouseDown={(e) => e.preventDefault()}
                                    >
                                        {showPass ? "Ẩn" : "Hiện"}
                                    </button>

                                    {showPwRules && (
                                        <div id="pw-rules" className="password-rules">
                                            <ul>
                                                {ruleItem(pwCheck.hasMinLen, `Ít nhất ${PASSWORD_RULES.minLen} ký tự`)}
                                                {PASSWORD_RULES.requireLower && ruleItem(pwCheck.hasLower, "Có chữ thường (a–z)")}
                                                {PASSWORD_RULES.requireUpper && ruleItem(pwCheck.hasUpper, "Có chữ hoa (A–Z)")}
                                                {PASSWORD_RULES.requireDigit && ruleItem(pwCheck.hasDigit, "Có chữ số (0–9)")}
                                                {PASSWORD_RULES.requireSpecial && ruleItem(pwCheck.hasSpecial, "Có ký tự đặc biệt")}
                                                {PASSWORD_RULES.noSpaces && ruleItem(pwCheck.hasNoSpaces, "Không chứa khoảng trắng")}
                                            </ul>
                                        </div>
                                    )}
                                </div>
                            </div>

                            {error && (
                                <div className="error-message">
                                    {error}
                                </div>
                            )}

                            <button
                                type="submit"
                                className="register-button"
                                disabled={!canSubmit}
                            >
                                {loading ? "Đang đăng ký..." : "Đăng ký"}
                            </button>

                            <div className="divider">
                                <div className="divider-line"></div>
                                <span className="divider-text">hoặc</span>
                                <div className="divider-line"></div>
                            </div>

                            <div className="google-login-wrapper">
                                <GoogleLoginButton
                                    onLoginSuccess={handleGoogleLoginSuccess}
                                    onError={(msg) => setGoogleError(msg)}
                                />
                                {googleError && (
                                    <div className="error-message google-error">
                                        {googleError}
                                    </div>
                                )}
                            </div>
                        </form>
                    ) : (
                        // ========== GIAI ĐOẠN XÁC THỰC EMAIL ==========
                        <div className="verify-section">
                            <div className="success-message">
                                <strong>Tạo tài khoản thành công!</strong>
                                <div className="success-details">
                                    Bạn cần xác minh email để kích hoạt tài khoản: <em>{registeredEmail}</em>
                                </div>
                            </div>

                            <div className="verify-instructions">
                                Chúng tôi đã gửi mã xác thực tới email của bạn. Vui lòng nhập mã bên dưới và bấm "Xác thực".
                            </div>

                            <form onSubmit={handleVerify} className="verify-form">
                                <div className="form-group">
                                    <label htmlFor="verifyCode" className="form-label">Mã xác thực</label>
                                    <input
                                        id="verifyCode"
                                        className="form-input"
                                        placeholder="Nhập mã xác thực"
                                        value={verifyCode}
                                        onChange={(e) => setVerifyCode(e.target.value)}
                                        required
                                        inputMode="numeric"
                                    />
                                </div>

                                {verifyErr && (
                                    <div className="error-message">
                                        {verifyErr}
                                    </div>
                                )}

                                {verifyOk && (
                                    <div className="success-message">
                                        {verifyOk}
                                    </div>
                                )}

                                <button
                                    type="submit"
                                    className="verify-button"
                                    disabled={verifying}
                                >
                                    {verifying ? "Đang xác thực..." : "Xác thực"}
                                </button>

                                <button
                                    type="button"
                                    className="resend-button"
                                    onClick={async () => {
                                        setVerifyErr(null);
                                        setVerifyOk(null);
                                        try {
                                            const email = (registeredEmail || form.email).trim();
                                            await authApi.resendOtp(email);
                                            setVerifyOk("📩 Đã gửi lại mã OTP. Vui lòng kiểm tra email.");
                                        } catch (e) {
                                            setVerifyErr(getErrorMessage(e) || "Gửi lại OTP thất bại.");
                                        }
                                    }}
                                >
                                    Gửi lại mã
                                </button>
                            </form>
                        </div>
                    )}

                    <div className="register-footer">
                        <div className="login-link">
                            <span>Đã có tài khoản? </span>
                            <Link to="/login" className="login-link-text">
                                Đăng nhập ngay
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
