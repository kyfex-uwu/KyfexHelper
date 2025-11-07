using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.CommunalHelper;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/CompleteWithoutEndTrigger")]
public class CompleteWithoutEndTrigger : Trigger {
    private readonly CompleteBehavior behavior;
    public CompleteWithoutEndTrigger(EntityData data, Vector2 offset) : base(data, offset) {
        this.behavior = new(data.Bool("collectBerries", true), data.Bool("canRetry",false));
    }
    public override void OnEnter(Player player) {
        base.OnEnter(player);
        completeBehaviors.Remove(this.SceneAs<Level>());
        completeBehaviors.Add(this.SceneAs<Level>(), this.behavior);
    }

    private record CompleteBehavior(bool collectBerries=true, bool canRetry=false) { }

    private static Dictionary<Level, CompleteBehavior> completeBehaviors = new();

    private static List<IDisposable> hooks = new();
    private static void onLevelEnd(On.Celeste.Level.orig_RegisterAreaComplete orig, Level self) {
        if (completeBehaviors.TryGetValue(self, out var behavior) && !behavior.collectBerries) {
            Player entity = self.Tracker.GetEntity<Player>();
            var oldFollowers = entity.Leader.Followers;
            entity.Leader.Followers = oldFollowers.FindAll(follower => !(follower.Entity is IStrawberry));
            orig(self);
            entity.Leader.Followers = oldFollowers;
        } else {
            completeBehaviors.Remove(self);
            orig(self);
        }
    }

    private static bool shouldCompletedCount(bool completed, Level self) {
        if (!completed) return false;

        //if no custom behavior, return true
        return !completeBehaviors.TryGetValue(self, out var behavior) || !behavior.canRetry;

    }
    private static void checkForRetry(ILContext ctx) {
        var cursor = new ILCursor(ctx);

        cursor.GotoNext(MoveType.After, instr => instr.MatchLdstr("menu_pause_retry"));
        cursor.GotoNext(MoveType.After, instr => instr.MatchLdfld(typeof(Level), "Completed"));
        cursor.EmitLdarg0();
        cursor.EmitDelegate(shouldCompletedCount);
    }

    private static void reload(On.Celeste.Level.orig_Reload orig, Level self) {
        if (completeBehaviors.ContainsKey(self)) {
            var realCompleted = self.Completed;
            self.Completed = false;
            orig(self);
            self.Completed = realCompleted;
        } else {
            orig(self);
        }
    }

    // private static bool showHud(Func<Level,bool> orig, Level self) {
    //     if (completeBehaviors.ContainsKey(self)) {
    //         var realCompleted = self.Completed;
    //         self.Completed = false;
    //         var toReturn = orig(self);
    //         self.Completed = realCompleted;
    //         return toReturn;
    //     } else {
    //         return orig(self);
    //     }
    // }
    public static void Load() {
        On.Celeste.Level.RegisterAreaComplete += onLevelEnd;
        hooks.Add(new ILHook(typeof(Level).GetMethod("orig_Pause", BindingFlags.Public | BindingFlags.Instance),
            checkForRetry));
        On.Celeste.Level.Reload += reload;
        // hooks.Add(new Hook(typeof(Level).GetMethod("get_ShowHud", BindingFlags.Public|BindingFlags.Instance),
        //     showHud));
    }

    public static void Unload() {
        On.Celeste.Level.RegisterAreaComplete -= onLevelEnd;
        On.Celeste.Level.Reload -= reload;
        foreach(var hook in hooks) hook.Dispose();
    }
}