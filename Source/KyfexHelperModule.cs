using System;
using System.Collections;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

public class KyfexHelperModule : EverestModule {
    public static KyfexHelperModule Instance { get; private set; }

    // public override Type SettingsType => typeof(KyfexHelperModuleSettings);
    // public static KyfexHelperModuleSettings Settings => (KyfexHelperModuleSettings) Instance._Settings;
    //
    // public override Type SessionType => typeof(KyfexHelperModuleSession);
    // public static KyfexHelperModuleSession Session => (KyfexHelperModuleSession) Instance._Session;
    //
    // public override Type SaveDataType => typeof(KyfexHelperModuleSaveData);
    // public static KyfexHelperModuleSaveData SaveData => (KyfexHelperModuleSaveData) Instance._SaveData;

    public KyfexHelperModule() {
        Instance = this;

        Logger.SetLogLevel("KyfexHelper", LogLevel.Verbose);
        
        FlagUnlock.unlockRoutines.Add(typeof(Key), (entity, follower) => 
            new Coroutine(keyUnlockRoutine(follower, entity)));
        FlagUnlock.onUnlockActions.Add(typeof(Key), (entity, follower) => {
            ((Key)follower.Entity).RegisterUsed();
        });
    }

    private static IEnumerator keyUnlockRoutine(Follower follower, FlagUnlock unlocker) {
        ((Key)follower.Entity).StartedUsing = true;
        yield return ((Key)follower.Entity).UseRoutine(unlocker.Center);
    }

    public override void Load() {
        KevinZipper.Load();
        PlayerCloneRenderer.Load();
    }

    public override void Unload() {
        KevinZipper.Unload();
        PlayerCloneRenderer.Unload();
    }
}