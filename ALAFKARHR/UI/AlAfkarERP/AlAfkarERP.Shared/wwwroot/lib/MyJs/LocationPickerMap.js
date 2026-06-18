window.alafkarLocationPicker = (() => {
    const maps = new Map();
    const tileSize = 256;

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function lonToTileX(lon, zoom) {
        return ((lon + 180) / 360) * Math.pow(2, zoom);
    }

    function latToTileY(lat, zoom) {
        const latRad = lat * Math.PI / 180;
        return (1 - Math.log(Math.tan(latRad) + 1 / Math.cos(latRad)) / Math.PI) / 2 * Math.pow(2, zoom);
    }

    function tileXToLon(x, zoom) {
        return x / Math.pow(2, zoom) * 360 - 180;
    }

    function tileYToLat(y, zoom) {
        const n = Math.PI - 2 * Math.PI * y / Math.pow(2, zoom);
        return 180 / Math.PI * Math.atan(0.5 * (Math.exp(n) - Math.exp(-n)));
    }

    function getState(elementId) {
        const state = maps.get(elementId);
        if (!state) {
            throw new Error(`Location picker '${elementId}' is not initialized.`);
        }

        return state;
    }

    function render(state) {
        const { element, marker, zoom } = state;
        const width = element.clientWidth || 640;
        const height = element.clientHeight || 320;
        const centerX = lonToTileX(state.lon, zoom) * tileSize;
        const centerY = latToTileY(state.lat, zoom) * tileSize;
        const startX = centerX - width / 2;
        const startY = centerY - height / 2;
        const minTileX = Math.floor(startX / tileSize);
        const maxTileX = Math.floor((startX + width) / tileSize);
        const minTileY = Math.floor(startY / tileSize);
        const maxTileY = Math.floor((startY + height) / tileSize);
        const tilesAcross = Math.pow(2, zoom);

        element.querySelectorAll(".location-picker-tile").forEach(tile => tile.remove());

        for (let x = minTileX; x <= maxTileX; x++) {
            for (let y = minTileY; y <= maxTileY; y++) {
                if (y < 0 || y >= tilesAcross) {
                    continue;
                }

                const wrappedX = ((x % tilesAcross) + tilesAcross) % tilesAcross;
                const tile = document.createElement("img");
                tile.className = "location-picker-tile";
                tile.alt = "";
                tile.draggable = false;
                tile.src = `https://tile.openstreetmap.org/${zoom}/${wrappedX}/${y}.png`;
                tile.style.left = `${Math.round(x * tileSize - startX)}px`;
                tile.style.top = `${Math.round(y * tileSize - startY)}px`;
                element.insertBefore(tile, marker);
            }
        }

        marker.style.left = "50%";
        marker.style.top = "50%";
    }

    function notifyLocationSelected(state) {
        if (state.dotNetRef?.invokeMethodAsync) {
            state.dotNetRef.invokeMethodAsync("OnMapLocationSelected", state.lat, state.lon);
        }
    }

    function setCenter(elementId, lat, lon, notify) {
        const state = getState(elementId);
        state.lat = clamp(Number(lat) || 0, -85, 85);
        state.lon = clamp(Number(lon) || 0, -180, 180);
        render(state);

        if (notify) {
            notifyLocationSelected(state);
        }
    }

    function setZoom(elementId, delta) {
        const state = getState(elementId);
        state.zoom = clamp(state.zoom + delta, 2, 19);
        render(state);
        return state.zoom;
    }

    function setCenterFromPixelOffset(state, offsetX, offsetY) {
        const centerX = lonToTileX(state.lon, state.zoom) * tileSize;
        const centerY = latToTileY(state.lat, state.zoom) * tileSize;
        state.lon = tileXToLon((centerX + offsetX) / tileSize, state.zoom);
        state.lat = tileYToLat((centerY + offsetY) / tileSize, state.zoom);
    }

    return {
        init(elementId, lat, lon, zoomOrDotNetRef, dotNetRef) {
            const element = document.getElementById(elementId);
            if (!element) {
                return;
            }

            element.innerHTML = "";
            element.classList.add("location-picker-map");

            const marker = document.createElement("div");
            marker.className = "location-picker-marker";
            marker.innerHTML = '<i class="bi bi-geo-alt-fill"></i>';
            element.appendChild(marker);

            const initialLat = clamp(Number(lat) || 24.7136, -85, 85);
            const initialLon = clamp(Number(lon) || 46.6753, -180, 180);
            const hasZoom = typeof zoomOrDotNetRef === "number";
            const initialZoom = hasZoom ? clamp(zoomOrDotNetRef, 2, 19) : 13;
            const callbackRef = hasZoom ? dotNetRef : zoomOrDotNetRef;

            const state = {
                element,
                marker,
                dotNetRef: callbackRef,
                zoom: initialZoom,
                lat: initialLat,
                lon: initialLon,
                isDragging: false,
                didDrag: false,
                dragStartX: 0,
                dragStartY: 0,
                dragStartLat: 0,
                dragStartLon: 0
            };

            maps.set(elementId, state);

            element.addEventListener("pointerdown", event => {
                const current = getState(elementId);
                current.isDragging = true;
                current.didDrag = false;
                current.dragStartX = event.clientX;
                current.dragStartY = event.clientY;
                current.dragStartLat = current.lat;
                current.dragStartLon = current.lon;
                element.setPointerCapture(event.pointerId);
                element.classList.add("is-dragging");
            });

            element.addEventListener("pointermove", event => {
                const current = getState(elementId);
                if (!current.isDragging) {
                    return;
                }

                const offsetX = current.dragStartX - event.clientX;
                const offsetY = current.dragStartY - event.clientY;
                if (Math.abs(offsetX) > 3 || Math.abs(offsetY) > 3) {
                    current.didDrag = true;
                }

                current.lat = current.dragStartLat;
                current.lon = current.dragStartLon;
                setCenterFromPixelOffset(current, offsetX, offsetY);
                render(current);
            });

            element.addEventListener("pointerup", event => {
                const current = getState(elementId);
                if (!current.isDragging) {
                    return;
                }

                current.isDragging = false;
                element.releasePointerCapture(event.pointerId);
                element.classList.remove("is-dragging");

                if (current.didDrag) {
                    notifyLocationSelected(current);
                }
            });

            element.addEventListener("pointercancel", event => {
                const current = getState(elementId);
                current.isDragging = false;
                element.releasePointerCapture(event.pointerId);
                element.classList.remove("is-dragging");
            });

            element.addEventListener("click", event => {
                const current = getState(elementId);
                if (current.didDrag) {
                    current.didDrag = false;
                    return;
                }

                const rect = element.getBoundingClientRect();
                const width = element.clientWidth || 640;
                const height = element.clientHeight || 320;
                const centerX = lonToTileX(current.lon, current.zoom) * tileSize;
                const centerY = latToTileY(current.lat, current.zoom) * tileSize;
                const clickedX = centerX + (event.clientX - rect.left - width / 2);
                const clickedY = centerY + (event.clientY - rect.top - height / 2);
                current.lon = tileXToLon(clickedX / tileSize, current.zoom);
                current.lat = tileYToLat(clickedY / tileSize, current.zoom);
                render(current);
                notifyLocationSelected(current);
            });

            render(state);
        },

        setCenter,

        zoomIn(elementId) {
            return setZoom(elementId, 1);
        },

        zoomOut(elementId) {
            return setZoom(elementId, -1);
        },

        useCurrentLocation(elementId) {
            const state = getState(elementId);

            if (!navigator.geolocation) {
                return false;
            }

            navigator.geolocation.getCurrentPosition(position => {
                setCenter(elementId, position.coords.latitude, position.coords.longitude, true);
            });

            return true;
        },

        dispose(elementId) {
            maps.delete(elementId);
        }
    };
})();
