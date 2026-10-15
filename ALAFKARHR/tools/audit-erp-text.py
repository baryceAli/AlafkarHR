"""Read-only ERP source text audit. Run with Python 3; --json emits findings.

Exit 1: invalid UTF-8 or high-confidence mojibake. REVIEW findings require
human assessment and do not fail the check. No intentional exceptions exist.
Public website, auth/login, storefront/POS, generated and vendor files are excluded.
"""
import argparse
import json
import os
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
ROOTS = ("UI/AlAfkarERP/AlAfkarERP.Shared", "UI/AlAfkarERP/AlAfkarERP.Web",
         "UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/lib/MyJs", "src/Modules", "src/Shared")
SKIP = {"bin", "obj", "migrations", "lib", "node_modules", "vendor",
        "publicwebsite", "website", "cart", "hometemplates"}
EXTENSIONS = {".razor", ".cs", ".resx", ".json", ".js", ".css", ".html"}
NON_ASCII = re.compile(r"[^\x00-\x7f]+")
SUSPICIOUS = re.compile(r"[\ufffd\u0080-\u009f]|[ÃÂØÙ]|â[€œ“]|ðŸ|ï»|ط§|ظ„")


def source_files():
    for relative in ROOTS:
        for directory, children, files in os.walk(ROOT / relative):
            children[:] = sorted(c for c in children if c.lower() not in SKIP)
            for name in sorted(files):
                path = Path(directory) / name
                if path.suffix.lower() not in EXTENSIONS:
                    continue
                if path.relative_to(ROOT).as_posix() in (
                    "UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Home.razor",
                    "UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Home.razor.css",
                ):
                    continue  # The /store route, not the ERP home workspace.
                if any(x in name.lower() for x in (".min.", "pos.", "login", "registerpage", "forgotpasswordpage", "authnewlayout")):
                    continue
                yield path


def undo_encoding(value, encoding):
    # Windows-1252 leaves five C1 bytes undefined. Preserve those bytes exactly;
    # never use replacement/ignore error handling, which would lose source text.
    raw = bytearray()
    for char in value:
        try:
            raw.extend(char.encode(encoding))
        except UnicodeEncodeError:
            if encoding == "cp1252" and ord(char) in (0x81, 0x8d, 0x8f, 0x90, 0x9d):
                raw.append(ord(char))
            else:
                raise
    return bytes(raw).decode("utf-8")


def recovered_span(value):
    if not SUSPICIOUS.search(value):
        return None
    for encoding in ("cp1252", "latin1", "cp1256"):
        try:
            decoded = undo_encoding(value, encoding)
        except (UnicodeEncodeError, UnicodeDecodeError):
            continue
        if decoded != value and not SUSPICIOUS.search(decoded):
            if any(ord(char) > 127 for char in decoded):
                return decoded
    return None


def audit():
    findings = []
    count = 0
    for path in source_files():
        count += 1
        relative = path.relative_to(ROOT).as_posix()
        try:
            content = path.read_bytes().decode("utf-8-sig")
        except UnicodeDecodeError as error:
            findings.append(dict(file=relative, line=1, severity="ERROR",
                                 original="", replacement=None,
                                 reason=f"Invalid UTF-8 at byte {error.start}"))
            continue
        for number, line in enumerate(content.split("\n"), 1):
            for match in NON_ASCII.finditer(line):
                original = match.group()
                if not SUSPICIOUS.search(original):
                    continue
                replacement = recovered_span(original)
                fatal = replacement is not None or "\ufffd" in original
                findings.append(dict(file=relative, line=number,
                                     severity="ERROR" if fatal else "REVIEW",
                                     original=original, replacement=replacement,
                                     reason="Recoverable mojibake" if replacement else
                                     "Replacement character" if fatal else "Suspicious text"))
    return count, findings


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--json", action="store_true")
    args = parser.parse_args()
    count, findings = audit()
    if args.json:
        print(json.dumps(dict(files_scanned=count, findings=findings), ensure_ascii=True, indent=2))
    else:
        for item in findings:
            # Escape non-ASCII for reliable output in legacy Windows terminals.
            span = ascii(item["original"])
            print(f'{item["file"]}:{item["line"]}: {item["severity"]}: {item["reason"]}: {span}')
        errors = sum(f["severity"] == "ERROR" for f in findings)
        print(f"Scanned {count} files: {errors} errors, {len(findings) - errors} review candidates.")
    return int(any(f["severity"] == "ERROR" for f in findings))


if __name__ == "__main__":
    sys.exit(main())
