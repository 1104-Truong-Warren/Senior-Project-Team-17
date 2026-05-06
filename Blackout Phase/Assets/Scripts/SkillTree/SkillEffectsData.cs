// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// used to see how scriptableObject works URL: https://www.youtube.com/watch?v=cy49zMBZvhg
// data for hold ing skill effects
// Weijun

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillEffectsData
{
    public string buffEffectName; // name of the buff skill

    public EffectTargetType targetType; // self/target

    public int durationTurnsLeft; // how long does the buff last

    public BuffEffectTimer tickTimer = BuffEffectTimer.EndOfUserTurn; // timer access set to end of Unit's turn

    public List<StatsModifier> modifiers = new List<StatsModifier>(); // list to store the buffers
}
