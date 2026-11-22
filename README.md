# Math Game Windows App

A small Windows math game app made from Teo's Figma design.

The home screen says **PLAY THE MATH GAME NOW!!!** and has a big PLAY button. The game screen shows a math question and clean number buttons from 1 to 10.

Questions use addition, subtraction, and small multiplication. Every answer is from 1 to 10, so it can be picked with the number buttons.

## Run it

Install Node.js first, then run:

```bash
npm install
npm start
```

## Build the Windows app

Run:

```bash
npm run dist
```

Electron Builder will make the Windows installer in the `dist` folder.

## Files

- `main.js` — opens the Electron window
- `index.html` — the two app screens
- `style.css` — the layout and colors from the design
- `game.js` — questions, score, and button logic
- `package.json` — app scripts and Windows build settings
