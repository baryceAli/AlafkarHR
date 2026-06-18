import fs from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outputPath = path.join(__dirname, "AlAfkar_ERP_UAT_Pack.xlsx");

const sheets = [
  ["Summary", null],
  ["UAT_Master_Matrix", "UAT_Master_Matrix.csv"],
  ["UI_Coverage_Gaps", "UI_Coverage_Gaps.csv"],
  ["Role_Permission_UAT", "Role_Permission_UAT.csv"],
  ["Test_Data_Setup", "Test_Data_Setup.csv"],
  ["Backend_Inventory", "Backend_Functionality_Inventory.csv"],
  ["Frontend_Menu", "Frontend_Menu_Routes.csv"],
  ["Frontend_Pages", "Frontend_Page_Routes.csv"],
  ["Permissions", "Permission_Inventory.csv"],
];

const workbook = Workbook.create();
const summary = workbook.worksheets.add("Summary");
summary.showGridLines = false;

function parseCsv(text) {
  const rows = [];
  let row = [];
  let cell = "";
  let inQuotes = false;

  for (let i = 0; i < text.length; i += 1) {
    const char = text[i];
    const next = text[i + 1];

    if (char === '"') {
      if (inQuotes && next === '"') {
        cell += '"';
        i += 1;
      } else {
        inQuotes = !inQuotes;
      }
      continue;
    }

    if (char === "," && !inQuotes) {
      row.push(cell);
      cell = "";
      continue;
    }

    if ((char === "\n" || char === "\r") && !inQuotes) {
      if (char === "\r" && next === "\n") {
        i += 1;
      }
      row.push(cell);
      if (row.some((value) => value.length > 0)) {
        rows.push(row);
      }
      row = [];
      cell = "";
      continue;
    }

    cell += char;
  }

  row.push(cell);
  if (row.some((value) => value.length > 0)) {
    rows.push(row);
  }

  const width = rows.reduce((max, current) => Math.max(max, current.length), 0);
  return rows.map((current) => {
    const padded = current.slice();
    while (padded.length < width) {
      padded.push("");
    }
    return padded;
  });
}

const csvStats = [];
for (const [, fileName] of sheets.filter((sheet) => sheet[1])) {
  const csvPath = path.join(__dirname, fileName);
  const csvText = await fs.readFile(csvPath, "utf8");
  const rows = csvText.trim() ? csvText.trim().split(/\r?\n/).length - 1 : 0;
  csvStats.push([fileName, rows]);
}

summary.getRange("A1:D1").values = [["AlAfkar ERP UAT Pack", "", "", ""]];
summary.getRange("A1:D1").merge();
summary.getRange("A1:D1").format = {
  fill: "#1F4E79",
  font: { bold: true, color: "#FFFFFF", size: 16 },
};

summary.getRange("A3:B7").values = [
  ["Purpose", "Manual UAT matrix proving backend functionality is represented by UI routes/actions."],
  ["Source", "Backend Carter endpoints, PermissionList, Blazor menu routes, and Blazor @page routes."],
  ["Execution order", "Test_Data_Setup, Role_Permission_UAT, UAT_Master_Matrix, then UI_Coverage_Gaps review."],
  ["Generated artifact", "CSV matrices plus this workbook."],
  ["Regenerate", "powershell -NoProfile -ExecutionPolicy Bypass -File .\\docs\\uat\\generate-uat.ps1"],
];
summary.getRange("A3:A7").format = { font: { bold: true }, fill: "#D9EAF7" };
summary.getRange("A9:B9").values = [["Sheet/File", "Rows"]];
summary.getRange("A9:B9").format = { font: { bold: true, color: "#FFFFFF" }, fill: "#305496" };
summary.getRangeByIndexes(9, 0, csvStats.length, 2).values = csvStats;
summary.getRange("A:B").format.autofitColumns();

for (const [sheetName, fileName] of sheets.filter((sheet) => sheet[1])) {
  const csvText = await fs.readFile(path.join(__dirname, fileName), "utf8");
  const rows = parseCsv(csvText);
  const sheet = workbook.worksheets.add(sheetName);
  sheet.showGridLines = false;
  if (rows.length > 0 && rows[0].length > 0) {
    sheet.getRangeByIndexes(0, 0, rows.length, rows[0].length).values = rows;
    const used = sheet.getRangeByIndexes(0, 0, rows.length, rows[0].length);
    const header = sheet.getRangeByIndexes(0, 0, 1, rows[0].length);
    header.format = {
      fill: "#305496",
      font: { bold: true, color: "#FFFFFF" },
      wrapText: true,
    };
    used.format.borders = { preset: "all", style: "thin", color: "#D9E2F3" };
    used.format.autofitColumns();
    used.format.autofitRows();
  }
  sheet.freezePanes.freezeRows(1);
}

const scan = await workbook.inspect({
  kind: "match",
  searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A",
  options: { useRegex: true, maxResults: 20 },
  summary: "formula error scan",
});
console.log(scan.ndjson);

const preview = await workbook.render({
  sheetName: "Summary",
  autoCrop: "all",
  scale: 1,
  format: "png",
});
await fs.writeFile(path.join(__dirname, "AlAfkar_ERP_UAT_Pack.preview.png"), new Uint8Array(await preview.arrayBuffer()));

const xlsx = await SpreadsheetFile.exportXlsx(workbook);
await xlsx.save(outputPath);
console.log(outputPath);
