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
        return KyfexHelperModule.Session.AubreyHelper_FakePosEnabled ?? orig(self);
    }
    public static void LoadHooks() {
        hooks.Add(new Hook(typeof(AubreyHelperModuleSettings).GetMethod("get_UseEveryWhere", BindingFlags.Instance|BindingFlags.Public), 
            hookPSE));
        // hooks.Add(new Hook(typeof()));
    }

    public static void UnloadHooks() {
        foreach (var hook in hooks) hook.Dispose();
    }
}