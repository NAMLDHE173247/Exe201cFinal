import { useState, useRef } from "react";
import styled, { createGlobalStyle } from "styled-components";

// ====== Import ảnh từng dáng người ======
import maleApple from "../assets/male_apple.png";
import malePear from "../assets/male_pear.png";
import maleTriangle from "../assets/male_triangle.png";
import maleRectangle from "../assets/male_rectangle.png";
import maleHourglass from "../assets/male_hourglass.png";

import femaleApple from "../assets/female_apple.png";
import femalePear from "../assets/female_pear.png";
import femaleTriangle from "../assets/female_triangle.png";
import femaleRectangle from "../assets/female_rectangle.png";
import femaleHourglass from "../assets/female_hourglass.png";

/* ================= GLOBAL STYLE ================= */
const GlobalStyle = createGlobalStyle`
  * { box-sizing: border-box; }
  html, body, #root {
    margin: 0;
    padding: 0;
    width: 100vw;
    height: 100vh;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
    background-color: #f0f2f5;
  }
`;

/* ================= LAYOUT ================= */
const FullScreenWrapper = styled.div`
  position: fixed;
  inset: 0;
  display: flex;
  height: 100vh;
  width: 100vw;
  background: #f0f2f5;
  overflow: hidden;
  @media (max-width: 900px) {
    flex-direction: column;
    height: auto;
    overflow: visible;
  }
`;

const LeftPanel = styled.div`
  flex-basis: 45%;
  max-width: 600px;
  min-width: 380px;
  background: #fff;
  padding: 30px;
  border-right: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  gap: 20px;
  overflow-y: auto;
`;

const RightPanel = styled.div`
  flex: 1;
  background-color: #fafaff;
  padding: 40px 50px;
  overflow-y: auto;
`;

/* ================= UI ELEMENTS ================= */
const Header = styled.h1`
  font-size: 26px;
  color: #764ba2;
  margin: 0 0 10px;
`;

const UploadArea = styled.div<{ $hasPreview?: boolean }>`
  border: 2px dashed ${p => p.$hasPreview ? '#667eea' : '#ddd'};
  padding: 10px;
  border-radius: 12px;
  background: #fafafa;
  min-height: 280px;
  cursor: pointer;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  &:hover { background-color: #f5f5ff; border-color: #764ba2; }
`;

const FileInput = styled.input` display: none; `;

const ImagePreview = styled.img`
  position: absolute;
  width: 100%;
  height: 100%;
  object-fit: contain;
  border-radius: 10px;
  top: 0;
  left: 0;
`;

const RemoveImageBtn = styled.button`
  position: absolute;
  top: 15px;
  right: 15px;
  width: 30px;
  height: 30px;
  border: none;
  background: rgba(0,0,0,0.6);
  color: #fff;
  border-radius: 50%;
  cursor: pointer;
`;

const FormSection = styled.div`
  background: #f8f9ff;
  padding: 20px;
  border-radius: 12px;
  border: 1px solid #e6e8ff;
`;

const Label = styled.label`
  display: block;
  margin-bottom: 12px;
  font-weight: 600;
`;

const SelectBox = styled.select`
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e6e8ff;
  border-radius: 10px;
`;

const SkinToneGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(30px, 1fr));
  gap: 10px;
`;

const SkinToneBox = styled.div<{ $color: string; $isSelected: boolean }>`
  width: 100%;
  aspect-ratio: 1;
  border-radius: 8px;
  background-color: ${p => p.$color};
  border: 3px solid ${p => p.$isSelected ? "#667eea" : "transparent"};
  cursor: pointer;
  transition: 0.2s;
  &:hover { transform: scale(1.05); }
