const editors = new Map();
export function attach(id, session, dotnet) {
    detach(id);
    const frame = document.getElementById(id);
    const origin = window.location.origin;
    const state = { dirty: false };
    const receive = event => {
        if (event.origin !== origin || event.source !== frame?.contentWindow || event.data?.session !== session) return;
        if (event.data.type === "website-ready") dotnet.invokeMethodAsync("PreviewReady");
        if (event.data.type === "website-select" && Array.isArray(event.data.fields)) dotnet.invokeMethodAsync("SelectPreviewField", event.data.fields, event.data.itemId);
        if (event.data.type === "website-preview-error") dotnet.invokeMethodAsync("PreviewError");
        if (event.data.type === "website-navigate" && typeof event.data.target === "string") dotnet.invokeMethodAsync("PreviewNavigate", event.data.target);
    };
    const beforeUnload = event => { if (state.dirty) { event.preventDefault(); event.returnValue = ""; } };
    window.addEventListener("message", receive);
    window.addEventListener("beforeunload", beforeUnload);
    editors.set(id, { frame, session, state, receive, beforeUnload });
}
export function send(id, content, dirty, editable) {
    const editor = editors.get(id); if (!editor) return;
    editor.state.dirty = dirty;
    editor.frame.contentWindow?.postMessage({ type: "website-snapshot", session: editor.session, content, editable }, window.location.origin);
}
export function detach(id) {
    const editor = editors.get(id); if (!editor) return;
    window.removeEventListener("message", editor.receive);
    window.removeEventListener("beforeunload", editor.beforeUnload);
    editors.delete(id);
}
