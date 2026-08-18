# FashionPass.Tests

Automated testing suite for [fashionpass.com](https://www.fashionpass.com) built with a clean **Page Object Model (POM)** architecture. Runs against real browsers via **Playwright** (Chromium/Chrome + WebKit/Safari) for web and **Appium** (iOS/Android) for mobile.

> **Status:** Shared, public, open-source project. Contributions welcome — see [CONTRIBUTING](#contributing).

---

## Stack

| Area | Choice |
|------|--------|
| Language | C# / .NET 7 |
| Test framework | NUnit 3 |
| Web automation | Microsoft.Playwright (Chromium, WebKit, Firefox) |
| Mobile automation | Appium.WebDriver (iOS/XCUITest, Android/UiAutomator2) |
| Assertions | FluentAssertions |
| Config | `appsettings.json` + environment overrides |

## Project layout

```
FashionPass.Tests/
├── FashionPass.Tests.sln
└── src/
    └── FashionPass.Tests/
        ├── appsettings.json              # test configuration (browser, timeouts, mobile)
        ├── Config/
        │   └── TestConfig.cs             # typed config + env overrides
        ├── Hooks/
        │   ├── BaseTest.cs               # shared test root
        │   ├── WebTest.cs                # Playwright lifecycle (browser/context/page per test)
        │   └── MobileTest.cs             # Appium lifecycle
        ├── Drivers/
        │   ├── PlaywrightDriverFactory.cs
        │   └── MobileDriverFactory.cs
        ├── Pages/
        │   ├── BasePage.cs               # web POM base (nav, click, fill, waits)
        │   ├── Components/               # reusable page fragments
        │   │   ├── HeaderComponent.cs
        │   │   └── FooterComponent.cs
        │   ├── Web/                      # web page objects
        │   │   ├── HomePage.cs
        │   │   ├── LoginPage.cs
        │   │   ├── SignUpPage.cs
        │   │   ├── MembershipPage.cs
        │   │   ├── ProductListingPage.cs
        │   │   ├── ProductDetailPage.cs
        │   │   └── BagPage.cs
        │   └── Mobile/                   # mobile page objects (Appium)
        │       ├── BaseMobilePage.cs
        │       └── HomeMobilePage.cs
        ├── Data/
        │   ├── Models/User.cs
        │   └── TestDataFactory.cs
        ├── Utilities/
        │   ├── Selectors.cs              # centralised, single source of truth for selectors
        │   └── ScreenshotHelper.cs
        └── Tests/
            ├── Web/                      # web tests
            │   ├── HomePageTests.cs
            │   ├── NavigationTests.cs
            │   └── LoginTests.cs
            └── Mobile/
                └── HomeMobileTests.cs
```

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- Playwright browsers (one-time):
  ```bash
  dotnet tool install --global Microsoft.Playwright.CLI
  playwright install chromium webkit   # Chrome/Chromium + Safari (WebKit)
  ```
- (Mobile only) Appium server: `npm install -g appium`, plus a simulator/emulator or real device.

## Run

```bash
# web tests (headless Chromium)
FASHIONPASS_HEADLESS=true dotnet test src/FashionPass.Tests/FashionPass.Tests.csproj --filter "FullyQualifiedName~Tests.Web"

# mobile tests (skip automatically unless Mobile.Enabled=true)
dotnet test --filter "FullyQualifiedName~Tests.Mobile"
```

## Configuration

Settings live in `src/FashionPass.Tests/appsettings.json`. The most relevant:

- `Browser.Type` — `chromium`, `webkit` (Safari engine), `firefox`
- `Browser.Channel` — e.g. `"chrome"` to use a system-installed Chrome
- `Browser.Headless` — run without a visible window
- `Mobile.Enabled` — turns mobile tests on (requires an Appium server)
- `Mobile.AppiumUrl`, `Mobile.DeviceName`, `Mobile.PlatformVersion` — device/simulator target

Environment overrides (useful for CI):

| Variable | Overrides |
|----------|-----------|
| `FASHIONPASS_ENV` | selects `appsettings.{env}.json` (default `production`) |
| `FASHIONPASS_BASEURL` | `BaseUrl` |
| `FASHIONPASS_BROWSER_TYPE` | `Browser.Type` |
| `FASHIONPASS_HEADLESS` | `Browser.Headless` |
| `FASHIONPASS_DEFAULT_TIMEOUT` / `FASHIONPASS_NAV_TIMEOUT` | timeouts |
| `FASHIONPASS_MOBILE_ENABLED` | `Mobile.Enabled` |

## Selectors & the live site

fashionpass.com is a Next.js/React SPA — the header, search bar, login, and bag are **client-rendered** after hydration, so tests wait for hydration (see `HeaderComponent.WaitUntilHydratedAsync`). All CSS/XPath selectors are centralised in `Utilities/Selectors.cs`. If the site's markup changes, update selectors **only there**.

> The login/signup form selectors were inferred from typical Next.js forms and may need a quick verification pass against the live DOM.

## CI

`.github/workflows/ci.yml` builds and runs the web suite headlessly on every push/PR.

## Contributing

1. Fork the repo and create a feature branch.
2. Keep selectors in `Utilities/Selectors.cs`, page logic in `Pages/`, and test intent in `Tests/`.
3. Run `dotnet build` and the web test suite before opening a PR.

## License

MIT — see [LICENSE](LICENSE).