using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/PlayerCloneRenderer")]
public class PlayerCloneRenderer : Entity {
    private Vector2 offset;
    public PlayerCloneRenderer(EntityData data, Vector2 offset) : base(Vector2.Zero) {
        this.offset = new Vector2(data.Float("xOffset", 0), data.Float("yOffset", 0));
    }
    public override void Render() {
        Entity entity = this.Scene.Tracker.GetEntity<Player>();
        if (entity == null) entity = this.Scene.Tracker.GetEntity<PlayerDeadBody>();
        if (entity!=null) {
            var oldPos = entity.Position;

            var nodes = new List<Vector2>();
            if (entity is Player player) {
                nodes = new List<Vector2>(player.Hair.Nodes);
                for (int i = 0; i < player.Hair.Nodes.Count; i++) {
                    player.Hair.Nodes[i]+=this.offset;
                }
            }
            
            entity.Position += this.offset;
            entity.Render();
            
            entity.Position = oldPos;
            if (entity is Player player2) {
                player2.Hair.Nodes = nodes;
            }
        }
    }
}