﻿using System;
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
#if DEBUG
        Logger.SetLogLevel("KyfexHelper", LogLevel.Verbose);
#endif
        
        FlagUnlock.unlockRoutines.Add(typeof(Key), (entity, follower) => 
            new Coroutine(keyUnlockRoutine(follower, entity)));
        FlagUnlock.onUnlockActions.Add(typeof(Key), (entity, follower) => {
            ((Key)follower.Entity).RegisterUsed();
        });

        if (Everest.Loader.DependencyLoaded(auspiciousDependency))
            AuspiciousCompat.Load();
        if (Everest.Loader.DependencyLoaded(luaCutscenesDependency))
            LuaCutscenesCompat.Load();
    }

    private static IEnumerator keyUnlockRoutine(Follower follower, FlagUnlock unlocker) {
        ((Key)follower.Entity).StartedUsing = true;
        yield return ((Key)follower.Entity).UseRoutine(unlocker.Center);
        if(unlocker.SFXwaitForUnlock) unlocker.playSound();
    }

    public override void Load() {
        KevinZipper.Load();
        PlayerCloneRenderer.Load();
        InputSwapBlock.Load();
        RespectFakeTransitionCamera.Load();
        CustomCollidableBadeline.Load();
    }

    public override void Unload() {
        KevinZipper.Unload();
        PlayerCloneRenderer.Unload();
        InputSwapBlock.Unload();
        RespectFakeTransitionCamera.Unload();
        CustomCollidableBadeline.Unload();
    }
    
    private static EverestModuleMetadata auspiciousDependency = new EverestModuleMetadata{ Name = "auspicioushelper", Version = new Version(0, 2, 1) };
    private static EverestModuleMetadata luaCutscenesDependency = new EverestModuleMetadata{ Name = "LuaCutscenes", Version = new Version(0, 2, 13) };
}