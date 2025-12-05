using System;

namespace Celeste.Mod.KyfexHelper;

public class KyfexHelperModuleSession : EverestModuleSession {
    public enum Enabled {
        ENABLED,
        DISABLED,
        UNSET
    }
    public Enabled AubreyHelper_FakePosEnabled = Enabled.UNSET;
    public float? AubreyHelper_Cooldown = null;
}