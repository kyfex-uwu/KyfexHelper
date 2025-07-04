using Celeste.Mod.auspicioushelper.iop;

namespace Celeste.Mod.KyfexHelper;

public class AuspiciousCompat {
    public static void Load() {
        TemplateIop.customClarify("KyfexHelper/TemplateKevinTrigger", (_, _, o, e) => 
            new TemplateKevinTrigger(e, o).tChildComponent);
    }
}