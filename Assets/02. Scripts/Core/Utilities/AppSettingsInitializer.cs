using UnityEngine;
using VContainer.Unity;

public class AppSettingsInitializer : IInitializable
{
    public void Initialize()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}