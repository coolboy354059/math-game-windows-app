# Math Game Windows App

A small Windows math game app made from Teo's Figma design.

The home screen says **PLAY THE MATH GAME NOW!!!** and has a big PLAY button. The game screen shows a math question and clean number buttons from 1 to 10.

Questions use addition, subtraction, and small multiplication. Every answer is from 1 to 10, so it can be picked with the number buttons.

The app is a C# Windows Forms window that shows the game page with Microsoft WebView2 (the same web tech that powers Microsoft Edge). The game itself is plain HTML, CSS, and JavaScript.

## Run it

Install the .NET 8 SDK first, then run:

```bash
dotnet run
```

## Build the Windows app

Run:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -o publish
```

The finished app will be in the `publish` folder. Because it is self-contained, the `.exe` runs without installing .NET first.

You do need the **Microsoft Edge WebView2 Runtime** on the computer. It is already installed on Windows 11. On older Windows versions, download it free from Microsoft if the app says it is missing.

GitHub Actions also builds the app automatically on every push to `main`. You can download the built app from the **Actions** tab as the `math-game-windows-app` artifact, or from the **Releases** page as `math-game-windows-app.zip`.

## App icon

The app has its own icon: a red frog with math numbers on it. The icon file is `app.ico`, and the project file points to it with `<ApplicationIcon>`, so the `.exe`, the window, and the taskbar all show the frog icon.

## Auto-update

The app can update itself!

When the game starts, it quietly asks GitHub: "is there a newer version?" If there is, it pops up a box that says **A new version of Math Game is ready. Update now?**

- Click **Yes** and the app downloads the new version, swaps the files, and restarts itself.
- Click **No** and the game just keeps playing like normal.
- If there is no internet, nothing happens. The game still works.

To ship an update:

1. Change the version number in `MathGameWindowsApp.csproj`, for example `<Version>1.0.1</Version>`.
2. Push to `main`.
3. GitHub Actions builds the app and publishes a new Release (like `v1.0.1`) with the app zip attached.
4. Next time the game starts, it will spot the new version and ask to update.

## Files

- `Program.cs` — starts the app
- `MainForm.cs` — the app window with the WebView2 control
- `UpdateChecker.cs` — checks GitHub for a newer version and installs it
- `MathGameWindowsApp.csproj` — project and build settings
- `app.ico` — the frog app icon
- `index.html` — the two app screens
- `style.css` — the layout and colors from the design
- `game.js` — questions, score, and button logic
