import { GoogleLogin } from "@react-oauth/google";
import type { CredentialResponse } from "@react-oauth/google"; // Chỉ import type
import Cookies from "js-cookie";
import { authApi } from "../api/auth";
import type { UserResponse } from "../api/auth";

type Props = {
    onLoginSuccess?: (data: { token: string; user: UserResponse }) => void;
    onError?: (message: string) => void;
};

export default function GoogleLoginButton({ onLoginSuccess, onError }: Props) {
    const handleSuccess = async (credentialResponse: CredentialResponse) => {
        const idToken = credentialResponse?.credential;
        if (!idToken) {
            onError?.("Không nhận được mã xác thực từ Google.");
            return;
        }
        try {
            const res = await authApi.loginWithGoogle(idToken);
            // Lưu token vào cookie
            Cookies.set("AuthToken", res.token, { expires: 1 }); // Lưu token trong 1 ngày
            onLoginSuccess?.({ token: res.token, user: res.user });
        } catch (err: any) {
            onError?.(err?.message || "Đăng nhập Google thất bại");
        }
    };

    return (
        <GoogleLogin
            onSuccess={handleSuccess}
            onError={() => onError?.("Đăng nhập Google thất bại")}
            useOneTap
        />
    );
}
