#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FullscreenGameView
{
    private static readonly Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");

    private static readonly PropertyInfo showToolbarProperty =
        gameViewType.GetProperty("showToolbar", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly object falseObject = false; // Only box once. This is a matter of principle.
    private static EditorWindow _instance;

    private static readonly bool fullscreen = true;

    static FullscreenGameView()
    {
        EditorApplication.playModeStateChanged -= ToggleFullScreen;
        if (!fullscreen)
            return;
        EditorApplication.playModeStateChanged += ToggleFullScreen;
    }

    [MenuItem("Window/General/Game (Fullscreen) %#&2", priority = 2)]
    public static void Toggle()
    {
        ToggleFullScreen(PlayModeStateChange.EnteredPlayMode);
    }

    public static void ToggleFullScreen(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange == PlayModeStateChange.EnteredEditMode || playModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            CloseGameWindow();
            return;
        }

        if (gameViewType == null)
        {
            Debug.LogError("GameView type not found.");
            return;
        }

        if (showToolbarProperty == null)
        {
            Debug.LogWarning("GameView.showToolbar property not found.");
        }

        switch (playModeStateChange)
        {
            case PlayModeStateChange.ExitingPlayMode:
                return;
            case PlayModeStateChange.EnteredPlayMode: //Used to toggle
                if (CloseGameWindow())
                    return;
                break;
        }

        _instance = (EditorWindow)ScriptableObject.CreateInstance(gameViewType);

        showToolbarProperty?.SetValue(_instance, falseObject);

        var desktopResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        var fullscreenRect = new Rect(Vector2.zero, desktopResolution);
        _instance.ShowPopup();
        _instance.position = fullscreenRect;
        _instance.Focus();
    }

    private static bool CloseGameWindow()
    {
        if (_instance != null)
        {
            _instance.Close();
            _instance = null;
            return true;
        }

        return false;
    }
}
#endif