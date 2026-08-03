using SeriousGameBPD.Data;
using SeriousGameBPD.Safety;
using SeriousGameBPD.Presentation;
using UnityEngine;

namespace SeriousGameBPD.Core
{
    /// <summary>
    /// Legacy scene bootstrap. Disabled when AppFlow is present.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            // AppFlow owns the runtime UX now.
            if (FindFirstObjectByType<AppFlow>() != null) return;
            // AppFlow also auto-starts on Boot/SampleScene; skip duplicate.
        }
    }
}
