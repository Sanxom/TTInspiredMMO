using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Marks entity as player-controlled
    /// </summary>
    public struct PlayerTag : IComponentData { }

    /// <summary>
    /// Marks entity as enemy AI
    /// </summary>
    public struct EnemyTag : IComponentData { }

    /// <summary>
    /// Marks entity as currently having active turn
    /// </summary>
    public struct ActiveTurnTag : IComponentData { }

    /// <summary>
    /// Marks entity as being in combat
    /// </summary>
    public struct InCombatTag : IComponentData { }

    /// <summary>
    /// Marks entity as defeated/dead
    /// </summary>
    public struct DefeatedTag : IComponentData { }

    /// <summary>
    /// Marks entity for destruction next frame
    /// </summary>
    public struct DestroyTag : IComponentData { }

    /// <summary>
    /// Marks combat instance as active
    /// </summary>
    public struct ActiveCombatTag : IComponentData { }

    /// <summary>
    /// Singleton tag - marks beginning of combat
    /// </summary>
    public struct BeginCombatTag : IComponentData { }

    /// <summary>
    /// Singleton tag - marks end of turn
    /// </summary>
    public struct TurnEndTag : IComponentData { }

    /// <summary>
    /// Singleton tag - AI is making decision
    /// </summary>
    public struct AITurnActiveTag : IComponentData { }

    /// <summary>
    /// Tag to mark that a combatant has already acted this round
    /// Removed at end of round to reset turn order
    /// </summary>
    public struct HasActedThisRoundTag : IComponentData { }
}