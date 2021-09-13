using BepInEx;
using UnityEngine;

namespace RuleBookEditor
{
    [BepInPlugin(ModGuid, ModIdentifier, ModVer)]
    public class RuleBookUnlocker : BaseUnityPlugin
    {
        internal const string ModVer =
#if DEBUG
            "9999." +
#endif
            "1.0.1";

        internal const string ModIdentifier = "RuleBookUnlocker";
        internal const string ModGuid = "com.Anreol." + ModIdentifier;

        public static RuleBookUnlocker instance;

        public void Awake()
        {
            instance = this;
#if DEBUG
            Debug.LogWarning("Running RuleBookUnlocker DEBUG build. PANIC!");
            //you can connect to yourself with a second instance of the game by hosting a private game with one and opening the console on the other and typing connect localhost:7777
            //Debug.LogWarning("Setting up localhost:7777");
            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };
#endif
            Init();
        }

        public void Start()
        {
            InitLate();
        }

        private void Init()
        {
            RuleBookEditor.Config.Initialize();
        }

        private void InitLate()
        {
            Rulebook.Initialize();
        }
    }
}