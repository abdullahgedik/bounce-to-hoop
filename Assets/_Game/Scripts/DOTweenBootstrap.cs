using DG.Tweening;
using UnityEngine;

// DOTween sets itself up the first time a tween is created: it loads its settings and allocates
// its tween pools. The game's first tween is the delayed level change after a basket, so that
// cost landed on the scoring frame as a visible stutter. Initializing before the first scene
// loads moves it into the loading time instead.
public static class DOTweenBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        DOTween.Init();
    }
}
