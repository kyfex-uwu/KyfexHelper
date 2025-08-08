using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/BubbleRedirector")]
public class BubbleRedirector : Entity {
    public BubbleRedirector(EntityData data, Vector2 offset) : base(data.Position + offset) {
        
    }
}