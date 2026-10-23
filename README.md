# Bounce to the Hoop

A 2D physics platformer where you play as a basketball. Drag to aim, release to launch, and bounce your way through each level into the hoop.

Originally made in a few hours for [Trijam #262](https://itch.io/jam/trijam-262), later revived and upgraded to Unity 6.

## How to Play

| Action | Mouse / Keyboard | Touch | Gamepad |
|---|---|---|---|
| Aim & launch | Click, drag back, release | Touch, drag back, release | — |
| Pause | `Esc` | — | `Start` |

Pull back like a slingshot: the further you drag, the stronger the throw (up to a limit). A dotted line shows where the ball will fly. There is a short cooldown between throws.

## Mechanics

- **Hoop:** drop the ball through it from above to finish the level.
- **Spikes:** deal damage; lose all your health and it's game over.
- **Enemies:** patrol back and forth along a fixed route and also deal damage. They can't be defeated, so time your throws to get past them.
- **Portals:** teleport the ball to a linked portal.
- **Sticky ball:** in some levels the ball turns green and sticks to surfaces instead of bouncing.

The game has 10 levels, plus a main menu and an end screen.

## Tech

- **Unity** 6000.6.6f1
- **URP** with the 2D Renderer and Pixel Perfect Camera
- **Cinemachine 3** for the follow camera
- **DOTween** for tweens
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
│   ├── Audios/        # Music
│   ├── Materials/     # 2D physics materials (bouncy / not bouncy)
│   ├── Prefabs/       # Ball, Hoop, Enemy, Spike, Portal, ...
│   ├── Scenes/        # Level_00 (menu) … Level_11 (end screen), Level_Test
│   ├── Scripts/
│   ├── Sprites/       # Pixel-art sprites, tiles and sprite atlas
│   └── Tilemaps/      # Brick rule tile (auto-tiling with slopes) and palette
├── DOTween/
├── InputActions/      # Project-wide input actions
└── Renderer/          # URP assets
```

## History

The original jam version (Unity 2022.3) is preserved under the [`trijam-original`](https://github.com/abdullahgedik/bounce-to-hoop/tree/trijam-original) tag.
