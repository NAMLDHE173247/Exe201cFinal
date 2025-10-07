import { useEffect, useState } from "react";
import { apiGet, apiPost } from "../api/client";
import { useAuth } from "../contexts/AuthContext";
import { Plus, Zap, Crown, ChevronLeft, ChevronRight, ShoppingBag, Wallet } from "lucide-react";




/* =========================================================
   Helper function cn()
   ========================================================= */
function cn(...classes: (string | undefined | null | false)[]) {
  return classes.filter(Boolean).join(" ");
}




/* =========================================================
   UI COMPONENTS INLINE
   ========================================================= */




// Button
function Button({
  children,
  className,
  variant = "default",
  size = "default",
  ...props
}: React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "default" | "outline" | "ghost";
  size?: "default" | "sm" | "lg";
}) {
  const base =
    "inline-flex items-center justify-center gap-2 rounded-md text-sm font-medium transition disabled:opacity-50 disabled:pointer-events-none";
  const variants: Record<string, string> = {
    default: "bg-pink-500 text-white hover:bg-pink-600",
    outline: "border border-gray-300 bg-white hover:bg-gray-50 text-gray-700",
    ghost: "bg-transparent hover:bg-gray-100 text-gray-700",
  };
  const sizes: Record<string, string> = {
    default: "px-4 py-2",
    sm: "px-3 py-1 text-sm",
    lg: "px-6 py-3 text-lg",
  };




  return (
    <button className={cn(base, variants[variant], sizes[size], className)} {...props}>
      {children}
    </button>
  );
}




// Card
function Card({
  children,
  className,
  ...props
}: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className={cn("bg-white border rounded-lg shadow-sm", className)} {...props}>
      {children}
    </div>
  );
}




function CardContent({
  children,
  className,
  ...props
}: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className={cn("p-4", className)} {...props}>
      {children}
    </div>
  );
}




/* =========================================================
   MAIN PAGE: TRYON
   ========================================================= */




type Item = {
  id: number;
  name: string;
  price?: number;
  material?: string;
  category?: string;
  affiliateUrl?: string;
  imageBase64?: string;
  isVipOnly: boolean;
  isActive: boolean;
  createdAt: string;
};




type TryOnResp = {
  imageBase64?: string;
  jobId?: number;
  ImageBase64?: string;
  JobId?: number;
};




