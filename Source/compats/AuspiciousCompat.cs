using Celeste.Mod.auspicioushelper;
using Celeste.Mod.auspicioushelper.iop;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

public class AuspiciousCompat {
    public static void Load() {
        TemplateIop.customClarify("KyfexHelper/TemplateKevinTrigger", (_, _, o, e) => 
            new TemplateKevinTrigger(e, o).tChildComponent);

        BubbleRedirector.checkAuspiciousBoosters = true;
    }

    public static void LoadBoosters(Scene scene) {
        if(!scene.Tracker.Entities.ContainsKey(typeof(ChannelBooster)))
            scene.Tracker.GetEntitiesTrackIfNeeded<ChannelBooster>();
    }
    public static Entity TryFindBooster(Scene scene) {
        foreach (var booster in scene.Tracker.GetEntities<ChannelBooster>()) {
            if ((booster as ChannelBooster).BoostingPlayer) return booster;
        }

        return null;
    }
}