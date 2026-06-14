window.alafkarTableExport = (() => {
    function value(source, pascalName, camelName) {
        return source?.[pascalName] ?? source?.[camelName];
    }

    function text(valueToFormat) {
        if (valueToFormat === null || valueToFormat === undefined || valueToFormat === "") {
            return "-";
        }

        return String(valueToFormat);
    }

    function escapeHtml(valueToEscape) {
        return text(valueToEscape)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function normalizePayload(payload) {
        return {
            title: text(value(payload, "Title", "title")),
            subtitle: text(value(payload, "Subtitle", "subtitle")),
            direction: text(value(payload, "Direction", "direction")).toLowerCase() === "rtl" ? "rtl" : "ltr",
            generatedAtLabel: text(value(payload, "GeneratedAtLabel", "generatedAtLabel")),
            columns: value(payload, "Columns", "columns") ?? [],
            rows: value(payload, "Rows", "rows") ?? []
        };
    }

    function buildTableHtml(payload) {
        const headers = payload.columns
            .map(column => `<th>${escapeHtml(column)}</th>`)
            .join("");
        const rows = payload.rows
            .map(row => `<tr>${Array.from(row).map(cell => `<td>${escapeHtml(cell)}</td>`).join("")}</tr>`)
            .join("");

        return `<table><thead><tr>${headers}</tr></thead><tbody>${rows}</tbody></table>`;
    }

    function buildDocumentHtml(rawPayload) {
        const payload = normalizePayload(rawPayload);
        const generatedAt = new Date().toLocaleString(payload.direction === "rtl" ? "ar" : "en");

        return `<!doctype html>
<html lang="${payload.direction === "rtl" ? "ar" : "en"}" dir="${payload.direction}">
<head>
    <meta charset="utf-8">
    <title>${escapeHtml(payload.title)}</title>
    <style>
        body {
            color: #111827;
            font-family: Arial, Tahoma, sans-serif;
            margin: 24px;
        }

        h1 {
            font-size: 22px;
            margin: 0 0 6px;
        }

        .meta {
            color: #6b7280;
            font-size: 12px;
            margin-bottom: 18px;
        }

        table {
            border-collapse: collapse;
            width: 100%;
        }

        th,
        td {
            border: 1px solid #d1d5db;
            font-size: 12px;
            padding: 8px;
            text-align: start;
            vertical-align: top;
        }

        th {
            background: #f3f4f6;
            font-weight: 700;
        }

        @media print {
            body {
                margin: 12mm;
            }
        }
    </style>
</head>
<body>
    <h1>${escapeHtml(payload.title)}</h1>
    <div class="meta">${escapeHtml(payload.subtitle)}<br>${escapeHtml(payload.generatedAtLabel)}: ${escapeHtml(generatedAt)}</div>
    ${buildTableHtml(payload)}
</body>
</html>`;
    }

    function normalizeCellText(cell) {
        return text(cell?.textContent)
            .replace(/\s+/g, " ")
            .trim() || "-";
    }

    function isActionHeader(headerText) {
        const normalized = headerText.toLowerCase();
        return normalized === "actions"
            || normalized === "action"
            || normalized === "الإجراءات"
            || normalized === "الاجراءات"
            || normalized === "العمليات";
    }

    function findNextTable(sourceElement, tableSelector) {
        const selector = tableSelector || "table";
        let cursor = sourceElement;

        while (cursor) {
            let sibling = cursor.nextElementSibling;
            while (sibling) {
                if (sibling.matches?.(selector)) {
                    return sibling;
                }

                const nestedTable = sibling.querySelector?.(selector);
                if (nestedTable) {
                    return nestedTable;
                }

                sibling = sibling.nextElementSibling;
            }

            cursor = cursor.parentElement;
        }

        return document.querySelector(selector);
    }

    function buildPayloadFromTable(tableSelector, meta, sourceElement) {
        const table = sourceElement
            ? findNextTable(sourceElement, tableSelector)
            : document.querySelector(tableSelector || "table");
        if (!table) {
            return normalizePayload(meta);
        }

        const headerCells = Array.from(table.querySelectorAll("thead th"));
        const exportedColumnIndexes = headerCells
            .map((cell, index) => ({ cell, index, text: normalizeCellText(cell) }))
            .filter(column => !column.cell.matches("[data-export-ignore], .no-export")
                && !isActionHeader(column.text));

        const columns = exportedColumnIndexes.map(column => column.text);
        const bodyRows = Array.from(table.querySelectorAll("tbody tr"))
            .filter(row => !row.matches("[data-export-ignore], .no-export"))
            .map(row => {
                const cells = Array.from(row.children);
                return exportedColumnIndexes.map(column => normalizeCellText(cells[column.index]));
            })
            .filter(row => row.some(cell => cell !== "-"));

        const payload = normalizePayload(meta);
        if (payload.title === "-") {
            payload.title = normalizeCellText(document.querySelector(".erp-page-title, h1, h3, h5"));
        }

        return {
            ...payload,
            columns,
            rows: bodyRows
        };
    }

    function openPrintWindow(payload) {
        const printWindow = window.open("", "_blank", "width=1200,height=800");
        if (!printWindow) {
            window.print();
            return;
        }

        printWindow.document.open();
        printWindow.document.write(buildDocumentHtml(payload));
        printWindow.document.close();
        printWindow.focus();
        printWindow.setTimeout(() => printWindow.print(), 250);
    }

    function downloadExcel(payload, fileName) {
        const documentHtml = buildDocumentHtml(payload);
        const blob = new Blob(["\ufeff", documentHtml], {
            type: "application/vnd.ms-excel;charset=utf-8"
        });
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = fileName || "table.xls";
        document.body.appendChild(link);
        link.click();
        URL.revokeObjectURL(link.href);
        link.remove();
    }

    function downloadTableExcel(tableSelector, meta, fileName) {
        downloadExcel(buildPayloadFromTable(tableSelector, meta), fileName);
    }

    function exportTablePdf(tableSelector, meta) {
        openPrintWindow(buildPayloadFromTable(tableSelector, meta));
    }

    function printTable(tableSelector, meta) {
        openPrintWindow(buildPayloadFromTable(tableSelector, meta));
    }

    function downloadNearestTableExcel(sourceElement, tableSelector, meta, fileName) {
        downloadExcel(buildPayloadFromTable(tableSelector, meta, sourceElement), fileName);
    }

    function exportNearestTablePdf(sourceElement, tableSelector, meta) {
        openPrintWindow(buildPayloadFromTable(tableSelector, meta, sourceElement));
    }

    function printNearestTable(sourceElement, tableSelector, meta) {
        openPrintWindow(buildPayloadFromTable(tableSelector, meta, sourceElement));
    }

    return {
        downloadExcel,
        downloadTableExcel,
        downloadNearestTableExcel,
        exportPdf: openPrintWindow,
        exportTablePdf,
        exportNearestTablePdf,
        print: openPrintWindow,
        printTable,
        printNearestTable
    };
})();
