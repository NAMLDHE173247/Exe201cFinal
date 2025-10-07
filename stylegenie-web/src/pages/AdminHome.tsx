export default function AdminHome() {
    return (
        <div style={containerStyle}>
            <h1>Admin Home</h1>
            <p>Chào mừng bạn đến trang dành cho Admin.</p>
        </div>
    );
}

const containerStyle: React.CSSProperties = {
    padding: 24,
    maxWidth: 800,
    margin: "0 auto",
};
