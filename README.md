# Minimal Edit Distance — Turkish Dictionary

A Windows desktop application written in C# that finds the closest matching words in a Turkish dictionary using the **Minimal Edit Distance** (Levenshtein distance) algorithm.

---

## What It Does

When you type a word — whether misspelled or just uncertain — the app searches through a Turkish vocabulary list and returns the word(s) that require the fewest edits (insertions, deletions, or substitutions) to match your input. This is the same core idea behind spell-checkers.

**Example:**
- You type: `kitab`
- The app finds: `kitap` (1 substitution away)

---

## How Minimal Edit Distance Works

Minimal Edit Distance (also known as Levenshtein distance) measures how different two strings are by counting the minimum number of single-character operations needed to turn one into the other:

| Operation | Example |
|-----------|---------|
| **Insert** | `kedi` → `kedii` |
| **Delete** | `araba` → `arab` |
| **Substitute** | `masa` → `mase` |

The algorithm compares your input against every word in the dictionary and returns the one(s) with the lowest score.

---

## Features

- Turkish-language dictionary with UTF-8 support (handles `ç`, `ğ`, `ı`, `ö`, `ş`, `ü`)
- Windows Forms GUI — no command line needed
- Sample vocabulary file included for quick testing
- Full vocabulary file (`vocabulary_tr_utf8.txt`) with a broad word list

---

## Requirements

- Windows OS
- [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework) (version compatible with the `.csproj` settings)
- Visual Studio (to open and build the solution)

---

## Getting Started

1. **Clone the repository**
   ```
   git clone https://github.com/MuradGhzda/Minimal-Edit-Distance-Turkish-Dictionary.git
   ```

2. **Open the solution**  
   Double-click `MED.sln` to open it in Visual Studio.

3. **Build the project**  
   Go to **Build → Build Solution** (or press `Ctrl + Shift + B`).

4. **Run the app**  
   Press `F5` or click **Start**. The application window will open.

5. **Use it**  
   Type a Turkish word into the input field and the app will display the closest matching word(s) from the dictionary.

---

## Project Structure

```
├── Form1.cs                  # Main application window and UI logic
├── Program.cs                # Entry point
├── MED.csproj                # Project configuration
├── MED.sln                   # Visual Studio solution file
├── App.config                # Application configuration
├── vocabulary_tr_utf8.txt    # Full Turkish vocabulary (UTF-8)
├── sample_vocabulary.txt     # Smaller word list for testing
└── .gitignore
```

---

## Language & Platform

- **Language:** C#
- **Framework:** .NET Framework (Windows Forms)
- **Platform:** Windows

---

## License

This project does not currently specify a license. Please contact the author before using it in other projects.
