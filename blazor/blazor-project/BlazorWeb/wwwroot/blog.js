(function () {
    const sThemeKey = "dev-archive-theme";

    function applyTheme(sTheme) {
        if (sTheme === "light") {
            document.documentElement.setAttribute("data-theme", "light");
            return;
        }

        document.documentElement.removeAttribute("data-theme");
    }

    function getCodeText(buttonElement) {
        const codeShellElement = buttonElement.closest(".code-shell");

        if (!codeShellElement) {
            return "";
        }

        const codeElement = codeShellElement.querySelector("code");
        return codeElement ? codeElement.innerText : "";
    }

    async function copyCode(buttonElement) {
        const sCodeText = getCodeText(buttonElement);

        if (!sCodeText) {
            return;
        }

        await navigator.clipboard.writeText(sCodeText);
        buttonElement.textContent = "Copied";
        window.setTimeout(function () {
            buttonElement.textContent = "Copy";
        }, 1400);
    }

    function getZoomOverlay() {
        let zoomOverlayElement = document.querySelector(".image-zoom-overlay");

        if (zoomOverlayElement) {
            return zoomOverlayElement;
        }

        zoomOverlayElement = document.createElement("div");
        zoomOverlayElement.className = "image-zoom-overlay";
        zoomOverlayElement.innerHTML = "<img alt=\"\">";
        document.body.appendChild(zoomOverlayElement);

        zoomOverlayElement.addEventListener("click", function () {
            zoomOverlayElement.classList.remove("is-open");
        });

        return zoomOverlayElement;
    }

    document.addEventListener("click", function (mouseEvent) {
        const copyButtonElement = mouseEvent.target.closest("[data-copy-code]");

        if (copyButtonElement) {
            copyCode(copyButtonElement);
            return;
        }

        const themeButtonElement = mouseEvent.target.closest("[data-theme-toggle]");

        if (themeButtonElement) {
            const bLightTheme = document.documentElement.getAttribute("data-theme") === "light";
            const sNextTheme = bLightTheme ? "dark" : "light";
            window.localStorage.setItem(sThemeKey, sNextTheme);
            applyTheme(sNextTheme);
            return;
        }

        const zoomButtonElement = mouseEvent.target.closest("[data-zoom-image]");

        if (zoomButtonElement) {
            const zoomOverlayElement = getZoomOverlay();
            const imageElement = zoomOverlayElement.querySelector("img");
            imageElement.src = zoomButtonElement.getAttribute("data-zoom-image");
            imageElement.alt = zoomButtonElement.querySelector("img")?.alt || "";
            zoomOverlayElement.classList.add("is-open");
        }
    });

    applyTheme(window.localStorage.getItem(sThemeKey));
})();
