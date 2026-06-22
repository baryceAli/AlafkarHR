(function () {
    const storageKey = "alafkar.erp.theme";
    const defaultTheme = {
        stylePreset: "classic",
        colorScheme: "blue",
        mode: "light"
    };

    function readTheme() {
        try {
            return { ...defaultTheme, ...JSON.parse(localStorage.getItem(storageKey) || "{}") };
        } catch {
            return defaultTheme;
        }
    }

    function applyTheme(theme) {
        const nextTheme = { ...defaultTheme, ...theme };
        document.documentElement.dataset.uiStyle = nextTheme.stylePreset;
        document.documentElement.dataset.colorScheme = nextTheme.colorScheme;
        document.documentElement.dataset.themeMode = nextTheme.mode;
        document.documentElement.style.colorScheme = nextTheme.mode;
        return nextTheme;
    }

    window.alafkarTheme = {
        get: function () {
            return applyTheme(readTheme());
        },
        set: function (colorScheme, mode, stylePreset) {
            const current = readTheme();
            const nextTheme = applyTheme({ ...current, colorScheme, mode, stylePreset: stylePreset || current.stylePreset });
            localStorage.setItem(storageKey, JSON.stringify(nextTheme));
            window.dispatchEvent(new CustomEvent("alafkar-theme-changed", { detail: nextTheme }));
            return nextTheme;
        },
        setMode: function (mode) {
            const current = readTheme();
            return this.set(current.colorScheme, mode, current.stylePreset);
        },
        setColorScheme: function (colorScheme) {
            const current = readTheme();
            return this.set(colorScheme, current.mode, current.stylePreset);
        },
        setStylePreset: function (stylePreset) {
            const current = readTheme();
            return this.set(current.colorScheme, current.mode, stylePreset);
        },
        bindThemeChanged: function (dotNetRef) {
            const handler = function (event) {
                const theme = event.detail || readTheme();
                dotNetRef.invokeMethodAsync("OnThemeChanged", theme.stylePreset || defaultTheme.stylePreset);
            };

            window.alafkarTheme.unbindThemeChanged();
            window.alafkarTheme._themeChangedHandler = handler;
            window.addEventListener("alafkar-theme-changed", handler);
        },
        unbindThemeChanged: function () {
            if (!window.alafkarTheme._themeChangedHandler) {
                return;
            }

            window.removeEventListener("alafkar-theme-changed", window.alafkarTheme._themeChangedHandler);
            window.alafkarTheme._themeChangedHandler = null;
        }
    };

    applyTheme(readTheme());
})();

(function () {
    let shortcutHandler = null;

    window.alafkarNavigation = {
        bindShortcuts: function (dotNetRef) {
            if (shortcutHandler) {
                document.removeEventListener("keydown", shortcutHandler);
            }

            shortcutHandler = function (event) {
                const key = (event.key || "").toLowerCase();
                if ((event.ctrlKey || event.metaKey) && key === "k") {
                    event.preventDefault();
                    dotNetRef.invokeMethodAsync("OpenCommandPaletteFromShortcut");
                }
            };

            document.addEventListener("keydown", shortcutHandler);
        },
        unbindShortcuts: function () {
            if (!shortcutHandler) {
                return;
            }

            document.removeEventListener("keydown", shortcutHandler);
            shortcutHandler = null;
        }
    };
})();
