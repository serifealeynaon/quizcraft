const API = "/api/quiz";

// State
let currentQuiz = null;
let currentQuestionIndex = 0;
let selectedAnswers = {};
let timerInterval = null;
let timeLeft = 0;
let totalTimeTaken = 0;
let playerName = "";
let questionCount = 0;

// ── Navigation ──

function showView(name) {
  document.querySelectorAll(".view").forEach((v) => v.classList.remove("active"));
  document.getElementById(`view-${name}`).classList.add("active");

  document.querySelectorAll(".nav-btn").forEach((b) => b.classList.remove("active"));
  const navBtn = document.querySelector(`.nav-btn[data-view="${name}"]`);
  if (navBtn) navBtn.classList.add("active");

  if (name === "home") loadQuizzes();
  if (name === "create") initCreateForm();
}

document.querySelectorAll(".nav-btn").forEach((btn) => {
  btn.addEventListener("click", () => showView(btn.dataset.view));
});

// ── Home: Load Quizzes ──

async function loadQuizzes() {
  const container = document.getElementById("quiz-list");
  try {
    const res = await fetch(API);
    const quizzes = await res.json();

    if (quizzes.length === 0) {
      container.innerHTML = '<p class="muted">No quizzes yet. Create the first one!</p>';
      return;
    }

    container.innerHTML = quizzes
      .map(
        (q) => `
      <div class="quiz-card" onclick="playQuiz('${q.shareCode}')">
        <h3>${escHtml(q.title)}</h3>
        <p>${escHtml(q.description || "No description")}</p>
        <div class="quiz-meta">
          <span>📝 ${q.questionCount} questions</span>
          <span>👥 ${q.attemptCount} plays</span>
          <span class="quiz-code">${q.shareCode}</span>
        </div>
      </div>`
      )
      .join("");
  } catch {
    container.innerHTML = '<p class="muted">Could not load quizzes.</p>';
  }
}

// ── Create Quiz ──

function initCreateForm() {
  questionCount = 0;
  document.getElementById("quiz-title").value = "";
  document.getElementById("quiz-desc").value = "";
  document.getElementById("quiz-timer").value = "";
  document.getElementById("questions-container").innerHTML = "";
  addQuestion();
}

function addQuestion() {
  questionCount++;
  const container = document.getElementById("questions-container");
  const qIndex = questionCount;

  const block = document.createElement("div");
  block.className = "question-block";
  block.id = `question-${qIndex}`;
  block.innerHTML = `
    <div class="q-header">
      <span>Question ${qIndex}</span>
      <button class="remove-btn" onclick="this.closest('.question-block').remove()">✕</button>
    </div>
    <div class="form-group">
      <input type="text" class="q-text" placeholder="Enter your question" maxlength="500" />
    </div>
    ${[1, 2, 3, 4]
      .map(
        (i) => `
    <div class="option-row">
      <input type="radio" name="correct-${qIndex}" value="${i}" ${i === 1 ? "checked" : ""} />
      <input type="text" class="opt-text" placeholder="Option ${i}" maxlength="300" />
      ${i === 1 ? '<span class="correct-label">✓ Correct</span>' : ""}
    </div>`
      )
      .join("")}
  `;

  // Update correct label on radio change
  block.querySelectorAll('input[type="radio"]').forEach((radio) => {
    radio.addEventListener("change", () => {
      block.querySelectorAll(".correct-label").forEach((l) => l.remove());
      const label = document.createElement("span");
      label.className = "correct-label";
      label.textContent = "✓ Correct";
      radio.closest(".option-row").appendChild(label);
    });
  });

  container.appendChild(block);
}

async function submitQuiz() {
  const title = document.getElementById("quiz-title").value.trim();
  const desc = document.getElementById("quiz-desc").value.trim();
  const timer = document.getElementById("quiz-timer").value;

  if (!title) return alert("Please enter a quiz title.");

  const blocks = document.querySelectorAll(".question-block");
  if (blocks.length === 0) return alert("Add at least one question.");

  const questions = [];
  for (const block of blocks) {
    const text = block.querySelector(".q-text").value.trim();
    if (!text) return alert("All questions must have text.");

    const optInputs = block.querySelectorAll(".opt-text");
    const correctRadio = block.querySelector('input[type="radio"]:checked');
    const correctIndex = parseInt(correctRadio.value) - 1;

    const options = [];
    let hasEmpty = false;
    optInputs.forEach((input, i) => {
      const t = input.value.trim();
      if (!t) hasEmpty = true;
      options.push({ text: t, isCorrect: i === correctIndex });
    });

    if (hasEmpty) return alert("All options must have text.");
    questions.push({ text, options });
  }

  const body = {
    title,
    description: desc || null,
    timeLimitSeconds: timer ? parseInt(timer) : null,
    questions,
  };

  const btn = document.getElementById("create-btn");
  btn.disabled = true;
  btn.textContent = "Creating...";

  try {
    const res = await fetch(API, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      const err = await res.json();
      alert(err.error || "Failed to create quiz.");
      return;
    }

    const quiz = await res.json();
    showCreatedQuiz(quiz);
  } catch {
    alert("Could not connect to server.");
  } finally {
    btn.disabled = false;
    btn.textContent = "Create Quiz";
  }
}

