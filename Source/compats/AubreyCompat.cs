using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.AubreyHelper;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.KyfexHelper;

public class AubreyCompat {
    private static List<Hook> hooks = new();
    private static bool saving = false;

    private static bool hookPSE(Func<AubreyHelperModuleSettings, bool> orig, AubreyHelperModuleSettings self) {
        if (KyfexHelperModule.Session == null || KyfexHelperModule.Session.AubreyHelper_FakePosEnabled == KyfexHelperModuleSession.Enabled.UNSET)
            return orig(self);
        return KyfexHelperModule.Session.AubreyHelper_FakePosEnabled == KyfexHelperModuleSession.Enabled.ENABLED;
    }
    private static float hookCooldown(Func<AubreyHelperModuleSettings.UseEveryWhereSettings, float> orig, AubreyHelperModuleSettings.UseEveryWhereSettings self) {
        if (KyfexHelperModule.Session == null || KyfexHelperModule.Session.AubreyHelper_Cooldown == null)
            return orig(self);
        return KyfexHelperModule.Session.AubreyHelper_Cooldown ?? 0;
    }
    public static void LoadHooks() {
        hooks.Add(new Hook(typeof(AubreyHelperModuleSettings).GetMethod("get_UseEveryWhere", BindingFlags.Instance|BindingFlags.Public), 
            hookPSE));
        hooks.Add(new Hook(typeof(AubreyHelperModuleSettings.UseEveryWhereSettings).GetMethod("get_Cooldown", BindingFlags.Instance|BindingFlags.Public), 
            hookCooldown));
    }

    public static void UnloadHooks() {
        foreach (var hook in hooks) hook.Dispose();
    }
}