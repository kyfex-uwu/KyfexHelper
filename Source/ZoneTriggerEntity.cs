using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/ZoneTriggerEntity")]
public class ZoneTriggerEntity : Entity {
    public readonly string triggerKey;
    public ZoneTriggerEntity(EntityData data, Vector2 offs) : base(data.Position + offs) {
        this.Collider = new Hitbox(data.Width, data.Height);
        this.triggerKey = data.String("triggerKey");
    }
}