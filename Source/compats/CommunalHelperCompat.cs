using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.CommunalHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.KyfexHelper;

public class CommunalHelperCompat {
    public static void Load() {
        BubbleRedirector.checkCommunalHelperBoosters = true;
    }
    public static List<Hook> hooks = new();
    public static void LoadHooks() {
        hooks.Add(new Hook(typeof(DreamBoosterSegment).GetMethod("RedDashUpdateBefore", BindingFlags.Instance | BindingFlags.NonPublic),
            changeLength));
        hooks.Add(new Hook(typeof(SegmentBooster).GetMethod("RedDashUpdateBefore", BindingFlags.Instance | BindingFlags.NonPublic),
            changeLength2));
        On.Celeste.Booster.PlayerReleased += oneTime;
        CloneBooster.Load();
        CloneDreamBooster.LoadHooks();
    }
    public static void UnloadHooks() {
        foreach (var hook in hooks) hook.Dispose();
        On.Celeste.Booster.PlayerReleased -= oneTime;
        CloneBooster.Unload();
        CloneDreamBooster.UnloadHooks();
    }

    private static void oneTime(On.Celeste.Booster.orig_PlayerReleased orig, Booster self) {
        orig(self);
        if (self is CloneBooster) {
            self.respawnTimer = float.PositiveInfinity;
        }
    }

    private static int? changeLength(Func<DreamBoosterSegment, Player, int?> orig, DreamBoosterSegment self, Player player) {
        var oldLength = self.Length;
        self.Length = float.PositiveInfinity;
        var toReturn = orig(self, player);
        self.Length = oldLength;
        return toReturn;
    }
    private static int? changeLength2(Func<SegmentBooster, Player, int?> orig, SegmentBooster self, Player player) {
        var oldLength = self.Length;
        self.Length = float.PositiveInfinity;
        var toReturn = orig(self, player);
        self.Length = oldLength;
        return toReturn;
    }

    // private static FieldInfo spiralBoosterClockwise = typeof(SpiralBooster).GetField("clockwise", BindingFlags.Instance | BindingFlags.NonPublic);
    private static FieldInfo curvedBoosterSpeed = typeof(CurvedBooster).GetField("endingSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
    public static void DerailBooster(Entity booster, Vector2 position, Player player) {
        if (booster is DreamBoosterCurve dream) {
            booster.Scene.Add(new CloneDreamBooster(position));
            player.Speed.Normalize();
            player.Speed *= dream.EndingSpeed.Length();
        }
        if (booster is CurvedBooster curved) {
            booster.Scene.Add(new CloneBooster(position, true));
            player.Speed.Normalize();
            player.Speed *= ((Vector2)curvedBoosterSpeed.GetValue(curved)).Length();
        }
        if (booster is SpiralBooster spiral) {
            // booster.Scene.Add(new CloneBooster(position, true, 
            //     CommunalHelperGFX.SpriteBank.Create((bool)spiralBoosterClockwise.GetValue(spiral) ? "clockwiseSpiralBooster" : "counterclockwiseSpiralBooster")));
        }
    }
}