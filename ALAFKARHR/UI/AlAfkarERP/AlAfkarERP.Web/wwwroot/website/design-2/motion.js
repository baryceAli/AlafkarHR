// Optional enhancement scoped to one Design 2 layout. SSR content is always visible.
export function connect(root) {
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const fine = matchMedia('(hover: hover) and (pointer: fine)');
    const key = 'alafkar-design-2-motion-paused';
    const button = root.querySelector('[data-d2-motion-toggle]');
    const states = new Map();
    let paused = false;
    let disposed = false;
    try { paused = sessionStorage.getItem(key) === 'true'; } catch { /* Storage is optional. */ }
    const quiet = () => paused || reduced.matches;
    const reveal = element => {
        element.classList.remove('d2-motion-pending');
        element.classList.add('d2-motion-visible');
    };
    // Keep observing the hero so floating accents stop when it leaves the viewport.
    const observer = typeof IntersectionObserver === 'function' ? new IntersectionObserver(entries => {
        for (const entry of entries) {
            const state = states.get(entry.target);
            if (!state) continue;
            if (entry.isIntersecting) reveal(entry.target);
            if (state.hero) entry.target.classList.toggle('d2-motion-in-view', entry.isIntersecting);
            else if (entry.isIntersecting) observer.unobserve(entry.target);
        }
    }, { threshold: 0 }) : null;

    function register(element) {
        const hero = element.dataset.d2Motion === 'hero';
        const card = element.dataset.d2Motion === 'card';
        const state = { hero, frame: 0, x: 0, y: 0 };
        const reset = () => {
            cancelAnimationFrame(state.frame);
            state.frame = 0;
            element.classList.remove('d2-motion-hover');
            for (const property of ['--d2-tilt-x', '--d2-tilt-y', '--d2-pointer-x', '--d2-pointer-y']) element.style.removeProperty(property);
        };
        const move = event => {
            if (quiet() || !fine.matches || event.pointerType === 'touch' || document.hidden) return;
            const bounds = element.getBoundingClientRect();
            if (!bounds.width || !bounds.height) return;
            state.x = Math.max(-1, Math.min(1, (event.clientX - bounds.left) / bounds.width * 2 - 1));
            state.y = Math.max(-1, Math.min(1, (event.clientY - bounds.top) / bounds.height * 2 - 1));
            if (!state.frame) state.frame = requestAnimationFrame(() => {
                state.frame = 0;
                element.classList.add('d2-motion-hover');
                element.style.setProperty('--d2-tilt-x', `${-state.y * (hero ? 7 : 3)}deg`);
                element.style.setProperty('--d2-tilt-y', `${state.x * (hero ? 9 : 3)}deg`);
                element.style.setProperty('--d2-pointer-x', `${(state.x + 1) * 50}%`);
                element.style.setProperty('--d2-pointer-y', `${(state.y + 1) * 50}%`);
            });
        };
        const focus = () => reveal(element);
        if (hero || card) {
            element.addEventListener('pointermove', move);
            element.addEventListener('pointerleave', reset);
            element.addEventListener('pointercancel', reset);
        }
        element.addEventListener('focusin', focus);
        state.reset = reset;
        state.cleanup = () => {
            reset();
            observer?.unobserve(element);
            element.removeEventListener('pointermove', move);
            element.removeEventListener('pointerleave', reset);
            element.removeEventListener('pointercancel', reset);
            element.removeEventListener('focusin', focus);
            element.classList.remove('d2-motion-pending', 'd2-motion-visible', 'd2-motion-in-view');
            element.style.removeProperty('--d2-reveal-delay');
        };
        states.set(element, state);
        const siblings = [...element.parentElement.children].filter(child => child.hasAttribute('data-d2-motion'));
        element.style.setProperty('--d2-reveal-delay', `${Math.min(siblings.indexOf(element) * 90, 360)}ms`);
        // Never hide initially visible SSR content or a keyboard-focused element.
        if (!quiet() && observer && element.getBoundingClientRect().top >= innerHeight && !element.contains(document.activeElement)) {
            element.classList.add('d2-motion-pending');
        } else reveal(element);
        if (hero || element.classList.contains('d2-motion-pending')) observer?.observe(element);
    }
    function refresh() {
        if (disposed) return;
        for (const [element, state] of states) if (!root.contains(element)) { state.cleanup(); states.delete(element); }
        for (const element of root.querySelectorAll('[data-d2-motion]')) if (!states.has(element)) register(element);
    }
    function update() {
        root.dataset.d2MotionState = quiet() ? 'paused' : 'active';
        root.classList.toggle('d2-motion-tab-hidden', document.hidden);
        for (const [element, state] of states) {
            state.reset();
            if (quiet()) reveal(element);
        }
        button.hidden = false;
        button.disabled = reduced.matches;
        button.textContent = quiet() ? 'تشغيل الحركة' : 'إيقاف الحركة';
        button.setAttribute('aria-pressed', String(!quiet()));
        button.title = reduced.matches ? 'الحركة متوقفة حسب تفضيلات تقليل الحركة في جهازك' : 'التحكم في حركة التصميم الثاني';
    }
    const toggle = () => {
        paused = !paused;
        try { sessionStorage.setItem(key, String(paused)); } catch { /* In-memory choice still works. */ }
        update();
    };
    const mutations = typeof MutationObserver === 'function' ? new MutationObserver(refresh) : null;
    // Child changes cover Blazor project filtering, without observing our style writes.
    refresh();
    update();
    mutations?.observe(root.querySelector('main'), { childList: true, subtree: true });
    button.addEventListener('click', toggle);
    reduced.addEventListener('change', update);
    fine.addEventListener('change', update);
    document.addEventListener('visibilitychange', update);
    return {
        disconnect() {
            disposed = true;
            mutations?.disconnect();
            observer?.disconnect();
            for (const state of states.values()) state.cleanup();
            states.clear();
            button.removeEventListener('click', toggle);
            reduced.removeEventListener('change', update);
            fine.removeEventListener('change', update);
            document.removeEventListener('visibilitychange', update);
            button.hidden = true;
            root.removeAttribute('data-d2-motion-state');
            root.classList.remove('d2-motion-tab-hidden');
        }
    };
}
