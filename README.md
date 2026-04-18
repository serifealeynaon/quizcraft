# 🧠 QuizCraft — Create, Share & Play Quizzes

A full-stack quiz platform where users can create custom quizzes, share them via unique links, and track scores. Built with ASP.NET Core and Entity Framework Core.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12-239120?style=flat-square&logo=csharp&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-blue?style=flat-square)

---

## ✨ Features

- **Create Quizzes** — Build custom quizzes with multiple-choice questions
- **Share via Link** — Each quiz gets a unique shareable code
- **Play & Score** — Take quizzes and get instant results with correct answers
- **Leaderboard** — See top scores for each quiz
- **Timer Mode** — Optional countdown timer per question
- **Quiz Dashboard** — View all your created quizzes and their statistics
- **RESTful API** — Clean API design, easy to integrate with any frontend

## 🏗️ Architecture

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   Frontend   │────▶│  ASP.NET Core│────▶│   SQLite     │
│  (Vanilla JS)│◀────│  Web API     │◀────│   Database   │
└──────────────┘     └──────────────┘     └──────────────┘
```

## 📦 Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core 8.0 |
| Language | C# 12 |
| ORM | Entity Framework Core |
| Database | SQLite |
| Frontend | Vanilla JS + HTML/CSS |
| Testing | xUnit |

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher

### Installation

```bash
# Clone the repository
git clone https://github.com/serifealeynaon/quizcraft.git
cd quizcraft

# Restore dependencies
dotnet restore

# Create the database
dotnet ef database update

# Run the application
dotnet run
```

The app will be running at `https://localhost:5001`

> **Note:** If you don't have EF tools installed, run:
> `dotnet tool install --global dotnet-ef`

## 📁 Project Structure

```
QuizCraft/
├── Controllers/        # API endpoints
│   ├── QuizController.cs
│   └── LeaderboardController.cs
├── Models/             # Data models
│   ├── Quiz.cs
│   ├── Question.cs
│   ├── Option.cs
│   └── QuizAttempt.cs
├── DTOs/               # Data transfer objects
│   ├── CreateQuizDTO.cs
│   └── QuizResponseDTO.cs
├── Services/           # Business logic
│   ├── IQuizService.cs
│   └── QuizService.cs
├── Data/               # Database context
│   └── AppDbContext.cs
├── wwwroot/            # Static frontend files
│   ├── index.html
│   ├── css/
│   │   └── style.css
│   └── js/
│       └── app.js
├── Program.cs
├── QuizCraft.csproj
├── appsettings.json
└── README.md
```

## 🔌 API Endpoints

### Quizzes

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/quiz` | Create a new quiz |
| `GET` | `/api/quiz/{code}` | Get quiz by share code |
| `GET` | `/api/quiz` | List all quizzes |
| `DELETE` | `/api/quiz/{id}` | Delete a quiz |

### Playing

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/quiz/{code}/submit` | Submit quiz answers |
| `GET` | `/api/quiz/{code}/leaderboard` | Get quiz leaderboard |

### Example: Create a Quiz

**Request:**
```json
POST /api/quiz
{
  "title": "JavaScript Fundamentals",
  "description": "Test your JS knowledge",
  "timeLimitSeconds": 30,
  "questions": [
    {
      "text": "What does '===' do in JavaScript?",
      "options": [
        { "text": "Assigns a value", "isCorrect": false },
        { "text": "Compares value only", "isCorrect": false },
        { "text": "Compares value and type", "isCorrect": true },
        { "text": "None of the above", "isCorrect": false }
      ]
    }
  ]
}
```

**Response:**
```json
{
  "id": 1,
  "title": "JavaScript Fundamentals",
  "shareCode": "js-fund-a1b2",
  "questionCount": 1,
  "createdAt": "2025-03-15T10:30:00Z"
}
```

## 💡 Example Use Cases

- 📚 Teachers creating quizzes for students
- 💼 Companies testing candidate knowledge
- 🎮 Friends challenging each other
- 📖 Self-study and revision tool

## 🗺️ Roadmap

- [x] Quiz CRUD operations
- [x] Shareable quiz codes
- [x] Score calculation & leaderboard
- [x] Timer per question
- [x] Interactive frontend
- [ ] User authentication (JWT)
- [ ] Quiz categories & tags
- [ ] Image support in questions
- [ ] Export results to CSV
- [ ] Real-time multiplayer mode
- [ ] Docker support

## 🤝 Contributing

Contributions are welcome! Please open an issue first to discuss what you'd like to change.

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  Built by <a href="https://github.com/serifealeynaon">Serife ON</a> — making learning interactive.
</p>
