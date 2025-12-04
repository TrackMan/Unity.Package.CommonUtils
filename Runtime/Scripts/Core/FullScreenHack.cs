using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public static class FullScreenHack
    {
        public static async Task RunFullScreenHackAsync()
        {
            await Awaiters.EndOfFrame;

            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("Switched to windowed mode");

            await Awaiters.EndOfFrame;
            await Awaiters.EndOfFrame;

            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("Restored full screen window mode");
            await Awaiters.EndOfFrame;
        }
    }
}