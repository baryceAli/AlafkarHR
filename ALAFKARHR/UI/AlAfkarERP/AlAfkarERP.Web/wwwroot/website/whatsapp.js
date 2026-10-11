export function openWhatsApp(url) {
    const target = new URL(url);
    if (target.protocol === "https:" && target.hostname === "wa.me" && target.pathname === "/966580007040") {
        window.open(target.href, "_blank", "noopener,noreferrer");
    }
}
