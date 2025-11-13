/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html", 
    "./src/**/*.{js,jsx,ts,tsx}",
    "./src/**/*.jsx",
    "./src/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        'custom-blue': '#E0F2F7',
        'custom-dark-blue': '#1A237E',
      }
    },
  },
  plugins: [],
};
