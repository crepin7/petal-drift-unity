# 🌸 Petal Drift

A dreamy one-touch petal drifting game built with **Unity**.

## 🎮 Gameplay

- **Touch left** half of screen — drift left
- **Touch right** half — drift right
- **Release** — gentle float upward
- **Collect flowers** to score and bounce higher
- **Avoid dark clouds** — they push you down
- **Wind zones** change your direction

Original concept by crepin7, ported from Godot 4.3 to Unity.

## 🏗 Project Setup

### Opening in Unity Editor

1. Install **Unity 2022.3 LTS** with **Android Build Support** (IL2CPP)
2. Open the project folder in Unity Hub
3. Let Unity import and compile
4. Open `Assets/Scenes/MainMenu.unity`
5. Press Play to test in Editor

### Tags Required

Create these tags (Edit > Project Settings > Tags and Layers):
- `Player`
- `Flower`
- `Hazard`
- `WindZone`

### Scenes

The `.unity` scene files must be created in the Unity Editor:

**MainMenu scene** (Assets/Scenes/MainMenu.unity):
- Main Camera (orthographic, 1080×1920)
- EventSystem
- Background (sprite or GameObject with Background.cs)
- Canvas (Screen Space - Overlay)
  - TitleLabel (TextMeshPro - "Petal Drift")
  - StartLabel (TextMeshPro - "Tap to Start")
  - HighScoreLabel (TextMeshPro - "Best: 0")
  - Instructions (TextMeshPro - touch controls)
- GameObject with MainMenu.cs

**Game scene** (Assets/Scenes/Game.unity):
- Main Camera (orthographic, size=5, 1080×1920)
- EventSystem
- Background (GameObject with Background.cs)
- Player (GameObject with Player.cs + Rigidbody2D + CircleCollider2D)
- FlowerSpawner (empty GameObject)
- HazardSpawner (empty GameObject)
- WindSpawner (empty GameObject)
- Canvas (Screen Space - Overlay)
  - ScoreText (TextMeshPro, top-left)
  - ComboText (TextMeshPro, center)
  - GameOverPanel (Panel)
    - FinalScoreLabel (TextMeshPro)
    - HighScoreLabel (TextMeshPro)
    - TapToRestartLabel (TextMeshPro)
- GameUI.cs on the Canvas
- Persistent GameObject with GameManager.cs (or DontDestroyOnLoad)

## 🚀 CI/CD Build (GitHub Actions)

The project uses [game-ci](https://game.ci/) for automated Android builds.

### Step 1: Get your Unity license

On your machine with Unity installed:
```bash
# macOS
cat ~/Library/Unity/Editor/Unity_lic.ulf

# Windows
type %USERPROFILE%\AppData\Roaming\Unity\Editor\Unity_lic.ulf
```

### Step 2: Add secret to GitHub

1. Go to your repo → **Settings → Secrets and variables → Actions**
2. Add **`UNITY_LICENSE`** with the content of your `.ulf` file
3. Push to `main` — the APK is built automatically!

### Manual trigger

Go to **Actions → "Build Petal Drift APK" → Run workflow**.

## 📁 Project Structure

```
petal-drift-unity/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs       — Global state, score, persistence
│   │   ├── Player.cs            — Petal movement & touch input
│   │   ├── Flower.cs            — Collectible flowers
│   │   ├── Hazard.cs            — Dark cloud obstacles
│   │   ├── WindZone.cs          — Wind currents
│   │   ├── Background.cs        — Parallax starfield
│   │   ├── MainMenu.cs          — Title screen
│   │   ├── GameUI.cs            — In-game HUD & game over
│   │   └── Editor/
│   │       └── BuildScript.cs   — CI build automation
│   └── Scenes/
│       ├── MainMenu.unity       — Title screen
│       └── Game.unity           — Main gameplay
├── Packages/manifest.json
├── ProjectSettings/
└── .github/workflows/build.yml
```

## 📜 License

MIT