function showCreatedQuiz(quiz) {
  const container = document.getElementById("questions-container");
  container.innerHTML = "";
  document.querySelector("#view-create .form-card").innerHTML = `
    <div class="share-box">
      <h2>🎉 Quiz Created!</h2>
      <p>Share this code with others:</p>
      <div class="code">${quiz.shareCode}</div>
      <button class="btn btn-primary" onclick="playQuiz('${quiz.shareCode}')">Play Now</button>
      <button class="btn btn-secondary mt-20" onclick="showView('create')">Create Another</button>
    </div>
  `;
}

// ── Join Quiz ──

function joinQuiz() {
  const code = document.getElementById("join-code").value.trim();
  if (!code) return alert("Please enter a quiz code.");
  playQuiz(code);
}

// ── Play Quiz ──

async function playQuiz(code) {
  try {
    const res = await fetch(`${API}/${code}`);
    if (!res.ok) {
      alert("Quiz not found. Check the code and try again.");
      return;
    }

    currentQuiz = await res.json();
    currentQuestionIndex = 0;
    selectedAnswers = {};
    totalTimeTaken = 0;

    // Ask player name first
    showView("play");
    document.getElementById("play-container").innerHTML = `
      <div class="name-input-section">
        <h2>🧠 ${escHtml(currentQuiz.title)}</h2>
        <p class="muted">${escHtml(currentQuiz.description || "")}</p>
        <p class="muted mt-20">${currentQuiz.questions.length} questions${currentQuiz.timeLimitSeconds ? ` • ${currentQuiz.timeLimitSeconds}s per question` : ""}</p>
        <div class="form-group mt-20">
          <input type="text" id="player-name" placeholder="Enter your name" maxlength="100" />
        </div>
        <button class="btn btn-primary" onclick="startQuiz()">Start Quiz</button>
      </div>
    `;
  } catch {
    alert("Could not load quiz.");
  }
}

function startQuiz() {
  playerName = document.getElementById("player-name").value.trim();
  if (!playerName) return alert("Please enter your name.");
  renderQuestion();
}

function renderQuestion() {
  const container = document.getElementById("play-container");
  const q = currentQuiz.questions[currentQuestionIndex];
  const total = currentQuiz.questions.length;
  const progress = ((currentQuestionIndex + 1) / total) * 100;

  let timerHtml = "";
  if (currentQuiz.timeLimitSeconds) {
    timeLeft = currentQuiz.timeLimitSeconds;
    timerHtml = `<div class="timer-display" id="timer">${timeLeft}</div>`;
  }

  container.innerHTML = `
    <div class="play-header">
      <h2>${escHtml(currentQuiz.title)}</h2>
      <div class="progress-bar"><div class="progress-fill" style="width: ${progress}%"></div></div>
    </div>
    ${timerHtml}
    <div class="question-card">
      <div class="question-number">Question ${currentQuestionIndex + 1} of ${total}</div>
      <h3>${escHtml(q.text)}</h3>
      <div class="options-list">
        ${q.options
          .map(
            (o) => `
          <button class="option-btn ${selectedAnswers[q.id] === o.id ? "selected" : ""}"
                  onclick="selectOption(${q.id}, ${o.id}, this)">
            ${escHtml(o.text)}
          </button>`
          )
          .join("")}
      </div>
    </div>
    <div class="play-nav">
      <button class="btn btn-secondary" onclick="prevQuestion()" ${currentQuestionIndex === 0 ? "disabled" : ""}>← Back</button>
      ${
        currentQuestionIndex === total - 1
          ? '<button class="btn btn-primary" onclick="finishQuiz()">Finish Quiz</button>'
          : '<button class="btn btn-primary" onclick="nextQuestion()">Next →</button>'
      }
    </div>
  `;

  // Start timer if applicable
  if (currentQuiz.timeLimitSeconds) {
    clearInterval(timerInterval);
    timerInterval = setInterval(() => {
      timeLeft--;
      totalTimeTaken++;
      const timerEl = document.getElementById("timer");
      if (timerEl) {
        timerEl.textContent = timeLeft;
        if (timeLeft <= 5) timerEl.classList.add("urgent");
      }
      if (timeLeft <= 0) {
        clearInterval(timerInterval);
        nextQuestion();
      }
    }, 1000);
  }
}

