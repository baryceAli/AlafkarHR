let cleanup;
export function connect(session, dotnet) {
    disconnect();
    if (window.parent === window) return;
    const origin = window.location.origin;
    const send = (type, data = {}) => window.parent.postMessage({ type, session, ...data }, origin);
    let initialized = false, editable = true;
    let pending, transferring = false;
    const transfer = async () => {
        if (transferring) return;
        transferring = true;
        try {
            while (pending) {
                const content = JSON.stringify(pending); pending = null;
                // Stay below Blazor's default incoming SignalR message limit, including UTF-8 text.
                for (let offset = 0; offset < content.length; offset += 4096) {
                    await dotnet.invokeMethodAsync("ReceivePreviewChunk", content.slice(offset, offset + 4096), offset === 0, offset + 4096 >= content.length, editable);
                }
                initialized = true;
            }
        } catch {
            send("website-preview-error");
        } finally { transferring = false; }
    };
    const receive = async event => {
        if (event.origin !== origin || event.source !== window.parent || event.data?.session !== session || event.data?.type !== "website-snapshot") return;
        if (!event.data.content || event.data.content.schemaVersion !== 1) return;
        editable = event.data.editable !== false;
        document.documentElement.classList.toggle("website-edit-preview", editable);
        pending = event.data.content;
        await transfer();
    };
    const click = event => {
        if (!editable) {
            const anchor = event.target.closest("a[href]");
            if (anchor) {
                const url = new URL(anchor.href, window.location.href);
                if (url.origin === origin && !url.pathname.startsWith("/publicwebsite-preview-media/") && !url.pathname.startsWith("/website/")) {
                    event.preventDefault(); send("website-navigate", { target: url.href });
                } else if (url.pathname.startsWith("/publicwebsite-preview-media/")) {
                    event.preventDefault(); window.open(url.href, "_blank", "noopener,noreferrer");
                }
            }
            return;
        }
        const field = event.target.closest("[data-pw-fields]");
        if (field) {
            event.preventDefault(); event.stopPropagation();
            const fields = [];
            for (let node = field; node && !node.matches("[data-pw-section],main"); node = node.parentElement) {
                if (node.dataset.pwFields) fields.push(...node.dataset.pwFields.split(","));
            }
            send("website-select", { fields: [...new Set(fields)], itemId: field.closest("[data-pw-item]")?.dataset.pwItem || null });
        } else if (event.target.closest("a,button,input,select,textarea,form")) {
            event.preventDefault(); event.stopPropagation();
        }
    };
    const decorate = () => { if (!editable) return; document.querySelectorAll("[data-pw-fields]").forEach(node => {
        if (!node.matches("a,button,input,select,textarea,option") && !node.hasAttribute("tabindex")) node.setAttribute("tabindex", "0");
    }); };
    const keydown = event => { if ((event.key === "Enter" || event.key === " ") && event.target.matches("[data-pw-fields]")) click(event); };
    const observer = new MutationObserver(decorate);
    observer.observe(document.body, { childList: true, subtree: true });
    decorate();
    window.addEventListener("message", receive);
    document.addEventListener("click", click, true);
    document.addEventListener("keydown", keydown, true);
    document.documentElement.classList.add("website-edit-preview");
    const interval = setInterval(() => { if (!initialized) send("website-ready"); }, 500);
    send("website-ready");
    cleanup = () => {
        clearInterval(interval);
        window.removeEventListener("message", receive);
        document.removeEventListener("click", click, true);
        document.removeEventListener("keydown", keydown, true);
        observer.disconnect();
        document.documentElement.classList.remove("website-edit-preview");
    };
}
export function disconnect() { cleanup?.(); cleanup = null; }
