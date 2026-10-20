# Bounce to the Hoop

A 2D physics platformer where you play as a basketball. Drag to aim, release to launch, and bounce your way through each level into the hoop.

Originally made in a few hours for [Trijam](https://itch.io/jams), later revived and upgraded to Unity 6.

## How to Play

| Action | Mouse / Keyboard | Touch | Gamepad |
|---|---|---|---|
| Aim & launch | Click, drag back, release | Touch, drag back, release | — |
| Pause | `Esc` | — | `Start` |

Pull back like a slingshot: the further you drag, the stronger the throw (up to a limit). There is a short cooldown between throws.

## Mechanics

- **Hoop:** reach it to finish the level.
- **Spikes & enemies:** deal damage; lose all your health and it's game over.
- **Patrolling enemies:** walk back and forth along a fixed route.
- **Portals:** teleport the ball to a linked portal.
- **Sticky ball:** in some levels the ball turns green and sticks to surfaces instead of bouncing.

The game has 10 levels, plus a main menu and an end screen.

## Tech

- **Unity** 6000.6.6f1
- **URP** with the 2D Renderer
- **Cinemachine 3** for the follow camera
- **Input System** with project-wide actions (`Gameplay`: Throw, Point, Pause)
- **TextMesh Pro** (bundled with uGUI)

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/abdullahgedik/bounce-to-hoop.git
   ```
2. Open the folder with Unity **6000.6.6f1** (via Unity Hub).
3. Open `Assets/_Game/Scenes/Level_00.unity` (main menu) and press Play.

## Project Structure

```
Assets/
├── _Game/
│   ├── Animations/
│   ├── Audios/        # Music + simple audio manager
│   ├── Materials/     # 2D physics materials (bouncy / not bouncy)
│   ├── Prefabs/       # Ball, Hoop, Enemy, Spike, Portal, ...
│   ├── Scenes/        # Level_00 (menu) … Level_11 (end screen), Level_Test
│   ├── Scripts/
│   ├── Sprites/
│   └── Tilemaps/
├── InputActions/      # Project-wide input actions
└── Renderer/          # URP assets
```

## History

The original jam version (Unity 2022.3) is preserved under the [`trijam-original`](https://github.com/abdullahgedik/bounce-to-hoop/tree/trijam-original) tag.
