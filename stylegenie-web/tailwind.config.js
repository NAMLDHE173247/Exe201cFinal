/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        "fashion-pink": {
          50: "#fff0f6",
          100: "#ffdeeb",
          200: "#fcc2d7",
          300: "#faa2c1",
          400: "#f783ac",
          500: "#f06595",
          600: "#e64980",
          700: "#d6336c",
          800: "#c2255c",
          900: "#a61e4d",
        },
        "fashion-purple": {
          100: "#ede9fe",
          200: "#ddd6fe",
          500: "#8b5cf6",
          600: "#7c3aed",
        },
      },
    },
  },
  plugins: [],
}
