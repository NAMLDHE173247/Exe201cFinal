// src/layouts/MainLayout.tsx
import { Outlet } from "react-router-dom";
import NavigationBarSection from "../sections/NavigationBarSection/NavigationBarSection";

export default function MainLayout() {
    return (
        <div>
            <NavigationBarSection />
            {/* Outlet sẽ render page con */}
            <Outlet />
        </div>
    );
}
