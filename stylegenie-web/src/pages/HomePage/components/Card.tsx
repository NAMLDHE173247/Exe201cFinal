import styles from "./components.module.css";

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {}

export default function Card({ className, ...props }: CardProps) {
  return <div className={[styles.card, className].filter(Boolean).join(" ")} {...props} />;
}


