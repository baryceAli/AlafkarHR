// Exercise real module logic with simulated DOM/media/observers; no browser or app.
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const source = fs.readFileSync(path.join(__dirname, '../../wwwroot/website/motion.js'), 'utf8');
class Element {
    listeners = new Map();
    dataset = {};
    properties = new Map();
    classes = new Set();
    classList = {
        add: (...values) => values.forEach(value => this.classes.add(value)),
        remove: (...values) => values.forEach(value => this.classes.delete(value)),
        contains: value => this.classes.has(value),
        toggle: (value, enabled) => enabled ? this.classes.add(value) : this.classes.delete(value)
    };
    style = { setProperty: (key, value) => this.properties.set(key, value),
        removeProperty: key => this.properties.delete(key) };
    addEventListener(name, handler) {
        const handlers = this.listeners.get(name) ?? new Set();
        handlers.add(handler); this.listeners.set(name, handlers);
    }
    removeEventListener(name, handler) { this.listeners.get(name)?.delete(handler); }
    emit(name, event = {}) { for (const handler of [...(this.listeners.get(name) ?? [])]) handler(event); }
    contains() { return false; }
    querySelector(selector) { return selector === 'img' ? this.image : null; }
    hasAttribute(name) { return name === 'data-pw-motion'; }
    setAttribute() {}
    removeAttribute(name) { if (name === 'data-pw-motion-state') delete this.dataset.pwMotionState; }
    getBoundingClientRect() { return { top: this.top ?? 0, left: 0, width: 200, height: 200 }; }
}
function frame(kind = 'image', { loaded = true, top = 0, image = true } = {}) {
    const item = new Element();
    item.dataset.pwMotion = kind;
    item.top = top;
    if (kind === 'card') item.classes.add('pw-project-card');
    if (image) item.image = Object.assign(new Element(), { complete: loaded, naturalWidth: loaded ? 512 : 0 });
    return item;
}
function setup(items, { mobile = true, reduced = false, paused = false, observers = true } = {}) {
    const root = new Element(), button = new Element(), document = new Element();
    const media = {
        '(prefers-reduced-motion: reduce)': Object.assign(new Element(), { matches: reduced }),
        '(hover: hover) and (pointer: fine)': Object.assign(new Element(), { matches: !mobile }),
        '(hover: none), (pointer: coarse)': Object.assign(new Element(), { matches: mobile })
    };
    let io, mutations, stored = paused ? 'true' : 'false';
    const frames = new Map();
    const setItems = next => { items = next; items.forEach(item => item.parentElement = { children: items }); };
    setItems(items);
    root.querySelector = selector => selector === '[data-pw-motion-toggle]' ? button : selector === 'main' ? root : null;
    root.querySelectorAll = () => items;
    root.contains = item => items.includes(item);
    document.hidden = false;
    class Observer {
        observed = new Set();
        constructor(callback) { this.callback = callback; io = this; }
        observe(item) { this.observed.add(item); }
        unobserve(item) { this.observed.delete(item); }
        disconnect() { this.observed.clear(); }
    }
    class Mutations {
        constructor(callback) { this.callback = callback; mutations = this; }
        observe() {}
        disconnect() { this.disconnected = true; }
    }
    const connection = vm.runInNewContext(source.replace('export function connect', 'function connect') + '\nconnect(root);', {
        root, document, innerHeight: 800, matchMedia: query => media[query],
        IntersectionObserver: observers ? Observer : undefined, MutationObserver: Mutations,
        sessionStorage: { getItem: () => stored, setItem: (_key, value) => stored = value },
        requestAnimationFrame: callback => { const id = frames.size + 1; frames.set(id, callback); return id; },
        cancelAnimationFrame: id => frames.delete(id)
    });
    return { root, button, document, media, connection, io, mutations,
        enter(item, visible = true) {
            if (io?.observed.has(item)) io.callback([{ target: item, isIntersecting: visible }]);
            if (io?.observed.has(item.image)) io.callback([{ target: item.image, isIntersecting: visible }]);
        },
        enterImage(item, visible = true) {
            if (io?.observed.has(item.image)) io.callback([{ target: item.image, isIntersecting: visible }]);
        },
        load(item) { item.image.complete = true; item.image.naturalWidth = 512; item.image.emit('load'); },
        change(query, value) { media[query].matches = value; media[query].emit('change'); },
        replace(next) { setItems(next); mutations.callback(); },
        flushFrames() { for (const callback of frames.values()) callback(); frames.clear(); }
    };
}
const animated = item => item.image.classList.contains('pw-mobile-image-enter');
const mobileQuery = '(hover: none), (pointer: coarse)';
const reducedQuery = '(prefers-reduced-motion: reduce)';

