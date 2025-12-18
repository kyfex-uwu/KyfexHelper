

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/EasyCustomWindTrigger")]
public class EasyCustomWind : WindTrigger {
    private readonly Vector2 dir;
    public EasyCustomWind(EntityData data, Vector2 offset) : base(data, offset) {
        this.Pattern = WindController.Patterns.None;
        this.dir = data.Vector2("windX", "windY");
    }
    public override void OnEnter(Player player) {
        base.OnEnter(player);
        var controller = this.Scene.Entities.FindFirst<WindController>();
        controller.SetPattern(WindController.Patterns.None);
        controller.targetSpeed = new Vector2(this.dir.X, this.dir.Y);
    }
}