import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { Button } from "./button";
import { Badge } from "./badge";
import { Menu, X, Sparkles, Wand2, Palette, Crown } from "lucide-react";
import { cn } from "../../lib/utils";

export function Navigation() {
  const [isOpen, setIsOpen] = useState(false);
  const location = useLocation();

  const navItems = [
    { href: "/", label: "Trang chủ", icon: Sparkles },
    { href: "/virtual-tryon", label: "Thử đồ ảo", icon: Wand2 },
    { href: "/outfit-mixer", label: "Phối đồ", icon: Palette },
    { href: "/pricing", label: "Gói VIP", icon: Crown },
  ];

  return (
    <nav className="fixed top-0 w-full bg-white/80 backdrop-blur-md z-50 border-b border-gray-100">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center space-x-3 group">
            <div className="w-10 h-10 bg-gradient-to-r from-fashion-pink-500 to-fashion-purple-500 rounded-xl flex items-center justify-center group-hover:scale-105 transition-transform">
              <Sparkles className="w-6 h-6 text-white" />
            </div>
            <div>
              <span className="text-xl font-bold bg-gradient-to-r from-fashion-pink-600 to-fashion-purple-600 bg-clip-text text-transparent">
                StyleGenie
              </span>
              <Badge variant="secondary" className="ml-2 text-xs">
                AI
              </Badge>
            </div>
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden md:flex items-center space-x-8">
            {navItems.map((item) => {
              const Icon = item.icon;
              const isActive = location.pathname === item.href;
              return (
                <Link
                  key={item.href}
                  to={item.href}
                  className={cn(
                    "flex items-center space-x-2 px-3 py-2 rounded-lg text-sm font-medium transition-colors",
                    isActive
                      ? "bg-fashion-pink-50 text-fashion-pink-600"
                      : "text-gray-600 hover:text-fashion-pink-600 hover:bg-fashion-pink-50",
                  )}
                >
                  <Icon className="w-4 h-4" />
                  <span>{item.label}</span>
                </Link>
              );
            })}
          </div>

          {/* Action Buttons */}
          <div className="hidden md:flex items-center space-x-4">
            <Button variant="ghost" size="sm">
              Đăng nhập
            </Button>
            <Button
              className="bg-gradient-to-r from-fashion-pink-500 to-fashion-purple-500 hover:from-fashion-pink-600 hover:to-fashion-purple-600"
              size="sm"
            >
              Dùng thử miễn phí
            </Button>
          </div>

          {/* Mobile menu button */}
          <div className="md:hidden">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setIsOpen(!isOpen)}
            >
              {isOpen ? (
                <X className="w-6 h-6" />
              ) : (
                <Menu className="w-6 h-6" />
              )}
            </Button>
          </div>
        </div>

        {/* Mobile Navigation */}
        {isOpen && (
          <div className="md:hidden py-4 border-t border-gray-100">
            <div className="flex flex-col space-y-2">
              {navItems.map((item) => {
                const Icon = item.icon;
                const isActive = location.pathname === item.href;
                return (
                  <Link
                    key={item.href}
                    to={item.href}
                    className={cn(
                      "flex items-center space-x-3 px-4 py-3 rounded-lg text-sm font-medium transition-colors",
                      isActive
                        ? "bg-fashion-pink-50 text-fashion-pink-600"
                        : "text-gray-600 hover:text-fashion-pink-600 hover:bg-fashion-pink-50",
                    )}
                    onClick={() => setIsOpen(false)}
                  >
                    <Icon className="w-5 h-5" />
                    <span>{item.label}</span>
                  </Link>
                );
              })}
              <div className="pt-4 border-t border-gray-100 space-y-2">
                <Button
                  variant="ghost"
                  className="w-full justify-start"
                  size="sm"
                >
                  Đăng nhập
                </Button>
                <Button
                  className="w-full bg-gradient-to-r from-fashion-pink-500 to-fashion-purple-500 hover:from-fashion-pink-600 hover:to-fashion-purple-600"
                  size="sm"
                >
                  Dùng thử miễn phí
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>
    </nav>
  );
}
