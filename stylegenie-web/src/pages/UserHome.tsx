export default function UserHome() {
    return (
        <div style={containerStyle}>
            <h1>User Home</h1>
            <p>Chào mừng bạn đến với trang người dùng (User).</p>
        </div>
    );
}

const containerStyle: React.CSSProperties = {
    padding: 24,
    maxWidth: 800,
    margin: "0 auto",
};
