// src/App.tsx
import { Routes, Route } from "react-router-dom";
import TryOn from "./pages/TryOn";
import SkinTone from "./pages/SkinTone";
import Affiliate from "./pages/Affiliate";
import Admin from "./pages/Admin";
import RegisterPage from "./pages/RegisterPage";
import AdminHome from "./pages/AdminHome";
import StaffHome from "./pages/StaffHome";
import UserHome from "./pages/UserHome";
import LoginPage from "./pages/LoginPage";
import HomePage from "./pages/HomePage";
import ForgotPasswordPage from "./pages/ForgotPasswordPage";
import MainLayout from "./pages/HomePage/layout/MainLayout";
import ManagerAccount from "./pages/ManagerAccount";
import ManagerProductItem from "./pages/ManagerProductItem";
import AddProductItem from "./pages/AddProductItem";
import EditProductItem from "./pages/EditProductItem";
import ApiKeyUpdate from "./pages/ApiKeyUpdate";
import PaymentSuccess from "./pages/HomePage/sections/ProductShowcaseSection/PaymentSuccess";
import PaymentCancel from "./pages/HomePage/sections/ProductShowcaseSection/PaymentCancel";
import PrivateRoute from "./pages/HomePage/components/PrivateRoute.tsx";

export default function App() {
    return (
        <Routes>
            {/* ✅ Trang PUBLIC — KHÔNG có navbar */}
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/payment/success" element={<PaymentSuccess />} />
            <Route path="/payment/cancel" element={<PaymentCancel />} />

            {/* ✅ Các trang CÓ NAVBAR (MainLayout bọc ngoài) */}
            <Route element={<MainLayout />}>
                <Route path="/tryon" element={<TryOn />} />
                <Route path="/skin" element={<SkinTone />} />
                <Route path="/affiliate" element={<Affiliate />} />

                {/* 🔒 Các route yêu cầu đăng nhập và phân quyền */}
                <Route
                    path="/user-home"
                    element={
                        <PrivateRoute allowedRoles={["3"]}>
                            <UserHome />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/staff-home"
                    element={
                        <PrivateRoute allowedRoles={["2"]}>
                            <StaffHome />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/admin-home"
                    element={
                        <PrivateRoute allowedRoles={["1"]}>
                            <AdminHome />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/admin"
                    element={
                        <PrivateRoute allowedRoles={["1"]}>
                            <Admin />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/manager-account"
                    element={
                        <PrivateRoute allowedRoles={["1"]}>
                            <ManagerAccount />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/manager-products"
                    element={
                        <PrivateRoute allowedRoles={["1", "2"]}>
                            <ManagerProductItem />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/manager-products/add"
                    element={
                        <PrivateRoute allowedRoles={["1", "2"]}>
                            <AddProductItem />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/manager-products/:id/edit"
                    element={
                        <PrivateRoute allowedRoles={["1", "2"]}>
                            <EditProductItem />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/admin/apikey"
                    element={
                        <PrivateRoute allowedRoles={["1"]}>
                            <ApiKeyUpdate />
                        </PrivateRoute>
                    }
                />
            </Route>
        </Routes>
    );
}