// Visible image, below-fold frame reveal, and lazy-loaded image each play once.
let hero = frame('hero'), service = frame('image', { top: 1000 }), project = frame('card', { loaded: false });
let h = setup([hero, service, project]);
assert.equal(hero.classes.has('pw-motion-pending'), false);
assert.equal(service.classes.has('pw-motion-pending'), true);
h.enter(hero); h.enter(service); h.enter(project);
assert.equal(animated(hero), true);
assert.equal(animated(service), true);
assert.equal(service.classes.has('pw-motion-pending'), false);
assert.equal(animated(project), false);
h.load(project);
assert.equal(animated(project), true);
hero.image.emit('animationend');
h.enter(hero, false); h.enter(hero);
assert.equal(animated(hero), false);

// A lazy image loaded after leaving the viewport waits for re-entry.
project = frame('card', { loaded: false });
h = setup([project]); h.enter(project); h.enter(project, false); h.load(project);
assert.equal(animated(project), false);
h.enter(project); assert.equal(animated(project), true);

// Seeing a card's text while its photo is outside the viewport is not image entry.
project = frame('card', { top: 1000 }); h = setup([project]);
h.io.callback([{ target: project, isIntersecting: true }]);
assert.equal(animated(project), false);
h.enterImage(project); assert.equal(animated(project), true);

// Pause/reduced motion cancel current effects, reveal content, and never replay seen images.
for (const preference of ['toggle', 'reduced']) {
    hero = frame('hero'); service = frame('image', { top: 1000 });
    h = setup([hero, service]); h.enter(hero);
    if (preference === 'toggle') h.button.emit('click'); else h.change(reducedQuery, true);
    assert.equal(animated(hero), false);
    assert.equal(h.root.dataset.pwMotionState, 'paused');
    assert.equal(service.classes.has('pw-motion-pending'), false);
    if (preference === 'toggle') h.button.emit('click'); else h.change(reducedQuery, false);
    assert.equal(animated(hero), false);
    h.enter(service); assert.equal(animated(service), true);
}
for (const preference of [{ paused: true }, { reduced: true }]) {
    hero = frame('hero'); h = setup([hero], preference); h.enter(hero);
    assert.equal(animated(hero), false);
    if (preference.paused) h.button.emit('click'); else h.change(reducedQuery, false);
    assert.equal(animated(hero), false);
}

// Hidden tabs defer new playback; existing classes survive to resume via CSS.
hero = frame('hero'); h = setup([hero]);
h.document.hidden = true; h.document.emit('visibilitychange'); h.enter(hero);
assert.equal(animated(hero), false);
h.document.hidden = false; h.document.emit('visibilitychange');
assert.equal(animated(hero), true);
h.document.hidden = true; h.document.emit('visibilitychange');
assert.equal(h.root.classes.has('pw-motion-tab-hidden'), true);
assert.equal(animated(hero), true);

// Filtering detaches old image callbacks and registers freshly rendered project photos.
project = frame('card'); h = setup([project]); h.enter(project);
const next = frame('card'); h.replace([next]);
assert.equal(animated(project), false);
assert.equal(project.image.listeners.get('load').size, 0);
h.enter(next); assert.equal(animated(next), true);
h.connection.disconnect();
assert.equal(animated(next), false);
assert.equal(next.image.listeners.get('animationend').size, 0);
assert.equal(h.io.observed.size, 0);
assert.equal(h.media[mobileQuery].listeners.get('change').size, 0);
assert.equal(h.mutations.disconnected, true);

// Desktop keeps pointer tilt, touch does not tilt; icons never become image targets.
hero = frame('hero'); h = setup([hero], { mobile: false }); h.enter(hero);
assert.equal(animated(hero), false);
hero.emit('pointermove', { pointerType: 'mouse', clientX: 200, clientY: 0 }); h.flushFrames();
assert.equal(hero.properties.get('--pw-tilt-y'), '9deg');
hero = frame('hero'); h = setup([hero]);
hero.emit('pointermove', { pointerType: 'touch', clientX: 200, clientY: 0 }); h.flushFrames();
assert.equal(hero.properties.has('--pw-tilt-y'), false);
const icon = frame('card', { image: false }); h = setup([icon]); h.enter(icon);
assert.equal(icon.classes.has('pw-motion-pending'), false);
hero = frame('hero'); h = setup([hero]); h.enter(hero); h.change(mobileQuery, false);
assert.equal(animated(hero), false);
hero = frame('hero', { top: 1000 }); h = setup([hero], { observers: false });
assert.equal(animated(hero), false);
assert.equal(hero.classes.has('pw-motion-pending'), false);
console.log('PASS: mobile loaded/lazy viewport entry, once-only playback, filtering, preferences, hidden tabs, pointer behavior, fallback and cleanup.');
