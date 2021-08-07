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
            "1.0.0";

        internal const string ModIdentifier = "RuleBookUnlocker";
        internal const string ModGuid = "com.Anreol." + ModIdentifier;

        public static RuleBookUnlocker instance;

        public void Awake()
        {
            instance = this;
#if DEBUG
            Debug.LogWarning("Running RuleBookUnlocker DEBUG build. PANIC!");
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