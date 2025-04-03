using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/BetterShieldedRefill")]
public class BetterShieldedRefill : FlyFeather {
    private readonly Refill refill;
    public BetterShieldedRefill(Vector2 position, bool singleUse, bool twoDashes)
        : base(position, true, singleUse) {
        this.refill = new Refill(this.Position, twoDashes, singleUse);
        this.refill.wiggler.sineAdd = 0;//disable refill
    }
    public BetterShieldedRefill(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Bool("singleUse"), data.Bool("twoDashes")) { }

    public override void Added(Scene scene) {
        base.Added(scene);
        this.Scene.Add(this.refill);
    }

    public override void Update() {
        base.Update();
        
    }
}