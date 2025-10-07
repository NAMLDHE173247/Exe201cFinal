const featureBlocks = [
  {
    id: 1,
    image: "https://c.animaapp.com/mg6w6o8vAvJ0o7/img/api-th------o.png",
    imagePosition: "left",
    subtitle: "Tìm thấy phong cách",
    title: "Mua ngay lập tức!",
    description:
      "Khám phá phong cách mới ngay lập tức với công nghệ Ai Try-On của StyleGenie. Tải ảnh, chọn đồ, và xem bạn trông như thế nào trước khi bạn mua sản phẩm đó.",
    primaryButton: "Thử đồ ngay",
    secondaryButton: "Xem demo",
  },
  {
    id: 2,
    image: "https://c.animaapp.com/mg6w6o8vAvJ0o7/img/image-21.png",
    imagePosition: "right",
    subtitle: "Khám phá",
    title: "Tone màu cá nhân với AI",
    description:
      "StyleGenie AI phân tích ảnh chân dung để xác định Personal Color (Warm, Cool, Neutral). Chỉ trong vài giây, bạn sẽ nhận gợi ý màu sắc phù hợp với làn da, mái tóc và gương mặt — giúp chọn trang phục, kiểu tóc và makeup nổi bật nhất.",
    primaryButton: "Khám phá ngay",
    secondaryButton: "Xem demo",
  },
  {
    id: 3,
    image:
      "https://c.animaapp.com/mg6w6o8vAvJ0o7/img/ti-t-ki-m-l-n-v-i-c-ng-c--thay---i-qu-n--o-ai.png",
    imagePosition: "left",
    subtitle: "Thả đồ thật như mơ",
    title: "Không cần mặc thử",
    description:
      "Với StyleGenie, bạn có thể thử đồ ảo và sở hữu trang phục yêu thích chỉ trong vài giây. Trải nghiệm mua sắm thông minh, tiện lợi và  không lo rủi ro.",
    primaryButton: "Mua ngay",
    secondaryButton: "Xem demo",
  },
];

export const FeaturedItemsSection = () => {
  return (
    <section className="features">
      <header className="features-header">
        <div className="features-subtitle">Tính Năng</div>
        <h2 className="features-title">Phối Đồ Mọi Lúc, Mọi Nơi</h2>
      </header>

      <div className="features-list">
        {featureBlocks.map((block) => (
          <div key={block.id} className="feature-item">
            {block.imagePosition === "left" ? (
              <>
                <div className="feature-image" style={{ backgroundImage: `url(${block.image})` }} />
                <div className="feature-content">
                  {block.subtitle && <div className="features-subtitle">{block.subtitle}</div>}
                  {block.title && <h3 className="features-title">{block.title}</h3>}
                  <p className="feature-desc">{block.description}</p>
                  <div className="feature-actions">
                    <button className="btn btn-gradient">{block.primaryButton}</button>
                    <button className="btn btn-outline-gradient">{block.secondaryButton}</button>
                  </div>
                </div>
              </>
            ) : (
              <>
                <div className="feature-content">
                  {block.subtitle && <div className="features-subtitle">{block.subtitle}</div>}
                  {block.title && <h3 className="features-title">{block.title}</h3>}
                  <p className="feature-desc">{block.description}</p>
                  <div className="feature-actions">
                    <button className="btn btn-gradient">{block.primaryButton}</button>
                    <button className="btn btn-outline-gradient">{block.secondaryButton}</button>
                  </div>
                </div>
                <img className="feature-image-img" alt="Feature" src={block.image} />
              </>
            )}
          </div>
        ))}
      </div>
    </section>
  );
};

export default FeaturedItemsSection;


