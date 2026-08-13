using SeriousGameBPD.Narrative;
using SeriousGameBPD.Skills;
using UnityEngine;

namespace SeriousGameBPD.World
{
    public class HouseContext : MonoBehaviour
    {
        public static HouseContext Instance { get; private set; }

        public WorldClock Clock;
        public DayDirector Director;
        public FPSController Player;
        public PhoneController Phone;
        public DoorController Door;
        public ThoughtOverlay Thoughts;
        public HouseHud Hud;
        public SkillController Skills;
        public Light Sun;
        public AffectPresentation AffectFx;
        public PhoneChatUI PhoneChat;
        public EventRunner Events;
        public Interactable Laptop;

        public bool SessionAffectHigh
        {
            get
            {
                var session = Core.GameSession.Instance;
                return session != null && session.Affect != null && session.Affect.IsHighImpulsivity;
            }
        }

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
