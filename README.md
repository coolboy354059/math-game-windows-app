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

GitHub Actions also builds the app automatically on every push to `main`. You can download the built app from the **Actions** tab as the `math-game-windows-app` artifact.

## Files

- `Program.cs` — starts the app
- `MainForm.cs` — the app window with the WebView2 control
- `MathGameWindowsApp.csproj` — project and build settings
- `index.html` — the two app screens
- `style.css` — the layout and colors from the design
- `game.js` — questions, score, and button logic
