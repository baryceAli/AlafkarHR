// PublicWebsite only. A brief transport retry must not interrupt normal reading.
const reconnectDelay = 3000;

function attach() {
    const dialog = document.getElementById('components-reconnect-modal');
    const retryButton = document.getElementById('components-reconnect-button');
    const resumeButton = document.getElementById('components-resume-button');
    if (!dialog || !retryButton || !resumeButton) return;

    let timer = null;
    let state = 'hide';
    let disposed = false;
    let busy = false;
    function cancelDisplay() {
        if (timer !== null) clearTimeout(timer);
        timer = null;
    }
    function openDialog() {
        cancelDisplay();
        if (!disposed && dialog.isConnected && !dialog.open) dialog.showModal();
    }
    function recovered() {
        state = 'hide';
        cancelDisplay();
        document.removeEventListener('visibilitychange', retryWhenVisible);
        if (dialog.open) dialog.close();
    }
    function handleState(event) {
        if (disposed) return;
        state = event.detail.state;
        switch (state) {
            case 'show':
            case 'retrying':
                if (timer === null && !dialog.open) {
                    timer = setTimeout(() => {
                        timer = null;
                        if (state === 'show' || state === 'retrying') openDialog();
                    }, reconnectDelay);
                }
                break;
            case 'hide':
                recovered();
                break;
            case 'failed':
                openDialog();
                document.addEventListener('visibilitychange', retryWhenVisible);
                break;
            case 'paused':
            case 'resume-failed':
                document.removeEventListener('visibilitychange', retryWhenVisible);
                openDialog();
                break;
            case 'rejected':
                cancelDisplay();
                location.reload();
                break;
        }
    }
    async function retry() {
        if (disposed || busy) return;
        busy = true;
        document.removeEventListener('visibilitychange', retryWhenVisible);
        try {
            const successful = await Blazor.reconnect();
            if (disposed) return;
            if (successful) recovered();
            else {
                const resumed = await Blazor.resumeCircuit();
                if (disposed) return;
                if (resumed) recovered();
                else location.reload();
            }
        } catch {
            if (!disposed) document.addEventListener('visibilitychange', retryWhenVisible);
        } finally {
            busy = false;
        }
    }
    async function resume() {
        if (disposed || busy) return;
        busy = true;
        try {
            const successful = await Blazor.resumeCircuit();
            if (disposed) return;
            if (successful) recovered();
            else location.reload();
        } catch {
            if (!disposed) location.reload();
        } finally {
            busy = false;
        }
    }
    function retryWhenVisible() {
        if (document.visibilityState === 'visible') void retry();
    }
    function detach(event) {
        disposed = true;
        cancelDisplay();
        dialog.removeEventListener('components-reconnect-state-changed', handleState);
        retryButton.removeEventListener('click', retry);
        resumeButton.removeEventListener('click', resume);
        document.removeEventListener('visibilitychange', retryWhenVisible);
        window.removeEventListener('pagehide', detach);
        if (dialog.open) dialog.close();
        // Reattach if the browser restores this document from its back/forward cache.
        if (event.persisted) window.addEventListener('pageshow', attach, { once: true });
    }
    dialog.addEventListener('components-reconnect-state-changed', handleState);
    retryButton.addEventListener('click', retry);
    resumeButton.addEventListener('click', resume);
    window.addEventListener('pagehide', detach);
    // Also handle a state already reported before the fingerprinted module loaded.
    const initial = ['failed', 'paused', 'resume-failed', 'rejected', 'show', 'retrying']
        .find(value => dialog.classList.contains(`components-reconnect-${value}`));
    if (initial) handleState({ detail: { state: initial } });
}

attach();
