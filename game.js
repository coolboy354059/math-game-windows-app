'use strict';

function randomInt(min, max, random = Math.random) {
  return Math.floor(random() * (max - min + 1)) + min;
}

function makeAdditionQuestion(random = Math.random) {
  const answer = randomInt(1, 10, random);
  const first = randomInt(0, answer, random);
  const second = answer - first;
  return { text: `${first}+${second}=`, answer };
}

function makeSubtractionQuestion(random = Math.random) {
  const answer = randomInt(1, 10, random);
  const second = randomInt(0, 9, random);
  const first = second + answer;
  return { text: `${first}-${second}=`, answer };
}

function makeMultiplicationQuestion(random = Math.random) {
  const pairs = [];
  for (let first = 1; first <= 10; first += 1) {
    for (let second = 1; second <= 10; second += 1) {
      const answer = first * second;
      if (answer >= 1 && answer <= 10) {
        pairs.push({ text: `${first}X${second}=`, answer });
      }
    }
  }
  return pairs[randomInt(0, pairs.length - 1, random)];
}

function makeQuestion(random = Math.random) {
  const makers = [makeAdditionQuestion, makeSubtractionQuestion, makeMultiplicationQuestion];
  return makers[randomInt(0, makers.length - 1, random)](random);
}

if (typeof document !== 'undefined') {
  const homeScreen = document.querySelector('#home-screen');
  const gameScreen = document.querySelector('#game-screen');
  const playButton = document.querySelector('#play-button');
  const homeButton = document.querySelector('#home-button');
  const questionEl = document.querySelector('#question');
  const feedbackEl = document.querySelector('#feedback');
  const scoreEl = document.querySelector('#score');
  const keys = document.querySelectorAll('.key');

  let currentQuestion = makeQuestion();
  let score = 0;

  function showQuestion() {
    questionEl.textContent = currentQuestion.text;
  }

  function showFeedback(message, className) {
    feedbackEl.textContent = message;
    feedbackEl.className = `feedback ${className}`;
  }

  function startGame() {
    homeScreen.classList.add('hidden');
    gameScreen.classList.remove('hidden');
    currentQuestion = makeQuestion();
    showQuestion();
    showFeedback('', '');
  }

  function goHome() {
    gameScreen.classList.add('hidden');
    homeScreen.classList.remove('hidden');
  }

  function checkAnswer(selectedAnswer) {
    if (selectedAnswer === currentQuestion.answer) {
      score += 1;
      scoreEl.textContent = String(score);
      showFeedback('Correct! Great job!', 'correct');
      currentQuestion = makeQuestion();
      showQuestion();
    } else {
      showFeedback('Try again!', 'try-again');
    }
  }

  playButton.addEventListener('click', startGame);
  homeButton.addEventListener('click', goHome);

  keys.forEach((key) => {
    key.addEventListener('click', () => {
      checkAnswer(Number(key.dataset.answer));
    });
  });

  showQuestion();
}

if (typeof module !== 'undefined') {
  module.exports = {
    makeAdditionQuestion,
    makeSubtractionQuestion,
    makeMultiplicationQuestion,
    makeQuestion
  };
}
