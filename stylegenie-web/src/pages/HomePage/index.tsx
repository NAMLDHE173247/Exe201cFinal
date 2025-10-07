import NavigationBarSection from "./sections/NavigationBarSection/NavigationBarSection";
import HeroBannerSection from "./sections/HeroBannerSection/HeroBannerSection";
import FeaturedItemsSection from "./sections/FeaturedItemsSection/FeaturedItemsSection";
import ProductShowcaseSection from "./sections/ProductShowcaseSection/ProductShowcaseSection";
import ContentSection from "./sections/ContentSection/ContentSection";
import "./styles.css";

export default function HomePage() {
  return (
    <div className="homepage-root">
      <NavigationBarSection />
      <HeroBannerSection />
      <div className="container">
        <FeaturedItemsSection />
        <ProductShowcaseSection />
      </div>
      <ContentSection />
    </div>
  );
}


