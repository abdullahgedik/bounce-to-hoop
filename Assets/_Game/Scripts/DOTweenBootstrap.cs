using DG.Tweening;
using UnityEngine;

public static class DOTweenBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        DOTween.Init();
    }
}
