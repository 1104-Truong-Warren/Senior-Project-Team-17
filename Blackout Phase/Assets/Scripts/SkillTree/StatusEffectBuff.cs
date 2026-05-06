// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// buffs for unit skills
// Weijun

using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class StatusEffectBuff
{
    public string buffEffectName; // name of the buff skill

    public int remainingBuffTurns; // how long does the buff last

    public bool expireAtTurnEnd; // buff stops at this turn's end

    public BuffEffectTimer tickTimer; // = BuffEffectTimer.EndOfUserTurn; // timer access set to end of Unit's turn

    public List<StatsModifier> modifiers = new List<StatsModifier>(); // list to store the buffers

    public UnitCore unit; // access the unit
}