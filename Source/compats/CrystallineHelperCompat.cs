
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using vitmod;

namespace Celeste.Mod.KyfexHelper;

public class CrystallineHelperCompat {
    public static List<Hook> hooks = new();
    public static void Load() {
        BubbleRedirector.checkCrystallineBoosters = true;
    }

    public static void LoadHooks() {
        hooks.Add(new Hook(typeof(EnergyBooster).GetMethod("BoostRoutine", BindingFlags.Instance | BindingFlags.NonPublic),
            boostRoutineModifier));
    }
    public static void UnloadHooks() {
        foreach (var hook in hooks) hook.Dispose();
    }

    public static Entity TryFindBooster(Scene scene) {
        foreach (var booster in scene.Tracker.GetEntities<EnergyBooster>()) {
            if ((booster as EnergyBooster).BoostingPlayer != null) return booster;
        }

        return null;
    }

    public static void LoadBoosters(Scene scene) {
        if(!scene.Tracker.Entities.ContainsKey(typeof(EnergyBooster)))
            scene.Tracker.GetEntitiesTrackIfNeeded<EnergyBooster>();
    }

    private static Dictionary<EnergyBooster, Vector2> newDirs = new();
    public static void AdjustBooster(Entity entity, BubbleRedirector.Direction direction) {
        if (entity is EnergyBooster eBooster) {
            eBooster.PlayerSpeed = BubbleRedirector.dirOf(direction) * eBooster.PlayerSpeed.Length();

            newDirs.Remove(eBooster);
            newDirs.Add(eBooster, eBooster.PlayerSpeed);
        }
    }

    private static IEnumerator boostRoutineModifier(Func<EnergyBooster, Player, Vector2, Vector2, IEnumerator> orig,
        EnergyBooster self, Player player, Vector2 dir, Vector2 speed) {
        var enumerator = new SwapImmediately(orig(self, player, dir, speed));
        while (enumerator.MoveNext()) {
            yield return enumerator.Current;
            if (newDirs.TryGetValue(self, out var newSpeed)) {
                speed.X = newSpeed.X;
                speed.Y = newSpeed.Y;
                newSpeed.SafeNormalize();
                dir.X = newSpeed.X;
                dir.Y = newSpeed.Y;
                // newDirs.Remove(self);
            }
        }
    }
}