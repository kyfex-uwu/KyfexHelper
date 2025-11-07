
using System;
using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using vitmod;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/TriggerableZone")]
public class TriggerableZone : TriggerTrigger{
    private static EntityData transform(EntityData data) {
        data.Values["activationType"] = "OnEntityCollide";
        data.Values["entityType"] = "Celeste.Mod.KyfexHelper.ZoneTriggerEntity";//"##"+data.String("triggerKey");
        return data;
    }
    
    public TriggerableZone(EntityData data, Vector2 offset) : base(transform(data), offset) { }

    private static Hook hook;
    public static new void Load() {
        hook = new Hook(typeof(VitModule).GetMethod("GetClassName", BindingFlags.Static | BindingFlags.Public),
            GetClassNameHook);
    }
    public static new void Unload() {
        hook?.Dispose();
    }

    private static bool GetClassNameHook(Func<string, Entity, bool> orig, string name, Entity entity) {
        if (name.StartsWith("Celeste.Mod.KyfexHelper.ZoneTriggerEntity##") &&
                orig("Celeste.Mod.KyfexHelper.ZoneTriggerEntity", entity)) {
            return (entity as ZoneTriggerEntity).triggerKey ==
                   name.Substring("Celeste.Mod.KyfexHelper.ZoneTriggerEntity##".Length);
        }
        return orig(name, entity);
    }
}