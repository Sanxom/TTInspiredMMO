using UnityEngine;

namespace GameName.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameName/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Info")]
        public string GameName = "[Game Name TBD]";
        public string Version = "0.1.0";

        [Header("Network Settings")]
        public string DefaultServerIP = "127.0.0.1";
        public ushort DefaultPort = 7777;
        public int MaxPlayersPerZone = 50;

        [Header("Combat Settings")]
        public float TurnDecisionTime = 8f;
        public int MaxPartySize = 4;
        public int MaxEnemiesInCombat = 8;

        [Header("Performance")]
        public int TargetFrameRate = 60;
        public bool EnableBurst = true;
        public bool EnableJobDebugging = false;
    }
}