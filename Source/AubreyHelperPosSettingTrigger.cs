using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/AubreyHelperPosSettingTrigger")]
public class AubreyHelperPosSettingTrigger : Trigger {
    public readonly bool? posSwap;
    public AubreyHelperPosSettingTrigger(EntityData data, Vector2 offs) : base(data, offs) {
        switch (data.String("posSwap")) {
            case "true": this.posSwap = true; break;
            case "false": this.posSwap = false; break;
            default: this.posSwap = null; break;
        }
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        KyfexHelperModule.Session.AubreyHelper_FakePosEnabled = this.posSwap;
    }
}