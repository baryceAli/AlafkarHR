// All state belongs to one PublicWebsite layout; no global page listeners survive disposal.
export function connect(root) {
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const pointer = matchMedia('(hover: hover) and (pointer: fine)');
    const states = new Map();
    const editing = () => document.documentElement.classList.contains('website-edit-preview');
    const quiet = () => reduced.matches || editing();
    const reveal = element => {
        element.classList.add('is-image-visible');
        observer?.unobserve(element);
    };
    const observer = typeof IntersectionObserver === 'function'
        ? new IntersectionObserver(entries => {
            for (const entry of entries) if (entry.isIntersecting) reveal(entry.target);
        }, { threshold: 0.08 }) : null;

    function register(element) {
        const image = element.querySelector('img');
        if (!image) return;
        const state = { image, source: image.getAttribute('src'), frame: 0, x: 0, y: 0 };
        const reset = () => {
            cancelAnimationFrame(state.frame);
            state.frame = 0;
            element.style.removeProperty('--image-pointer-x');
            element.style.removeProperty('--image-pointer-y');
        };
        const move = event => {
            if (quiet() || !pointer.matches || event.pointerType === 'touch') return;
            const bounds = element.getBoundingClientRect();
            if (!bounds.width || !bounds.height) return;
            state.x = Math.max(-1, Math.min(1, (event.clientX - bounds.left) / bounds.width * 2 - 1));
            state.y = Math.max(-1, Math.min(1, (event.clientY - bounds.top) / bounds.height * 2 - 1));
            if (!state.frame) state.frame = requestAnimationFrame(() => {
                state.frame = 0;
                element.style.setProperty('--image-pointer-x', state.x);
                element.style.setProperty('--image-pointer-y', state.y);
            });
        };
        const loaded = () => element.classList.toggle('is-image-ready', image.complete && image.naturalWidth > 0);
        image.addEventListener('load', loaded);
        image.addEventListener('error', loaded);
        if (element.dataset.imageEffect === 'hero') {
            element.addEventListener('pointermove', move);
            element.addEventListener('pointerleave', reset);
            element.addEventListener('pointercancel', reset);
        }
        state.cleanup = () => {
            reset();
            observer?.unobserve(element);
            image.removeEventListener('load', loaded);
            image.removeEventListener('error', loaded);
            element.removeEventListener('pointermove', move);
            element.removeEventListener('pointerleave', reset);
            element.removeEventListener('pointercancel', reset);
            element.classList.remove('is-image-pending', 'is-image-visible', 'is-image-ready');
        };
        state.reset = reset;
        state.loaded = loaded;
        states.set(element, state);
        loaded();
        // Enhance only offscreen elements. Above-the-fold SSR images never flash hidden.
        if (!quiet() && observer && element.getBoundingClientRect().top >= innerHeight) {
            element.classList.add('is-image-pending');
            observer.observe(element);
        } else reveal(element);
    }

    function refresh() {
        for (const [element, state] of states) {
            if (!root.contains(element) || element.querySelector('img') !== state.image) {
                state.cleanup();
                states.delete(element);
            } else {
                if (state.source !== state.image.getAttribute('src')) {
                    state.source = state.image.getAttribute('src');
                    element.classList.remove('is-image-ready');
                    state.loaded();
                }
                if (quiet()) {
                    state.reset();
                    reveal(element);
                } else if (!pointer.matches) state.reset();
            }
        }
        root.querySelectorAll('[data-image-effect]:not([data-image-effect="still"])').forEach(element => {
            if (!states.has(element)) register(element);
        });
    }
    const mutations = new MutationObserver(refresh);
    mutations.observe(root, { childList: true, subtree: true, attributes: true, attributeFilter: ['src', 'hidden'] });
    const preview = new MutationObserver(refresh);
    preview.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
    reduced.addEventListener('change', refresh);
    pointer.addEventListener('change', refresh);
    refresh();
    return {
        disconnect() {
            mutations.disconnect();
            preview.disconnect();
            observer?.disconnect();
            reduced.removeEventListener('change', refresh);
            pointer.removeEventListener('change', refresh);
            for (const state of states.values()) state.cleanup();
            states.clear();
        }
    };
}
