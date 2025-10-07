// src/pages/ForgotPasswordPage.tsx
import { useEffect, useMemo, useState } from "react";
import type { CSSProperties } from "react";
import { useNavigate } from "react-router-dom";
import { apiRequest } from "../api/client";

const sendOtpEndpoint = "/api/ForgotPassword/send-otp";
const resetEndpoint = "/api/ForgotPassword/reset";

const pageCss = `
html, body, #root { height: 100%; }
body.forgot-no-wrapper { margin: 0; padding: 0; width: 100vw; overflow-x: hidden; }
body.forgot-no-wrapper #root > div { max-width: none !important; width: 100% !important; margin: 0 !important; padding: 0 !important; }
`;

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

export default function ForgotPasswordPage() {
    const navigate = useNavigate();

    useEffect(() => {
        document.body.classList.add("forgot-no-wrapper");
        return () => document.body.classList.remove("forgot-no-wrapper");
    }, []);

    // step 1
    const [email, setEmail] = useState("");
    const [sending, setSending] = useState(false);
    const [sendOk, setSendOk] = useState<string | null>(null);
    const [sendErr, setSendErr] = useState<string | null>(null);

    // step 2
    const [otp, setOtp] = useState("");
    const [newPw, setNewPw] = useState("");
    const [showPw, setShowPw] = useState(false);
    const [resetting, setResetting] = useState(false);
    const [resetOk, setResetOk] = useState<string | null>(null);
    const [resetErr, setResetErr] = useState<string | null>(null);

    const emailLooksValid = useMemo(() => /\S+@\S+\.\S+/.test(email), [email]);
    const pwCheck = useMemo(() => checkPassword(newPw), [newPw]);

    const anyPwRuleFailed =
        !pwCheck.hasMinLen ||
        !pwCheck.hasLower ||
        !pwCheck.hasUpper ||
        !pwCheck.hasDigit ||
        !pwCheck.hasSpecial ||
        !pwCheck.hasNoSpaces;

    const sendOtp = async (ev: React.FormEvent) => {
        ev.preventDefault();
        setSendOk(null);
        setSendErr(null);

        if (!emailLooksValid) {
            setSendErr("Email không hợp lệ.");
            return;
        }

        try {
            setSending(true);
            await apiRequest(sendOtpEndpoint + `?email=${encodeURIComponent(email.trim())}`, {
                method: "POST",
                body: {}, // body rỗng; backend đọc query
            });
            setSendOk("📩 Đã gửi mã OTP tới email của bạn. Vui lòng kiểm tra hộp thư.");
            setSendErr(null);
        } catch (e: unknown) {
            // Backend có thể trả: "Tài khoản này đăng nhập bằng Google..."
            setSendErr(getErrorMessage(e) || "Gửi OTP thất bại.");
        } finally {
            setSending(false);
        }
    };

    const resetPassword = async (ev: React.FormEvent) => {
        ev.preventDefault();
        setResetOk(null);
        setResetErr(null);

        if (!otp.trim()) {
            setResetErr("Vui lòng nhập mã OTP.");
            return;
        }
        if (anyPwRuleFailed) {
            setResetErr("Mật khẩu mới chưa đạt yêu cầu.");
            return;
        }

        try {
            setResetting(true);
            await apiRequest(resetEndpoint, {
                method: "POST",
                body: {
                    email: email.trim(),
                    otp: otp.trim(),
                    newPassword: newPw,
                },
            });

            setResetOk("✅ Đặt lại mật khẩu thành công! Đang chuyển sang trang đăng nhập...");
            setResetErr(null);

            // chuyển sang login sau 2 giây
            setTimeout(() => navigate("/login"), 2000);
        } catch (e: unknown) {
            setResetErr(getErrorMessage(e) || "Đặt lại mật khẩu thất bại.");
        } finally {
            setResetting(false);
        }
    };

    // ===== styles
    const page: CSSProperties = {
        minHeight: "100svh",
        width: "100vw",
        display: "grid",
        placeItems: "center",
        background:
            "radial-gradient(1200px 600px at 10% 10%, rgba(99,102,241,0.15) 0%, rgba(99,102,241,0) 60%)," +
            "radial-gradient(900px 500px at 90% 30%, rgba(16,185,129,0.18) 0%, rgba(16,185,129,0) 60%)," +
            "linear-gradient(180deg, #0b0d12 0%, #0b0d12 100%)",
        color: "#fff",
        overflowX: "hidden",
        padding: 24,
    };

    const card: CSSProperties = {
        width: "min(520px, 92vw)",
        background: "rgba(255,255,255,0.06)",
        backdropFilter: "blur(10px)",
        borderRadius: 16,
        boxShadow: "0 10px 30px rgba(0,0,0,0.35)",
        padding: 22,
    };

    const title: CSSProperties = { margin: "0 0 8px 0", fontSize: 28, fontWeight: 800 };
    const dim: CSSProperties = { color: "rgba(255,255,255,0.75)" };
    const formGrid: CSSProperties = { display: "grid", gap: 12, marginTop: 12 };

    const input: CSSProperties = {
        width: "100%",
        padding: "10px 12px",
        borderRadius: 10,
        border: "1px solid #d1d5db",
        background: "#fff",
        color: "#111",
        outline: "none",
    };

    const inputWrap: CSSProperties = { position: "relative", width: "100%" };

    const toggleBtn: CSSProperties = {
        position: "absolute",
        right: 8,
        top: "50%",
        transform: "translateY(-50%)",
        background: "transparent",
        border: "none",
        color: "#2563eb",
        fontWeight: 600,
        cursor: "pointer",
        padding: "4px 6px",
        lineHeight: 1,
    };

    const btn: CSSProperties = {
        width: "100%",
        padding: "10px 14px",
        borderRadius: 10,
        border: "none",
        background: "#1677ff",
        color: "#fff",
        fontWeight: 700,
        cursor: "pointer",
    };

    const bannerOk: CSSProperties = {
        background: "rgba(34,197,94,0.15)",
        border: "1px solid rgba(34,197,94,0.35)",
        color: "#bbf7d0",
        padding: 10,
        borderRadius: 10,
    };
    const bannerErr: CSSProperties = {
        background: "rgba(239,68,68,0.15)",
        border: "1px solid rgba(239,68,68,0.35)",
        color: "#fecaca",
        padding: 10,
        borderRadius: 10,
    };

    const ruleItem = (ok: boolean, text: string) => (
        <li key={text} style={{ display: "flex", gap: 8, alignItems: "center", color: ok ? "#86efac" : "#fecaca", fontSize: 13 }}>
            <span aria-hidden>{ok ? "✅" : "❌"}</span>
            <span>{text}</span>
        </li>
    );

    return (
        <>
            <style>{pageCss}</style>
            <div style={page}>
                <div style={card}>
                    <h2 style={title}>Quên mật khẩu</h2>
                    <p style={{ ...dim, marginTop: 0 }}>
                        Nhập email của bạn để nhận mã OTP đặt lại mật khẩu.
                    </p>

                    {/* Bước 1: Nhập email và gửi OTP */}
                    <form onSubmit={sendOtp} style={formGrid}>
                        <input
                            style={input}
                            type="email"
                            placeholder="Email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                            autoComplete="email"
                            inputMode="email"
                        />
                        {sendErr && <div style={bannerErr}>{sendErr}</div>}
                        {sendOk && <div style={bannerOk}>{sendOk}</div>}

                        <button type="submit" style={{ ...btn, opacity: sending ? 0.7 : 1 }} disabled={sending}>
                            {sending ? "Đang gửi..." : "Gửi mã OTP"}
                        </button>
                    </form>

                    {/* Bước 2: Nhập OTP + mật khẩu mới (chỉ hiện sau khi đã gửi OTP) */}
                    {sendOk && (
                        <div style={{ marginTop: 18 }}>
                            <p style={{ ...dim, marginTop: 0 }}>
                                Vui lòng nhập mã OTP và đặt mật khẩu mới.
                            </p>
                            <form onSubmit={resetPassword} style={{ display: "grid", gap: 12 }}>
                                <input
                                    style={input}
                                    placeholder="Mã OTP"
                                    value={otp}
                                    onChange={(e) => setOtp(e.target.value)}
                                    required
                                    inputMode="numeric"
                                />

                                <div style={inputWrap}>
                                    <input
                                        style={{ ...input, paddingRight: 70 }}
                                        placeholder="Mật khẩu mới"
                                        type={showPw ? "text" : "password"}
                                        value={newPw}
                                        onChange={(e) => setNewPw(e.target.value)}
                                        required
                                        autoComplete="new-password"
                                    />
                                    <button
                                        type="button"
                                        aria-label={showPw ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                                        onClick={() => setShowPw((s) => !s)}
                                        onMouseDown={(e) => e.preventDefault()}
                                        style={toggleBtn}
                                    >
                                        {showPw ? "Ẩn" : "Hiện"}
                                    </button>
                                </div>

                                {/* Hiện rule kiểm tra mật khẩu */}
                                <div style={{ background: "rgba(0,0,0,0.35)", border: "1px solid rgba(255,255,255,0.15)", padding: 10, borderRadius: 10 }}>
                                    <ul style={{ margin: 0, paddingLeft: 18 }}>
                                        {ruleItem(pwCheck.hasMinLen, `Ít nhất ${PASSWORD_RULES.minLen} ký tự`)}
                                        {PASSWORD_RULES.requireLower && ruleItem(pwCheck.hasLower, "Có chữ thường (a–z)")}
                                        {PASSWORD_RULES.requireUpper && ruleItem(pwCheck.hasUpper, "Có chữ hoa (A–Z)")}
                                        {PASSWORD_RULES.requireDigit && ruleItem(pwCheck.hasDigit, "Có chữ số (0–9)")}
                                        {PASSWORD_RULES.requireSpecial && ruleItem(pwCheck.hasSpecial, "Có ký tự đặc biệt")}
                                        {PASSWORD_RULES.noSpaces && ruleItem(pwCheck.hasNoSpaces, "Không chứa khoảng trắng")}
                                    </ul>
                                </div>

                                {resetErr && <div style={bannerErr}>{resetErr}</div>}
                                {resetOk && <div style={bannerOk}>{resetOk}</div>}

                                <button type="submit" style={{ ...btn, opacity: resetting ? 0.7 : 1 }} disabled={resetting}>
                                    {resetting ? "Đang đặt lại..." : "Đặt lại mật khẩu"}
                                </button>
                            </form>
                        </div>
                    )}
                </div>
            </div>
        </>
    );
}