function selectOption(questionId, optionId, btn) {
  selectedAnswers[questionId] = optionId;
  btn.closest(".options-list").querySelectorAll(".option-btn").forEach((b) => b.classList.remove("selected"));
  btn.classList.add("selected");
}

function nextQuestion() {
  clearInterval(timerInterval);
  if (currentQuestionIndex < currentQuiz.questions.length - 1) {
    currentQuestionIndex++;
    renderQuestion();
  } else {
    finishQuiz();
  }
}

function prevQuestion() {
  clearInterval(timerInterval);
  if (currentQuestionIndex > 0) {
    currentQuestionIndex--;
    renderQuestion();
  }
}

async function finishQuiz() {
  clearInterval(timerInterval);

  const answers = currentQuiz.questions.map((q) => ({
    questionId: q.id,
    selectedOptionId: selectedAnswers[q.id] || 0,
  }));

  const body = {
    playerName,
    timeTakenSeconds: totalTimeTaken,
    answers,
  };

  try {
    const res = await fetch(`${API}/${currentQuiz.shareCode}/submit`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      alert("Failed to submit quiz.");
      return;
    }

    const result = await res.json();
    showResult(result);
  } catch {
    alert("Could not submit quiz.");
  }
}

function showResult(result) {
  showView("result");

  const pctClass = result.percentage >= 70 ? "high" : result.percentage >= 40 ? "medium" : "low";

  document.getElementById("result-container").innerHTML = `
    <div class="result-card">
      <h2>${escHtml(result.quizTitle)}</h2>
      <div class="score-circle ${pctClass}">${result.percentage}%</div>
      <p><strong>${result.score}</strong> out of <strong>${result.totalQuestions}</strong> correct</p>
      <p class="muted">Time: ${result.timeTakenSeconds}s</p>
      <div class="mt-20">
        <button class="btn btn-primary" onclick="showLeaderboard('${currentQuiz.shareCode}')">🏆 Leaderboard</button>
        <button class="btn btn-secondary" onclick="playQuiz('${currentQuiz.shareCode}')">🔄 Try Again</button>
      </div>
    </div>

    <h3>📝 Answer Details</h3>
    ${result.details
      .map(
        (d) => `
      <div class="detail-item ${d.isCorrect ? "correct" : "wrong"}">
        <div class="q-text">${d.isCorrect ? "✅" : "❌"} ${escHtml(d.questionText)}</div>
        <div class="answers">
          <span>Your answer: <strong>${escHtml(d.selectedAnswer)}</strong></span>
          ${!d.isCorrect ? `<span>Correct: <strong>${escHtml(d.correctAnswer)}</strong></span>` : ""}
        </div>
      </div>`
      )
      .join("")}

    <div class="text-center mt-20">
      <button class="btn btn-secondary" onclick="showView('home')">← Back to Home</button>
    </div>
  `;
}

// ── Leaderboard ──

async function showLeaderboard(code) {
  showView("leaderboard");
  const container = document.getElementById("leaderboard-container");
  container.innerHTML = '<p class="muted">Loading leaderboard...</p>';

  try {
    const res = await fetch(`${API}/${code}/leaderboard`);
    const entries = await res.json();

    if (entries.length === 0) {
      container.innerHTML = '<p class="muted">No entries yet.</p>';
      return;
    }

    container.innerHTML = `
      <h2>🏆 Leaderboard</h2>
      <table class="leaderboard-table">
        <thead>
          <tr>
            <th>Rank</th>
            <th>Player</th>
            <th>Score</th>
            <th>%</th>
            <th>Time</th>
          </tr>
        </thead>
        <tbody>
          ${entries
            .map(
              (e) => `
            <tr>
              <td class="${e.rank <= 3 ? `rank-${e.rank}` : ""}">${e.rank <= 3 ? ["🥇", "🥈", "🥉"][e.rank - 1] : e.rank}</td>
              <td>${escHtml(e.playerName)}</td>
              <td>${e.score}/${e.totalQuestions}</td>
              <td>${e.percentage}%</td>
              <td>${e.timeTakenSeconds}s</td>
            </tr>`
            )
            .join("")}
        </tbody>
      </table>
      <div class="text-center mt-20">
        <button class="btn btn-secondary" onclick="showView('home')">← Back to Home</button>
      </div>
    `;
  } catch {
    container.innerHTML = '<p class="muted">Could not load leaderboard.</p>';
  }
}

// ── Helpers ──

function escHtml(str) {
  const div = document.createElement("div");
  div.textContent = str;
  return div.innerHTML;
}

// ── Init ──
loadQuizzes();