`;

const BodyTypeGrid = styled.div`
  display: flex;
  justify-content: space-between;
  gap: 12px;
  overflow-x: auto;
  padding-bottom: 10px;

  &::-webkit-scrollbar { height: 6px; }
  &::-webkit-scrollbar-thumb { background: #ccc; border-radius: 3px; }
`;

const BodyImageBox = styled.div<{ $isSelected: boolean }>`
  text-align: center;
  border: 2px solid ${p => (p.$isSelected ? "#764ba2" : "transparent")};
  border-radius: 8px;
  padding: 5px;
  background-color: #fff;
  cursor: pointer;
  width: 18%;
  min-width: 90px;
  transition: all 0.3s ease;
  
  &:hover {
    transform: scale(1.05);
    box-shadow: 0 0 10px rgba(118, 75, 162, 0.2);
  }
`;

const BodyImage = styled.img`
  width: 100%;
  height: auto;
  border-radius: 6px;
  display: block;
  margin: 0 auto;
`;

const BodyLabel = styled.div`
  font-size: 13px;
  color: #333;
  margin-top: 6px;
  text-align: center;
`;

const SubmitBtn = styled.button`
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: #fff;
  border: none;
  padding: 16px 20px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  width: 100%;
  margin-top: auto;
  &:disabled { background: #ccc; cursor: not-allowed; }
`;

const PlaceholderText = styled.div`
  text-align: center;
  color: #888;
  font-style: italic;
  margin-top: 40px;
`;

const TabContainer = styled.div`
  display: flex;
  gap: 20px;
  border-bottom: 1px solid #e0e0e0;
  margin-bottom: 30px;
`;

const TabButton = styled.button<{ $isActive: boolean }>`
  border: none;
  background: none;
  cursor: pointer;
  font-weight: 600;
  padding: 10px 20px;
  color: ${p => p.$isActive ? "#764ba2" : "#888"};
  border-bottom: 3px solid ${p => p.$isActive ? "#764ba2" : "transparent"};
`;

const ResultsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 25px;
`;

const ProductCard = styled.div`
  background: #fff;
  border-radius: 15px;
  box-shadow: 0 8px 25px rgba(0,0,0,0.08);
  overflow: hidden;
  transition: 0.2s;
  &:hover { transform: translateY(-5px); }
`;

const ProductImage = styled.div`
  width: 100%;
  aspect-ratio: 3/4;
  background-color: #eee;
  background-size: cover;
  background-position: center;
`;

const ProductInfo = styled.div` padding: 15px; `;

const ProductName = styled.h4`
  margin: 0 0 5px;
  font-size: 16px;
`;

const BuyButton = styled.a`
  display: inline-block;
  margin-top: 8px;
  background: #764ba2;
  color: white;
  padding: 8px 12px;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  &:hover { background: #8b63c2; }
`;

/* ================= COMPONENT ================= */
export default function SkinTone() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedGender, setSelectedGender] = useState("female");
  const [selectedTone, setSelectedTone] = useState("");
  const [selectedBodyType, setSelectedBodyType] = useState("");
  const [activeTab, setActiveTab] = useState(0);
  const [loading, setLoading] = useState(false);
  const [aiStyles, setAiStyles] = useState<
    { title: string; items: { id: number; name: string; price: number; imageBase64?: string; affiliateUrl?: string }[] }[]
  >([]);

  /* ================= HANDLE IMAGE ================= */
  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      setPreviewUrl(URL.createObjectURL(file));
    }
  };

  const handleRemoveImage = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.stopPropagation();
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setSelectedFile(null);
    setPreviewUrl(null);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  /* ================= CONSTANTS ================= */
  const skinTones = [
    { id: "fair", color: "#fde6d8" }, { id: "light", color: "#f5c997" },
    { id: "medium", color: "#d49968" }, { id: "olive", color: "#c17f4f" },
    { id: "tan", color: "#9d6b44" }, { id: "deep", color: "#5d3a27" },
  ];

  const bodyTypes =
    selectedGender === "male"
      ? [
        { value: "apple", img: maleApple, label: "Quả táo" },
        { value: "pear", img: malePear, label: "Quả lê" },
        { value: "triangle", img: maleTriangle, label: "Tam giác ngược" },
        { value: "rectangle", img: maleRectangle, label: "Chữ nhật" },
        { value: "hourglass", img: maleHourglass, label: "Đồng hồ cát" },
      ]
      : [
        { value: "apple", img: femaleApple, label: "Quả táo" },
        { value: "pear", img: femalePear, label: "Quả lê" },
        { value: "triangle", img: femaleTriangle, label: "Tam giác ngược" },
        { value: "rectangle", img: femaleRectangle, label: "Chữ nhật" },
        { value: "hourglass", img: femaleHourglass, label: "Đồng hồ cát" },
      ];

  /* ================= HANDLE SUBMIT ================= */
  const handleGenerate = async () => {
    if (!selectedFile || !selectedTone || !selectedGender || !selectedBodyType) {
      alert("⚠️ Vui lòng chọn đầy đủ thông tin trước khi gửi!");
      return;
    }

    setLoading(true);
    setAiStyles([]);

    const formData = new FormData();
    formData.append("image", selectedFile);
    formData.append("skinTone", selectedTone);
    formData.append("gender", selectedGender);
    formData.append("bodyType", selectedBodyType);

    try {
      const response = await fetch("/api/style/analyze", { method: "POST", body: formData });
      const text = await response.text();
      if (!response.ok) throw new Error(`Lỗi từ server: ${text}`);
      const data = JSON.parse(text);

      if (data && Array.isArray(data.recommendations)) {
        setAiStyles(data.recommendations);
        setActiveTab(0);
      } else {
        setAiStyles([]);
        alert("AI chưa có gợi ý phù hợp, vui lòng thử lại!");
      }
    } catch (err) {
      console.error("❌ Lỗi khi gọi API:", err);
      alert((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  /* ================= RENDER ================= */
  return (
    <>
      <GlobalStyle />
      <FullScreenWrapper>
        <LeftPanel>
          <Header>✨ Style Genie</Header>

          {/* Upload ảnh */}
          <FormSection>
            <Label>Ảnh của bạn</Label>
            <UploadArea $hasPreview={!!previewUrl} onClick={() => fileInputRef.current?.click()}>
              <FileInput type="file" accept="image/*" ref={fileInputRef} onChange={handleImageChange} />
              {previewUrl ? (
                <>
                  <ImagePreview src={previewUrl} alt="preview" />
                  <RemoveImageBtn onClick={handleRemoveImage}>×</RemoveImageBtn>
                </>
              ) : (
                <PlaceholderText>📸 Bấm để chọn hoặc kéo ảnh vào đây</PlaceholderText>
              )}
            </UploadArea>
          </FormSection>

          {/* Giới tính & Màu da */}
          <FormSection>
            <Label>Giới tính & Màu da</Label>
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
              <SelectBox value={selectedGender} onChange={(e) => setSelectedGender(e.target.value)}>
                <option value="female">Nữ</option>
                <option value="male">Nam</option>
              </SelectBox>

              <SkinToneGrid>
                {skinTones.map((t) => (
                  <SkinToneBox
                    key={t.id}
                    $color={t.color}
                    $isSelected={selectedTone === t.id}
                    onClick={() => setSelectedTone(t.id)}
                  />
                ))}
              </SkinToneGrid>
            </div>
          </FormSection>

          {/* Dáng người */}
          <FormSection>
            <Label>Dáng người của bạn</Label>
            <BodyTypeGrid>
              {bodyTypes.map((b) => (
                <BodyImageBox
                  key={b.value}
                  $isSelected={selectedBodyType === b.value}
                  onClick={() => setSelectedBodyType(b.value)}
                >
                  <BodyImage src={b.img} alt={b.label} />
                  <BodyLabel>{b.label}</BodyLabel>
                </BodyImageBox>
              ))}
            </BodyTypeGrid>
          </FormSection>

          <SubmitBtn onClick={handleGenerate} disabled={loading}>
            {loading ? "🔮 Đang phân tích..." : "Nhận gợi ý từ AI"}
          </SubmitBtn>
        </LeftPanel>

        {/* Kết quả */}
        <RightPanel>
          {loading ? (
            <PlaceholderText>⏳ AI đang phân tích phong cách cho bạn...</PlaceholderText>
          ) : !Array.isArray(aiStyles) || aiStyles.length === 0 ? (
            <PlaceholderText>💡 Hãy chọn thông tin và nhấn “Nhận gợi ý từ AI”</PlaceholderText>
          ) : (
            <>
              <TabContainer>
                {aiStyles.map((style, i) => (
                  <TabButton key={i} $isActive={activeTab === i} onClick={() => setActiveTab(i)}>
                    {style.title}
                  </TabButton>
                ))}
              </TabContainer>

              <ResultsGrid>
                {aiStyles[activeTab]?.items?.length > 0 ? (
                  aiStyles[activeTab].items.map((item) => (
                    <ProductCard key={item.id}>
                      <ProductImage
                        style={{
                          backgroundImage: item.imageBase64
                            ? `url(${item.imageBase64.startsWith("data:image")
                              ? item.imageBase64
                              : `data:image/jpeg;base64,${item.imageBase64}`})`
                            : undefined,
                        }}
                      />
                      <ProductInfo>
                        <ProductName>{item.name}</ProductName>
                        <p style={{ color: "#667eea", fontWeight: 600 }}>
                          {item.price?.toLocaleString("vi-VN")}₫
                        </p>
                        {item.affiliateUrl && (
                          <BuyButton href={item.affiliateUrl} target="_blank" rel="noopener noreferrer">
                            Mua ngay 🛒
                          </BuyButton>
                        )}
                      </ProductInfo>
                    </ProductCard>
                  ))
                ) : (
                  <PlaceholderText>Không có sản phẩm phù hợp.</PlaceholderText>
                )}
              </ResultsGrid>
            </>
          )}
        </RightPanel>
      </FullScreenWrapper>
    </>
  );
}
