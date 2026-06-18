/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./src/**/*.{html,ts,tsx,jsx,js}",
  ],
  theme: {
    extend: {
      colors: {
        // Primary palette - Professional blue (Lovable style)
        primary: {
          50: "hsl(214 100% 97%)",
          100: "hsl(214 100% 94%)",
          200: "hsl(214 98% 87%)",
          300: "hsl(214 97% 77%)",
          400: "hsl(214 95% 63%)",
          500: "hsl(214 91% 52%)",
          600: "hsl(217 91% 46%)",  // Primary action color
          700: "hsl(217 91% 40%)",
          800: "hsl(217 91% 30%)",
          900: "hsl(217 91% 20%)",
        },

        // Semantic colors
        success: "hsl(142 76% 36%)",
        "success-light": "hsl(142 76% 92%)",
        "success-dark": "hsl(142 76% 20%)",

        warning: "hsl(38 92% 50%)",
        "warning-light": "hsl(38 92% 93%)",
        "warning-dark": "hsl(38 92% 25%)",

        destructive: "hsl(0 84% 60%)",
        "destructive-light": "hsl(0 84% 94%)",
        "destructive-dark": "hsl(0 84% 35%)",

        info: "hsl(199 89% 48%)",
        "info-light": "hsl(199 89% 92%)",
        "info-dark": "hsl(199 89% 25%)",

        // Background colors - Clean white base
        background: "hsl(0 0% 100%)",
        "background-alt": "hsl(210 20% 98%)",
        "background-secondary": "hsl(210 20% 95%)",

        // Foreground text colors
        foreground: "hsl(222 47% 11%)",
        "foreground-muted": "hsl(222 47% 35%)",
        "foreground-secondary": "hsl(215 14% 50%)",

        // Sidebar specific
        sidebar: "hsl(222 47% 11%)",
        "sidebar-light": "hsl(210 20% 90%)",
        "sidebar-text": "hsl(210 20% 96%)",

        // Borders and dividers
        border: "hsl(214 32% 91%)",
        "border-muted": "hsl(214 32% 95%)",
        "border-light": "hsl(214 32% 97%)",

        // Status specific colors
        "status-draft": "hsl(214 32% 70%)",
        "status-submitted": "hsl(217 91% 50%)",
        "status-approval": "hsl(38 92% 50%)",
        "status-approved": "hsl(142 76% 50%)",
        "status-rejected": "hsl(0 84% 60%)",
        "status-closed": "hsl(222 47% 40%)",

        // Surface colors for cards
        "surface-light": "hsl(0 0% 100%)",
        "surface-dark": "hsl(222 47% 11%)",
      },

      spacing: {
        sidebar: "256px",
      },

      borderRadius: {
        DEFAULT: "0.5rem",
        lg: "0.75rem",
        xl: "1rem",
      },

      boxShadow: {
        DEFAULT: "0 1px 2px 0 rgba(0, 0, 0, 0.05)",
        sm: "0 1px 2px 0 rgba(0, 0, 0, 0.05)",
        md: "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -2px rgba(0, 0, 0, 0.1)",
        lg: "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -4px rgba(0, 0, 0, 0.1)",
        xl: "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1)",
      },

      animation: {
        "fade-in": "fadeIn 0.2s ease-in",
        "slide-down": "slideDown 0.3s ease-out",
        "slide-up": "slideUp 0.3s ease-out",
        "scale-in": "scaleIn 0.2s ease-in",
        "pulse-badge": "pulseBadge 2s cubic-bezier(0.4, 0, 0.6, 1) infinite",
      },

      keyframes: {
        fadeIn: {
          "0%": { opacity: "0" },
          "100%": { opacity: "1" },
        },
        slideDown: {
          "0%": { transform: "translateY(-8px)", opacity: "0" },
          "100%": { transform: "translateY(0)", opacity: "1" },
        },
        slideUp: {
          "0%": { transform: "translateY(8px)", opacity: "0" },
          "100%": { transform: "translateY(0)", opacity: "1" },
        },
        scaleIn: {
          "0%": { transform: "scale(0.95)", opacity: "0" },
          "100%": { transform: "scale(1)", opacity: "1" },
        },
        pulseBadge: {
          "0%, 100%": { opacity: "1" },
          "50%": { opacity: "0.6" },
        },
      },

      fontFamily: {
        sans: ["'Inter'", "system-ui", "-apple-system", "sans-serif"],
      },

      fontSize: {
        xs: ["0.75rem", { lineHeight: "1rem" }],
        sm: ["0.875rem", { lineHeight: "1.25rem" }],
        base: ["1rem", { lineHeight: "1.5rem" }],
        lg: ["1.125rem", { lineHeight: "1.75rem" }],
        xl: ["1.25rem", { lineHeight: "1.75rem" }],
        "2xl": ["1.5rem", { lineHeight: "2rem" }],
        "3xl": ["1.875rem", { lineHeight: "2.25rem" }],
      },
    },
  },

  plugins: [],
};
