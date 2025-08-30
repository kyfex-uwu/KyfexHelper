using Celeste.Mod.CommunalHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

public class CloneDreamBooster : DreamBooster {
    private static bool loaded = false;
    public static void LoadHooks() {
        if (loaded) return;
        loaded = true;
        On.Celeste.Booster.OnPlayer += customOnPlayer;
    }

    public static void UnloadHooks() {
        if (!loaded) return;
        On.Celeste.Booster.OnPlayer -= customOnPlayer;
    }

    private static void customOnPlayer(On.Celeste.Booster.orig_OnPlayer orig, Booster self, Player player) {
        if (self is CloneDreamBooster clone) {
            player.Center = self.Position;
            player.StateMachine.state = clone.red?5:2;
            player.boostTarget = clone.Center;
            player.boostRed = clone.red;
            player.LastBooster = clone;
            
            self.RemoveSelf();
        } else 
            orig(self, player);
    }
    
    public CloneDreamBooster(Vector2 position)
        : base(position) {
        this.Remove(this.sprite);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        while(this.outline.Components.Count > 0) this.outline.Components[0].RemoveSelf();
    }
}