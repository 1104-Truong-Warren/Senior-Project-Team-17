//
// Weijun

using UnityEngine;

public abstract class UnitCore : MonoBehaviour
{
    // the abstract skillcore for skillExecutor 
    public abstract OverlayTile1 CurrentTile { get; } // get tile

    // get stats
    public abstract int CurrentHP { get; } 
    public abstract int MaxHP { get; }

    // get combat stats
    public abstract int HitRate { get; }
    public abstract int EvasionRate { get; }
    public abstract int AttackRange { get; }
    public abstract int BaseAttack { get; }

    // taking dmg
    public abstract void TakeDamage(int dmg);

    // is the unit dead?
    public virtual bool IsDead()
    {
        return CurrentHP <= 0;
    }
}
