import { Link, useLocation } from "react-router-dom";

export default function Nav() {
  const loc = useLocation();
  const A = ({ to, children }: any) => (
    <Link to={to} style={{
      padding:"8px 12px", borderRadius:8,
      background: loc.pathname===to ? "#1677ff" : "#f0f0f0",
      color: loc.pathname===to ? "white" : "black",
      textDecoration:"none", fontWeight:600, marginRight:8
    }}>{children}</Link>
  );
  return (
    <div style={{padding:12, borderBottom:"1px solid #eee"}}>
      <A to="/">Try-On</A><A to="/skin">Skin Tone</A>
      <A to="/affiliate">Affiliate</A><A to="/admin">Admin</A>
    </div>
  );
}
