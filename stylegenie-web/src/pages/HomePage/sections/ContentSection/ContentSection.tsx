export const ContentSection = () => {
  const navigationItems = [
    "Trang chủ",
    "Thử đồ ảo",
    "Phân tích cá nhân hóa",
    "Tủ đồ",
    "Góp VIP",
  ];

  const socialIcons = [
    {
      src: "https://c.animaapp.com/mg6w6o8vAvJ0o7/img/icon.svg",
      alt: "Social Icon 1",
    },
    {
      src: "https://c.animaapp.com/mg6w6o8vAvJ0o7/img/icon-2.svg",
      alt: "Social Icon 2",
    },
    {
      src: "https://c.animaapp.com/mg6w6o8vAvJ0o7/img/icon-1.svg",
      alt: "Social Icon 3",
    },
  ];

  return (
    <section className="contact">
      <div className="contact-left">
        <nav className="contact-links">
          <div className="contact-links-row">
            {navigationItems.map((item, index) => (
              <div key={index} className="contact-link">
                {item}
              </div>
            ))}
          </div>
        </nav>

        <h2 className="contact-title">Liên hệ</h2>

        <div className="contact-socials">
          {socialIcons.map((icon, index) => (
            <img key={index} className="contact-social" alt={icon.alt} src={icon.src} />
          ))}
        </div>

        <p className="contact-desc">
          Cần tư vấn sản phẩm hợp tác, hay hỗ trợ kỹ thuật?
          <br />
          Hãy gửi tin nhắn cho chúng tôi - phản hồi trong giờ hành chính
        </p>

        <div className="contact-brand">
          <img className="brand-logo" alt="StyleGenie Logo" src="https://c.animaapp.com/mg6w6o8vAvJ0o7/img/group-1.png" />
          <div className="brand-name">StyleGenie</div>
        </div>
      </div>

      <div className="contact-form">
        <button className="btn btn-gradient">Gửi tin nhắn</button>

        <div className="input input-half">
          <input placeholder="Họ và tên" />
        </div>

        <div className="input input-half right">
          <input placeholder="Email" type="email" />
        </div>

        <div className="textarea">
          <textarea placeholder="Nội dung tin nhắn" />
        </div>
      </div>
    </section>
  );
};

export default ContentSection;


