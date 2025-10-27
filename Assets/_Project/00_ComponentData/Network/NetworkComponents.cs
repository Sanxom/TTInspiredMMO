using Unity.Entities;
using Unity.NetCode;

namespace GameName.ComponentData
{
    /// <summary>
    /// Client input command for ability casting
    /// </summary>
    public struct CastAbilityCommand : ICommandData
    {
        public NetworkTick Tick { get; set; }

        public int AbilitySlotIndex;  // Which hotbar slot (0-9)
        public int TargetIndex;       // Target position index
        public bool Execute;          // Button pressed this frame
    }

    /// <summary>
    /// RPC: Server notifies clients of ability result
    /// </summary>
    public struct AbilityResultRpc : IRpcCommand
    {
        public Entity Caster;
        public Entity Target;
        public int AbilityId;
        public int DamageDealt;
        public bool WasCritical;
    }

    /// <summary>
    /// RPC: Server notifies client of failed action
    /// </summary>
    public struct ActionFailedRpc : IRpcCommand
    {
        public FailureReason Reason;
    }

    public enum FailureReason : byte
    {
        InsufficientMana = 0,
        OnCooldown = 1,
        InvalidTarget = 2,
        OutOfRange = 3,
        NotYourTurn = 4
    }

    /// <summary>
    /// Network ownership component
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct NetworkOwnerComponent : IComponentData
    {
        [GhostField] public int NetworkId;
        [GhostField] public bool IsLocalPlayer;
    }
}