using System;
using System.Collections;
using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/CustomDirectionBadelineBoost")]
public class CustomDirectionBadelineBoost : BadelineBoost {
    public readonly float direction;
    
    public CustomDirectionBadelineBoost(EntityData data, Vector2 offset)
        : base(data.NodesWithPosition(offset), data.Bool("lockCamera", true), data.Bool(nameof (canSkip)), data.Bool(nameof (finalCh9Boost)), data.Bool(nameof (finalCh9GoldenBoost)), data.Bool(nameof (finalCh9Dialog))) {
        this.direction = float.Parse(data.String("direction", ((float)Math.PI / 2f).ToString()));
        
    }

    private static CustomDirectionBadelineBoost customLaunch = null;
    private static IEnumerator customBoostRoutine(On.Celeste.BadelineBoost.orig_BoostRoutine orig, BadelineBoost self, Player player) {
        if (self is CustomDirectionBadelineBoost custom) {
            customLaunch = custom;
        }
        yield return new SwapImmediately(orig(self, player));
        if (self is CustomDirectionBadelineBoost) {
            while (player.StateMachine.State == 10) {
                yield return null;
            }
        }
        customLaunch = null;
    }

    private static float setX(float old) {
        if (customLaunch != null) return (float) Math.Cos(customLaunch.direction)*330f;
        return old;
    }

    private static float setY(float old) {
        if (customLaunch != null) return (float) Math.Sin(customLaunch.direction)*-330f;
        return old;
    }
    private static Vector2 replaceSpeed(Vector2 old) {
        if (customLaunch != null) return new Vector2((float)(old.Length() * Math.Cos(customLaunch.direction)),
            (float)(old.Length() * -Math.Sin(customLaunch.direction)));
        return old;
    }
    private static void customBoostPos(ILContext ctx) {
        var cursor = new ILCursor(ctx);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchStfld<Vector2>("X"));
        cursor.EmitDelegate(setX);
        cursor.GotoNext(MoveType.Before, instr => instr.MatchStfld<Vector2>("Y"));
        cursor.EmitDelegate(setY);
    }
    private static void customBoostPos2(ILContext ctx) {
        var cursor = new ILCursor(ctx);
        cursor.GotoNext(MoveType.Before, instr => instr.MatchStfld<Player>("Speed"));
        cursor.EmitDelegate(replaceSpeed);
    }

    public static void Load() {
        On.Celeste.BadelineBoost.BoostRoutine += customBoostRoutine;
        IL.Celeste.Player.BadelineBoostLaunch += customBoostPos;
        IL.Celeste.Player.SummitLaunchUpdate += customBoostPos2;
    }

    public static void Unload() {
        On.Celeste.BadelineBoost.BoostRoutine -= customBoostRoutine;
        IL.Celeste.Player.BadelineBoostLaunch -= customBoostPos;
        IL.Celeste.Player.SummitLaunchUpdate += customBoostPos2;
    }
}