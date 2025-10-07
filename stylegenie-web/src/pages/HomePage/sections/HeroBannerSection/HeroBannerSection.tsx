import styles from "./HeroBannerSection.module.css";
import Button from "../../components/Button";

export const HeroBannerSection = () => {
  return (
    <section className={styles.hero}>
      <div className={styles.container}>
        <div className={styles.left}>
          <h1 className={styles.title}>
            <span className={styles.line1}>
              Khám phá <span className={styles.textGradient}>PHONG CÁCH</span>
            </span>
            
            <span className={styles.line2}>của riêng bản thân</span>
          </h1>

          <p className={styles.desc}>
            Nền tảng thời trang AI đầu tiên tại Việt Nam giúp bạn tìm kiếm phong
            cách hoàn hảo. Thử đồ ảo, phối đồ outfit thông minh và nhận gợi ý cá
            nhân hóa từ AI.
          </p>

          <div className={styles.ctas}>
            <Button> Bắt đầu ngay miễn phí</Button>
            <Button variant="outline-gradient">Xem demo</Button>
          </div>
        </div>

        <div>
          <img
            className={styles.image}
            alt="Fashion AI platform demonstration"
            src="https://c.animaapp.com/mg6w6o8vAvJ0o7/img/faa58e15-4206-40f1-9e84-b7200ea3abdf-1.png"
          />
        </div>
      </div>
    </section>
  );
};

export default HeroBannerSection;
