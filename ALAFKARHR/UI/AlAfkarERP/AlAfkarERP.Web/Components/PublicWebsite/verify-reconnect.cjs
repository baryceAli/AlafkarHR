// Isolated state-machine checks; no application, network, browser or dependencies.
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const source = fs.readFileSync(path.join(__dirname, '../../wwwroot/website/reconnect.js'), 'utf8');

class Events {
    listeners = new Map();
    addEventListener(name, handler, options = {}) {
        const entries = this.listeners.get(name) ?? new Map();
        entries.set(handler, options);
        this.listeners.set(name, entries);
    }
    removeEventListener(name, handler) { this.listeners.get(name)?.delete(handler); }
    async emit(name, event = {}) {
        for (const [handler, options] of [...(this.listeners.get(name) ?? [])]) {
            if (options.once) this.removeEventListener(name, handler);
            await handler(event);
        }
    }
    count(name) { return this.listeners.get(name)?.size ?? 0; }
}
function setup(initial = '') {
    let now = 0, next = 0, reloads = 0;
    const timers = new Map();
    const dialog = Object.assign(new Events(), {
        open: false, isConnected: true, opens: 0, closes: 0,
        classList: { contains: value => value === `components-reconnect-${initial}` },
        showModal() { assert.equal(this.open, false); this.open = true; this.opens++; },
        close() { assert.equal(this.open, true); this.open = false; this.closes++; }
    });
    const retry = new Events(), resume = new Events(), window = new Events();
    const document = Object.assign(new Events(), {
        visibilityState: 'visible',
        getElementById: id => ({ 'components-reconnect-modal': dialog,
            'components-reconnect-button': retry, 'components-resume-button': resume })[id]
    });
    const Blazor = { reconnect: async () => true, resumeCircuit: async () => true };
    vm.runInNewContext(source, { document, window, Blazor, location: { reload: () => reloads++ },
        setTimeout(fn, delay) { const id = ++next; timers.set(id, { fn, at: now + delay }); return id; },
        clearTimeout(id) { timers.delete(id); } });
    return { dialog, document, window, retry, resume, Blazor, timers,
        reloads: () => reloads,
        state: state => dialog.emit('components-reconnect-state-changed', { detail: { state } }),
        advance(ms) {
            now += ms;
            for (const [id, timer] of [...timers]) if (timer.at <= now) { timers.delete(id); timer.fn(); }
        }
    };
}
(async () => {
    let h = setup();
    assert.equal(h.dialog.open, false);
    assert.equal(h.timers.size, 0);
    await h.state('show');
    h.advance(2000);
    await h.state('show');
    await h.state('retrying');
    h.advance(999);
    assert.equal(h.dialog.open, false);
    h.advance(1);
    assert.equal(h.dialog.opens, 1); // Repeated events did not reset the first deadline.
    await h.state('show');
    assert.equal(h.timers.size, 0);
    await h.state('hide');
    await h.state('hide');
    assert.equal(h.dialog.closes, 1);

    h = setup();
    await h.state('show');
    h.advance(1000);
    await h.state('hide');
    h.advance(5000);
    assert.equal(h.dialog.opens, 0);

    for (const state of ['failed', 'paused', 'resume-failed']) {
        h = setup();
        await h.state('show');
        await h.state(state);
        assert.equal(h.dialog.open, true);
        assert.equal(h.timers.size, 0);
        h.advance(5000);
        assert.equal(h.dialog.opens, 1);
    }
    h = setup();
    await h.state('failed');
    await h.retry.emit('click');
    assert.equal(h.dialog.open, false);
    assert.equal(h.document.count('visibilitychange'), 0);
    await h.state('paused');
    await h.resume.emit('click');
    assert.equal(h.dialog.open, false);

    h = setup();
    h.Blazor.reconnect = async () => false;
    h.Blazor.resumeCircuit = async () => false;
    await h.state('failed');
    await h.retry.emit('click');
    assert.equal(h.reloads(), 1);
    for (const resumeResult of [async () => false, async () => { throw new Error('resume failed'); }]) {
        h = setup();
        h.Blazor.resumeCircuit = resumeResult;
        await h.state('paused');
        await h.resume.emit('click');
        assert.equal(h.reloads(), 1);
    }
    h = setup();
    h.Blazor.reconnect = async () => false;
    await h.state('failed');
    await h.retry.emit('click');
    assert.equal(h.dialog.open, false); // A successful circuit resume also recovers.
    assert.equal(h.reloads(), 0);
    h = setup();
    h.Blazor.reconnect = async () => { throw new Error('offline'); };
    await h.state('failed');
    await h.retry.emit('click');
    assert.equal(h.dialog.open, true);
    assert.equal(h.document.count('visibilitychange'), 1);
    h.Blazor.reconnect = async () => true;
    await h.document.emit('visibilitychange');
    // visibility callbacks initiate async retry without blocking event dispatch.
    await Promise.resolve();
    assert.equal(h.dialog.open, false);

    h = setup();
    await h.state('show');
    await h.window.emit('pagehide', { persisted: false });
    h.advance(5000);
    assert.equal(h.dialog.opens, 0);
    assert.equal(h.dialog.count('components-reconnect-state-changed'), 0);
    assert.equal(h.retry.count('click'), 0);
    assert.equal(h.resume.count('click'), 0);
    assert.equal(h.window.count('pagehide'), 0);

    h = setup();
    await h.window.emit('pagehide', { persisted: true });
    await h.window.emit('pageshow', { persisted: true });
    assert.equal(h.dialog.count('components-reconnect-state-changed'), 1);
    await h.state('show');
    h.advance(3000);
    assert.equal(h.dialog.open, true);
    h = setup('failed');
    assert.equal(h.dialog.open, true); // Late script load still exposes genuine failure.
    h = setup();
    await h.state('show');
    await h.state('rejected');
    assert.equal(h.reloads(), 1);
    assert.equal(h.timers.size, 0);
    console.log('PASS: 3-second grace, repeat events, early/late recovery, immediate failure/pause, retry/resume, unload and cached-document restoration.');
})().catch(error => { console.error(error); process.exitCode = 1; });
