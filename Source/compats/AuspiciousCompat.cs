using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.auspicioushelper;
using Celeste.Mod.auspicioushelper.iop;
using Celeste.Mod.KyfexHelper.ausZip;
using Monocle;
using MonoMod.RuntimeDetour;
using BindingFlags = System.Reflection.BindingFlags;

namespace Celeste.Mod.KyfexHelper;

public class AuspiciousCompat {
    public static readonly FieldInfo sposField =
        typeof(TemplateZipmover).GetField("spos", BindingFlags.Instance | BindingFlags.NonPublic);
    public static readonly FieldInfo tcComponentParentField =
        typeof(TemplateIop.TemplateChildComponent).GetField("parent", BindingFlags.Instance | BindingFlags.NonPublic);
    
    public static void Load() {
        TemplateIop.customClarify("KyfexHelper/TemplateKevinTrigger", (_, _, o, e) => 
            new TemplateKevinTrigger(e, o).tChildComponent);
        TemplateIop.customClarify("KyfexHelper/GearField", (_, _, o, e) => 
            new GearField(e, o).tcComponent);
        TemplateIop.customClarify("KyfexHelper/ZipperTrafficLight", (_, _, o, e) => 
            new ZipperTrafficLight(e, o).tcComponent);

        BubbleRedirector.checkAuspiciousBoosters = true;
    }

    public static void LoadBoosters(Scene scene) {
        if(!scene.Tracker.Entities.ContainsKey(typeof(ChannelBooster)))
            scene.Tracker.GetEntitiesTrackIfNeeded<ChannelBooster>();
    }
    public static Entity TryFindBooster(Scene scene) {
        foreach (var booster in scene.Tracker.GetEntities<ChannelBooster>()) {
            if ((booster as ChannelBooster).hasRiders<Player>()) return booster;
        }

        return null;
    }
    
    //--

    private static List<Hook> hooks = new();
    public static void LoadHooks() {
        
    }
    public static void UnloadHooks() {
        foreach (var hook in hooks) hook.Dispose();
    }
}