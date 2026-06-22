(function () {
    const storageKey = "alafkar.erp.theme";
    const defaultTheme = {
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
        document.documentElement.dataset.colorScheme = nextTheme.colorScheme;
        document.documentElement.dataset.themeMode = nextTheme.mode;
        document.documentElement.style.colorScheme = nextTheme.mode;
        return nextTheme;
    }

    window.alafkarTheme = {
        get: function () {
            return applyTheme(readTheme());
        },
        set: function (colorScheme, mode) {
            const nextTheme = applyTheme({ colorScheme, mode });
            localStorage.setItem(storageKey, JSON.stringify(nextTheme));
            return nextTheme;
        },
        setMode: function (mode) {
            const current = readTheme();
            return this.set(current.colorScheme, mode);
        },
        setColorScheme: function (colorScheme) {
            const current = readTheme();
            return this.set(colorScheme, current.mode);
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
