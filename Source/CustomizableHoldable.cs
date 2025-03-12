using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper {
    [CustomEntity("KyfexHelper/CustomizableHoldable")]
    public class CustomizableHoldable : Actor{
        private Sprite sprite;
        private Holdable hold;
        public CustomizableHoldable(EntityData data, Vector2 offset) : base(data.Position+offset) {
            base.Depth = data.Int("depth",100);
            base.Collider = new Hitbox(data.Width, data.Height, data.Width*-0.5f, data.Height*-1);
            Add(this.sprite = GFX.SpriteBank.Create(data.Attr("sprite","theo_crystal")));
            Add(this.hold = new Holdable(data.Float("cooldown",0.1f)));

            var holdboxWidth = data.Int("hbWidth", 16);
            this.hold.PickupCollider = new Hitbox(holdboxWidth, 22f, holdboxWidth*-0.5f, -16f);

            base.Tag = Tags.TransitionUpdate;
        }
    }
}
