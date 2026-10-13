// Optional enhancement scoped to the PublicWebsite layout. SSR content is always visible.
export function connect(root) {
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const fine = matchMedia('(hover: hover) and (pointer: fine)');
    const mobile = matchMedia('(hover: none), (pointer: coarse)');
    const key = 'alafkar-public-website-motion-paused';
    const button = root.querySelector('[data-pw-motion-toggle]');
    const mobileMenu = root.querySelector('.pw-mobile-nav');
    const states = new Map();
    const imageStates = new Map();
    let paused = false;
    let disposed = false;
    try { paused = sessionStorage.getItem(key) === 'true'; } catch { /* Storage is optional. */ }
    const quiet = () => paused || reduced.matches;
    const reveal = element => {
        element.classList.remove('pw-motion-pending');
        element.classList.add('pw-motion-visible');
    };
    // Keep observing the hero so floating accents stop when it leaves the viewport.
    const observer = typeof IntersectionObserver === 'function' ? new IntersectionObserver(entries => {
        for (const entry of entries) {
            const imageState = imageStates.get(entry.target);
            if (imageState) {
                imageState.imageInView = entry.isIntersecting;
                imageState.enterImage();
                continue;
            }
            const state = states.get(entry.target);
            if (!state) continue;
            if (entry.isIntersecting) reveal(entry.target);
            if (state.hero) entry.target.classList.toggle('pw-motion-in-view', entry.isIntersecting);
            else if (entry.isIntersecting) observer.unobserve(entry.target);
        }
    }, { threshold: 0 }) : null;

    function register(element) {
        const hero = element.dataset.pwMotion === 'hero';
        const card = element.dataset.pwMotion === 'card';
        const image = hero || element.dataset.pwMotion === 'image' || element.classList.contains('pw-project-card')
            ? element.querySelector('img') : null;
        const state = { hero, imagePlayed: false, imageInView: false, frame: 0, x: 0, y: 0 };
        const finishImage = () => image?.classList.remove('pw-mobile-image-enter');
        const enterImage = () => {
            if (disposed || !observer || !image || state.imagePlayed || !state.imageInView || !image.complete || image.naturalWidth === 0) return;
            if (document.hidden) return;
            state.imagePlayed = true;
            if (mobile.matches && !quiet()) image.classList.add('pw-mobile-image-enter');
            observer.unobserve(image);
        };
        state.enterImage = enterImage;
        state.stopImage = () => {
            if (state.imageInView && image?.complete && image.naturalWidth > 0) {
                state.imagePlayed = true;
                observer?.unobserve(image);
            }
            finishImage();
        };
        image?.addEventListener('load', enterImage);
        image?.addEventListener('animationend', finishImage);
        const reset = () => {
            cancelAnimationFrame(state.frame);
            state.frame = 0;
            element.classList.remove('pw-motion-hover');
            for (const property of ['--pw-tilt-x', '--pw-tilt-y', '--pw-pointer-x', '--pw-pointer-y']) element.style.removeProperty(property);
        };
        const move = event => {
            if (quiet() || !fine.matches || event.pointerType === 'touch' || document.hidden) return;
            const bounds = element.getBoundingClientRect();
            if (!bounds.width || !bounds.height) return;
            state.x = Math.max(-1, Math.min(1, (event.clientX - bounds.left) / bounds.width * 2 - 1));
            state.y = Math.max(-1, Math.min(1, (event.clientY - bounds.top) / bounds.height * 2 - 1));
            if (!state.frame) state.frame = requestAnimationFrame(() => {
                state.frame = 0;
                element.classList.add('pw-motion-hover');
                element.style.setProperty('--pw-tilt-x', `${-state.y * (hero ? 7 : 3)}deg`);
                element.style.setProperty('--pw-tilt-y', `${state.x * (hero ? 9 : 3)}deg`);
                element.style.setProperty('--pw-pointer-x', `${(state.x + 1) * 50}%`);
                element.style.setProperty('--pw-pointer-y', `${(state.y + 1) * 50}%`);
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
            element.classList.remove('pw-motion-pending', 'pw-motion-visible', 'pw-motion-in-view');
            element.style.removeProperty('--pw-reveal-delay');
            image?.removeEventListener('load', enterImage);
            image?.removeEventListener('animationend', finishImage);
            if (image) {
                observer?.unobserve(image);
                imageStates.delete(image);
            }
            finishImage();
        };
        states.set(element, state);
        if (image) {
            imageStates.set(image, state);
            observer?.observe(image);
        }
        const siblings = [...element.parentElement.children].filter(child => child.hasAttribute('data-pw-motion'));
        element.style.setProperty('--pw-reveal-delay', `${Math.min(siblings.indexOf(element) * 90, 360)}ms`);
        // Never hide initially visible SSR content or a keyboard-focused element.
        if (!quiet() && observer && element.getBoundingClientRect().top >= innerHeight && !element.contains(document.activeElement)) {
            element.classList.add('pw-motion-pending');
        } else reveal(element);
        if (hero || element.classList.contains('pw-motion-pending')) observer?.observe(element);
    }
    function refresh() {
        if (disposed) return;
        for (const [element, state] of states) if (!root.contains(element)) { state.cleanup(); states.delete(element); }
        for (const element of root.querySelectorAll('[data-pw-motion]')) if (!states.has(element)) register(element);
    }
    function update() {
        root.dataset.pwMotionState = quiet() ? 'paused' : 'active';
        root.classList.toggle('pw-motion-tab-hidden', document.hidden);
        for (const [element, state] of states) {
            state.reset();
            if (!mobile.matches) state.stopImage();
            if (quiet()) {
                reveal(element);
                state.stopImage();
            } else state.enterImage();
        }
        button.hidden = false;
        button.disabled = reduced.matches;
        button.textContent = quiet() ? 'تشغيل الحركة' : 'إيقاف الحركة';
        button.setAttribute('aria-pressed', String(!quiet()));
        button.title = reduced.matches ? 'الحركة متوقفة حسب تفضيلات تقليل الحركة في جهازك' : 'التحكم في حركة الموقع';
    }
    const toggle = () => {
        paused = !paused;
        try { sessionStorage.setItem(key, String(paused)); } catch { /* In-memory choice still works. */ }
        update();
    };
    // Close immediately, including links to an anchor on the current page.
    const closeMobileMenu = event => {
        if (event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;
        const link = event.target.closest?.('a[href]');
        if (link && mobileMenu?.contains(link)) mobileMenu.open = false;
    };
    const mutations = typeof MutationObserver === 'function' ? new MutationObserver(refresh) : null;
    // Child changes cover Blazor project filtering, without observing our style writes.
    refresh();
    update();
    mutations?.observe(root.querySelector('main'), { childList: true, subtree: true });
    button.addEventListener('click', toggle);
    mobileMenu?.addEventListener('click', closeMobileMenu);
    reduced.addEventListener('change', update);
    fine.addEventListener('change', update);
    mobile.addEventListener('change', update);
    document.addEventListener('visibilitychange', update);
    return {
        disconnect() {
            disposed = true;
            mutations?.disconnect();
            observer?.disconnect();
            for (const state of states.values()) state.cleanup();
            states.clear();
            imageStates.clear();
            button.removeEventListener('click', toggle);
            mobileMenu?.removeEventListener('click', closeMobileMenu);
            reduced.removeEventListener('change', update);
            fine.removeEventListener('change', update);
            mobile.removeEventListener('change', update);
            document.removeEventListener('visibilitychange', update);
            button.hidden = true;
            root.removeAttribute('data-pw-motion-state');
            root.classList.remove('pw-motion-tab-hidden');
        }
    };
}
