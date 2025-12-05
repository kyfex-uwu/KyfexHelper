using Celeste.Mod.AubreyHelper;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/AubreyHelperPosSettingTrigger")]
public class AubreyHelperPosSettingTrigger : Trigger {
    public readonly KyfexHelperModuleSession.Enabled posSwap;
    public readonly float? cooldown;
    public AubreyHelperPosSettingTrigger(EntityData data, Vector2 offs) : base(data, offs) {
        switch (data.String("posSwap")) {
            case "true": this.posSwap = KyfexHelperModuleSession.Enabled.ENABLED; break;
            case "false": this.posSwap = KyfexHelperModuleSession.Enabled.DISABLED; break;
            default: this.posSwap = KyfexHelperModuleSession.Enabled.UNSET; break;
        }

        this.cooldown = data.Bool("useCooldown") ? data.Float("cooldown") : null;
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        KyfexHelperModule.Session.AubreyHelper_FakePosEnabled = this.posSwap;
        KyfexHelperModule.Session.AubreyHelper_Cooldown = this.cooldown;
    }
}