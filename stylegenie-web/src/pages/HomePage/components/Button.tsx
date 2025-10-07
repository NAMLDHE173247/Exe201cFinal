import styles from "./components.module.css";

type ButtonVariant = "gradient" | "outline-gradient" | "ghost";
type ButtonSize = "md" | "lg" | "sm";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  fullWidth?: boolean;
}

export default function Button({
  variant = "gradient",
  size = "md",
  fullWidth,
  className,
  children,
  ...props
}: ButtonProps) {
  const base = styles.btn;
  const sizeClass = size === "lg" ? styles["btn-lg"] : size === "sm" ? styles["btn-sm"] : styles["btn-md"];
  const variantClass =
    variant === "outline-gradient"
      ? styles["btn-outline-gradient"]
      : variant === "ghost"
      ? styles["btn-ghost"]
      : styles["btn-gradient"];
  const widthClass = fullWidth ? styles["btn-block"] : "";

  return (
    <button className={[base, sizeClass, variantClass, widthClass, className].filter(Boolean).join(" ")} {...props}>
      {children}
    </button>
  );
}


