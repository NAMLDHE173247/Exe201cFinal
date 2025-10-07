export default function StaffHome() {
    return (
        <div style={containerStyle}>
            <h1>Staff Home</h1>
            <p>Đây là trang dành cho nhân viên (Staff).</p>
        </div>
    );
}

const containerStyle: React.CSSProperties = {
    padding: 24,
    maxWidth: 800,
    margin: "0 auto",
};
