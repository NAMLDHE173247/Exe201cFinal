import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { GoogleOAuthProvider } from "@react-oauth/google";
import App from "./App";
import "antd/dist/reset.css";
import "./index.css";

import { AuthProvider } from "./contexts/AuthContext";
import { PaymentProvider } from "./contexts/PaymentContext"; // ✅ thêm dòng này

// ✅ CLIENT_ID lấy từ Google Developer Console
const CLIENT_ID = "418794469453-qevr8bhas6q4eokp13akoa87fb0utb77.apps.googleusercontent.com";

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
            <BrowserRouter>
                <AuthProvider>
                    <PaymentProvider>
                        <App />
                    </PaymentProvider>
                </AuthProvider>
            </BrowserRouter>
        </GoogleOAuthProvider>
    </React.StrictMode>
);
