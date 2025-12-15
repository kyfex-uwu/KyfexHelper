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
        On.Celeste.Level.Update += cleanInfLength;
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

    private static void cleanInfLength(On.Celeste.Level.orig_Update orig, Level self) {
        orig(self);
        infLength.RemoveWhere(entityRef => !entityRef.TryGetTarget(out var _));
    }
    private static int? changeLength(Func<DreamBoosterSegment, Player, int?> orig, DreamBoosterSegment self, Player player) {
        var oldLength = self.Length;
        foreach (var entityRef in infLength) {
            if (entityRef.TryGetTarget(out var booster) && booster == self) {
                self.Length = float.PositiveInfinity;
                break;
            }
        }
        var toReturn = orig(self, player);
        self.Length = oldLength;
        return toReturn;
    }
    private static int? changeLength2(Func<SegmentBooster, Player, int?> orig, SegmentBooster self, Player player) {
        var oldLength = self.Length;
        foreach (var entityRef in infLength) {
            if (entityRef.TryGetTarget(out var booster) && booster == self) {
                self.Length = float.PositiveInfinity;
                break;
            }
        }
        var toReturn = orig(self, player);
        self.Length = oldLength;
        return toReturn;
    }

    private static HashSet<WeakReference<Entity>> infLength = new();
    // private static FieldInfo spiralBoosterClockwise = typeof(SpiralBooster).GetField("clockwise", BindingFlags.Instance | BindingFlags.NonPublic);
    private static FieldInfo curvedBoosterSpeed = typeof(CurvedBooster).GetField("endingSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
    public static void DerailBooster(Entity booster, Vector2 position, Player player) {
        if (booster is DreamBoosterCurve dreamCurved) {
            booster.Scene.Add(new CloneDreamBooster(position));
            player.Speed.Normalize();
            player.Speed *= dreamCurved.EndingSpeed.Length();
        }
        if (booster is CurvedBooster curved) {
            booster.Scene.Add(new CloneBooster(position, true));
            player.Speed.Normalize();
            player.Speed *= ((Vector2)curvedBoosterSpeed.GetValue(curved)).Length();
        }

        if (booster is DreamBoosterSegment || booster is SegmentBooster) {
            infLength.Add(new WeakReference<Entity>(booster));
        }

        if (booster is SpiralBooster spiral) {
            // booster.Scene.Add(new CloneBooster(position, true, 
            //     CommunalHelperGFX.SpriteBank.Create((bool)spiralBoosterClockwise.GetValue(spiral) ? "clockwiseSpiralBooster" : "counterclockwiseSpiralBooster")));
        }
    }
}