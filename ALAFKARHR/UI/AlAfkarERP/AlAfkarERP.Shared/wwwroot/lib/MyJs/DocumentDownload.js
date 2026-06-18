window.alafkarDocumentDownload = (function () {
    function downloadBase64(fileName, contentType, base64) {
        var link = document.createElement("a");
        link.href = "data:" + (contentType || "application/octet-stream") + ";base64," + base64;
        link.download = fileName || "document";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    return {
        downloadBase64: downloadBase64
    };
})();