export default function TryOn() {
  const { user } = useAuth();
  const [balance, setBalance] = useState<number | null>(null);
  const [selectedModel, setSelectedModel] = useState<File | null>(null);
  const [previewModel, setPreviewModel] = useState<string | null>(null);




  const [items, setItems] = useState<Item[]>([]);
  const [selected, setSelected] = useState<Record<string, Item | null>>({
    upper: null,
    lower: null,
    fullset: null,
  });




  const [selectedCategory, setSelectedCategory] = useState("upper");
  const [selectedMaterial, setSelectedMaterial] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(12);




  const [loading, setLoading] = useState(false);
  const [resultUrl, setResultUrl] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);




  /* =========================================================
     Load items
     ========================================================= */
  useEffect(() => {
    apiGet<Item[]>("/api/Item")
      .then((data) => setItems(data))
      .catch(() => setError("Không tải được danh sách sản phẩm"));
  }, []);
  // ✅ Load balance
  useEffect(() => {
    const tenantId = (user as any)?.tenantId;
    if (!tenantId) {
      console.warn("⚠️ Không tìm thấy tenantId trong user", user);
      return;
    }




    console.log("🔄 Đang gọi API balance cho tenantId =", tenantId);




    apiGet<{ tenantId: number; balance: number }>(`/api/admin/wallet/balance?tenantId=${tenantId}`)
      .then((res) => {
        console.log("✅ Balance response:", res);
        setBalance(res.balance);
      })
      .catch(async (err) => {
        console.error("❌ Lỗi lấy balance:", err);
        setBalance(0);
      });
  }, [user]);




  /* =========================================================
     File to base64
     ========================================================= */
  const fileToBase64 = (file: File): Promise<string> =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve((reader.result as string).split(",")[1] || "");
      reader.onerror = (err) => reject(err);
      reader.readAsDataURL(file);
    });




  /* =========================================================
     Handle model upload
     ========================================================= */
  const handleModelSelect = (file: File | null) => {
    setSelectedModel(file);
    setPreviewModel(file ? URL.createObjectURL(file) : null);
  };




  /* =========================================================
     Handle Try-On
     ========================================================= */
  const handleTryOn = async () => {
    setError(null);
    setResultUrl(null);




    const hasUpper = !!selected.upper;
    const hasLower = !!selected.lower;
    const hasFull = !!selected.fullset;




    if (!selectedModel || (!hasUpper && !hasLower && !hasFull)) {
      setError("Vui lòng chọn ảnh model và ít nhất một sản phẩm.");
      return;
    }




    const tenantId = (user as any)?.tenantId;
    if (!tenantId) {
      setError("Không xác định được tenantId. Vui lòng đăng nhập lại.");
      return;
    }




    try {
      setLoading(true);
      const modelB64 = await fileToBase64(selectedModel);




      const itemImg = hasFull
        ? selected.fullset?.imageBase64
        : hasUpper
          ? selected.upper?.imageBase64
          : selected.lower?.imageBase64;




      if (!itemImg) throw new Error("Sản phẩm không có ảnh.");




      const requestBody = {
        tenantId,
        clothType: hasFull ? "full" : hasUpper ? "upper" : "lower",
        modelJpeg: modelB64,
        clothJpeg: itemImg.replace(/^data:image\/\w+;base64,/, ""),
      };




      const response = await apiPost<TryOnResp>("/api/vto/tryon", requestBody);
      const base64 = response?.imageBase64 ?? response?.ImageBase64;
      if (base64) setResultUrl(`data:image/jpeg;base64,${base64}`);
    } catch (err: any) {
      setError(err.message || "Lỗi không xác định.");
    } finally {
      setLoading(false);
    }
  };




  /* =========================================================
     Filters
     ========================================================= */




  const categories = [
    { id: "", label: "Tất cả" },
    { id: "upper", label: "Áo" },
    { id: "lower", label: "Quần" },
    { id: "fullset", label: "Fullset" },
  ];




  const materials = Array.from(new Set(items.map((i) => i.material || "Khác")));




  const filteredByCategory = items.filter(
    (i) =>
      !selectedCategory ||
      (i.category || "").toLowerCase() === selectedCategory.toLowerCase()
  );




  const filteredItems = filteredByCategory.filter((i) =>
    selectedMaterial ? (i.material || "").trim() === selectedMaterial : true
  );




  const totalPages = Math.ceil(filteredItems.length / itemsPerPage);
  const paginatedItems = filteredItems.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );




  /* =========================================================
     RENDER
     ========================================================= */
  return (
    <div className="h-screen w-screen flex overflow-hidden bg-gray-50">
      {/* LEFT PANEL */}
      <div className="fixed left-0 top-0 bottom-0 w-1/2 border-r border-gray-200 p-6 flex flex-col gap-6 overflow-y-auto bg-gray-50">
        <Card className="flex-1">
          <CardContent className="p-6 text-center">
            {loading && <div className="text-pink-600">Đang xử lý...</div>}
            {error && <div className="text-red-500">{error}</div>}
            {resultUrl ? (
              <img src={resultUrl} alt="result" className="max-w-full rounded-lg mx-auto" />
            ) : (
              <div className="text-gray-400">Chưa có kết quả thử đồ</div>
            )}
            <Button
              className="mt-4 bg-gradient-to-r from-pink-500 to-purple-500"
              onClick={handleTryOn}
              disabled={loading}
            >
              <Zap className="w-4 h-4 mr-2" /> Thử đồ ngay
            </Button>
          </CardContent>
        </Card>




        {/* Upload model + chọn sản phẩm */}
        <div className="grid grid-cols-2 gap-4">
          {/* Upload model */}
          <Card>
            <CardContent className="text-center">
              <h3 className="font-semibold mb-2">Ảnh dáng người</h3>
              <label
                htmlFor="upload-model"
                className="w-40 h-52 mx-auto flex items-center justify-center border-2 border-dashed border-pink-400 rounded-lg cursor-pointer hover:bg-pink-50"
              >
                {previewModel ? (
                  <img
                    src={previewModel}
                    alt="preview"
                    className="w-full h-full object-cover rounded-lg"
                  />
                ) : (
                  <Plus className="w-10 h-10 text-pink-400" />
                )}
              </label>
              <input
                id="upload-model"
                type="file"
                accept="image/*"
                className="hidden"
                onChange={(e) => handleModelSelect(e.target.files?.[0] || null)}
              />
            </CardContent>
          </Card>




          {/* Selected clothes */}
          <Card>
            <CardContent>
              <h3 className="font-semibold mb-3 text-center">Đã chọn</h3>
              <div className="grid grid-cols-3 gap-3">
                {["upper", "lower", "fullset"].map((type) => (
                  <div key={type} className="text-center">
                    <div className="text-xs mb-1 font-medium capitalize">
                      {type === "upper" ? "Áo" : type === "lower" ? "Quần" : "Fullset"}
                    </div>
                    {selected[type]?.imageBase64 ? (
                      <img
                        src={selected[type]!.imageBase64!}
                        alt={selected[type]!.name}
                        className="w-24 h-28 object-cover rounded-lg mx-auto"
                      />
                    ) : (
                      <div className="w-24 h-28 mx-auto flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg text-gray-400">
                        Chưa chọn
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>




      {/* RIGHT PANEL */}
      <div className="w-1/2 ml-[50%] p-6 flex flex-col gap-6 overflow-y-auto">


        {/* Balance */}
        <div className="flex items-center gap-2 text-gray-700 bg-pink-50 px-3 py-1 rounded w-fit mb-4">
          <Wallet className="w-5 h-5 text-pink-500" />
          <span className="font-semibold">
            {balance !== null ? balance.toLocaleString() : "..."}
          </span>
          <span className="text-xs text-gray-500 ml-1">đ</span>
        </div>




        <div className="flex gap-4 min-w-0">
          {/* CATEGORY FILTER DỌC */}
          <div className="w-40 min-w-[120px] sticky top-0 self-start">


            <Card>
              <CardContent>
                <h4 className="font-semibold mb-3">Danh mục</h4>
                <div className="flex flex-col gap-2">
                  {categories.map((c) => (
                    <button
                      key={c.id}
                      className={cn(
                        "text-left px-3 py-2 rounded truncate",
                        selectedCategory === c.id
                          ? "bg-pink-50 text-pink-600"
                          : "hover:bg-gray-100"
                      )}
                      onClick={() => {
                        setSelectedCategory(c.id);
                        setCurrentPage(1);
                      }}
                    >
                      {c.label}
                    </button>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>




          {/* PHẦN CÒN LẠI: LỌC NGANG MATERIAL + GRID */}
          <div className="flex-1 min-w-0 flex flex-col">
            {/* MATERIAL FILTER NGANG */}
            <div className="flex items-center gap-2 mb-4 overflow-x-auto sticky top-0 bg-white z-10 py-2">


              <span className="font-semibold text-gray-700 shrink-0">Chất liệu:</span>
              <button
                className={cn(
                  "px-3 py-2 rounded truncate",
                  !selectedMaterial ? "bg-pink-50 text-pink-600" : "hover:bg-gray-100"
                )}
                onClick={() => {
                  setSelectedMaterial(null);
                  setCurrentPage(1);
                }}
              >
                Tất cả
              </button>
              {materials.map((m) => (
                <button
                  key={m}
                  className={cn(
                    "px-3 py-2 rounded truncate",
                    selectedMaterial === m
                      ? "bg-pink-50 text-pink-600"
                      : "hover:bg-gray-100"
                  )}
                  onClick={() => {
                    setSelectedMaterial(m);
                    setCurrentPage(1);
                  }}
                >
                  {m}
                </button>
              ))}
            </div>




            {/* PRODUCT GRID */}
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 min-w-0 overflow-y-auto max-h-[70vh] pr-2">
              {paginatedItems.map((item) => (
                <Card
                  key={item.id}
                  className={cn(
                    "cursor-pointer hover:scale-105 transition group min-w-0",
                    Object.values(selected).some((s) => s?.id === item.id) ? "ring-2 ring-pink-500" : ""
                  )}
                  onClick={() => {
                    const cat = (item.category || "").trim().toLowerCase();
                    if (!["upper", "lower", "fullset"].includes(cat)) return;
                    setSelected((prev) => ({
                      ...prev,
                      [cat]: item,
                      ...(cat === "fullset"
                        ? { upper: null, lower: null }
                        : { fullset: null }),
                    }));
                  }}
                >
                  <CardContent className="flex flex-col gap-3 min-w-0">
                    <div
                      className="w-full h-40 rounded border bg-gray-100"
                      style={{
                        backgroundImage: item.imageBase64
                          ? `url(${item.imageBase64})`
                          : undefined,
                        backgroundSize: "cover",
                        backgroundPosition: "center",
                      }}
                    />
                    <div className="flex justify-between items-start min-w-0">
                      <div className="min-w-0">
                        <h4 className="font-medium text-gray-800 truncate">{item.name}</h4>
                        <p className="text-sm text-gray-500 truncate">
                          {item.price?.toLocaleString()}₫
                        </p>
                        <p className="text-xs text-gray-400 truncate">{item.material}</p>
                      </div>
                      {item.isVipOnly && <Crown className="w-4 h-4 text-yellow-500" />}
                    </div>
                    {item.affiliateUrl && (
                      <a
                        href={item.affiliateUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="mt-1 inline-flex items-center gap-2 text-pink-600 hover:text-pink-700 text-sm font-medium truncate"
                        onClick={(e) => e.stopPropagation()}
                      >
                        <ShoppingBag className="w-4 h-4" />
                        Mua ngay
                      </a>
                    )}
                  </CardContent>
                </Card>
              ))}
            </div>
            {/* PAGINATION */}
            <div className="flex justify-center items-center gap-4 mt-4">
              <Button
                variant="outline"
                size="sm"
                disabled={currentPage === 1}
                onClick={() => setCurrentPage((p) => p - 1)}
              >
                <ChevronLeft className="w-4 h-4" />
              </Button>
              <span className="text-sm text-gray-600">
                Trang {currentPage} / {totalPages || 1}
              </span>
              <Button
                variant="outline"
                size="sm"
                disabled={currentPage === totalPages}
                onClick={() => setCurrentPage((p) => p + 1)}
              >
                <ChevronRight className="w-4 h-4" />
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}











