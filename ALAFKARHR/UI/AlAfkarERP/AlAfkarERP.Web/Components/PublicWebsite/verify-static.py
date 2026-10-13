"""Check public motion/navigation contracts without starting the app or a browser."""
from pathlib import Path
import re

HERE = Path(__file__).resolve().parent
WEB = HERE.parent.parent


def read(path):
    return path.read_text(encoding="utf-8-sig")


def require(condition, message):
    if not condition:
        raise SystemExit("FAIL: " + message)


motion = read(WEB / "wwwroot/website/motion.js")
css = read(WEB / "wwwroot/website/website.css")
pages = {p.name: read(p) for p in HERE.glob("*.razor")}
markup = "\n".join(pages.values())
layout = pages["WebsiteLayout.razor"]
router = pages["PublicRoutes.razor"]
app = read(WEB / "Components/App.razor")
content = read(HERE / "WebsiteContent.cs")

# Dataset camel case must map to the exact HTML/CSS attribute name.
dataset_names = set(re.findall(r"dataset\.([A-Za-z0-9]+)", motion))
require(dataset_names == {"pwMotion", "pwMotionState"}, "unexpected or stale dataset names")
for name in dataset_names:
    attribute = "data-" + re.sub(r"[A-Z]", lambda m: "-" + m[0].lower(), name)
    require(attribute in css, f"CSS does not recognize {attribute}")
require('data-pw-motion="hero"' in markup, "hero hook missing")
require('data-pw-motion="card"' in markup, "card hooks missing")
require('data-pw-motion="image"' in markup, "image hooks missing")
require('data-pw-motion-toggle' in markup, "motion toggle hook missing")
require('data-pw-motion-state="active"' in css and 'data-pw-motion-state="paused"' in css,
        "CSS active/paused states missing")
require(not re.search(r"d2Motion|data-d2-|--d2-", motion + css + markup), "stale motion names remain")
for token in set(re.findall(r"--pw-[a-z-]+", motion)):
    require(token in css, f"JavaScript/CSS custom property mismatch: {token}")
mobile_query = '(hover: none), (pointer: coarse)'
require(mobile_query in motion and '@media ' + mobile_query in css,
        "mobile image eligibility must agree between JavaScript and CSS")
require('--pw-mobile-image-duration: 1200ms;' in css and '--pw-image-zoom: 1.04;' in css,
        "mobile image motion timing/scale changed")
require('@keyframes pw-mobile-image-enter' in css and 'to { transform: scale(1); }' in css,
        "mobile image must settle at its normal scale")
require('observer?.observe(image)' in motion and 'image.naturalWidth === 0' in motion,
        "image motion must observe actual loaded images")
require("removeEventListener('load', enterImage)" in motion
        and "removeEventListener('animationend', finishImage)" in motion,
        "mobile image listeners must be removed")
require('.pw-site.pw-motion-tab-hidden .pw-mobile-image-enter' in css,
        "mobile image motion must pause in a hidden tab")
require(re.search(r'<div class="pw-hero-image"><img[^>]+/></div>\s*<div class="pw-art-caption"',
                  pages['WebsiteHome.razor']) is not None,
        "hero caption must remain outside the clipping wrapper")
require('.pw-hero-image { overflow: hidden; border-radius:' in css and '.pw-hero-art > img' not in css,
        "hero clipping and image selectors must match its new wrapper")
require('Assets["website/website.css"]' in app, "stylesheet bypasses asset resolver")
require('"/" + Assets["website/motion.js"]' in layout, "module import must resolve from the origin root")

canonical = {"/", "/about", "/services", "/projects", "/partners", "/contact", "/request-proposal"}
require(set(re.findall(r'@page "([^"]+)"', pages["WebsitePage.razor"])) == canonical,
        "canonical page coverage changed")
require('data-enhance-nav="false"' not in layout, "layout still disables public navigation")
for name, text in pages.items():
    if name == "WebsiteNotFound.razor":
        require('target="_top"' in text, "error exits must request a new HTTP response")
        continue
    for tag in re.findall(r"<a\b[^>]*>", text):
        require(not ('target="_top"' in tag and re.search(r'href="(?:/|@)', tag)),
                f"forced internal document navigation in {name}: {tag}")
require('new Uri(Navigation.Uri).AbsolutePath' in router and '@key="Pathname"' in router,
        "router must key page/layout lifetime by pathname")
require('.Query' not in router and '.Fragment' not in router, "query/fragment changes must retain form state")
require('FocusOnNavigate' in router, "navigation focus was removed")
require('mobileMenu.open = false' in motion, "same-page mobile links must close the menu")
for event in ('click', 'change', 'visibilitychange'):
    require(f"addEventListener('{event}'" in motion and f"removeEventListener('{event}'" in motion,
            f"missing lifecycle cleanup for {event}")
require('mutations?.disconnect()' in motion and 'observer?.disconnect()' in motion,
        "observer cleanup missing")
require('if (disposed)' in layout and 'initialized.InvokeVoidAsync("disconnect")' in layout,
        "late initialization must be cleaned up after navigation")
require('prefers-reduced-motion' in motion and 'sessionStorage' in motion,
        "motion preferences must remain supported")
require('WebsiteWhatsApp.ServiceKey' in pages["WebsiteProposalForm.razor"]
        and 'WebsiteContent.ProposalServices.Any' in pages["WebsiteProposalForm.razor"],
        "quotation service whitelist/preselection missing")
for key in ("iftar", "suqya", "meals", "baskets", "hajj", "infrastructure"):
    require(f'"/services#{key}"' in content, f"canonical service anchor missing: {key}")
reconnect = read(WEB / "wwwroot/website/reconnect.js")
reconnect_markup = pages["WebsiteReconnect.razor"]
require('Assets["website/reconnect.js"]' in reconnect_markup,
        "public reconnect must use its own resolved module")
require('ReconnectModal.razor.js' not in reconnect_markup,
        "public website must not attach the ERP reconnect handler")
require('Components/Layout/ReconnectModal.razor.js' in read(WEB / "Components/Layout/ReconnectModal.razor"),
        "ERP reconnect module reference changed")
dialog_tag = re.search(r'<dialog\b[^>]*>', reconnect_markup).group(0)
require(not re.search(r'\sopen(?:\s|=|>)', dialog_tag), "reconnect dialog must start closed")
require('.pw-reconnect:not([open]) { display: none; }' in css, "closed reconnect dialog CSS missing")
require('const reconnectDelay = 3000;' in reconnect and 'clearTimeout(timer)' in reconnect,
        "public reconnect delay/cancellation missing")
require("removeEventListener('pagehide', detach)" in reconnect
        and "removeEventListener('components-reconnect-state-changed', handleState)" in reconnect,
        "reconnect lifecycle cleanup missing")
print("PASS: motion dataset/HTML/CSS contract, mobile image/clipping contract, resolved assets, seven-page navigation, "
      "pathname lifetime, mobile-menu cleanup, quotation preselection and isolated reconnect markup/lifecycle.")
