using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/MapperNote")]
public class LoennEntity : Entity{
    public LoennEntity(){ }
    public override void Added(Scene scene) {
        this.RemoveSelf();
    }
}