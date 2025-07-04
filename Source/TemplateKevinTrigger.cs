using System;
using Celeste.Mod.auspicioushelper.iop;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/TemplateKevinTrigger")]
public class TemplateKevinTrigger : Solid {
    private enum Direction {
        LEFT, RIGHT, UP, DOWN
    }

    private readonly Direction dir;
    private bool canActivate = true;
    public readonly TemplateIop.TemplateChildComponent tChildComponent;
    
    public TemplateKevinTrigger(EntityData data, Vector2 offset) :
        base(data.Position + offset, data.Width, data.Height, true) {
        this.dir = data.Enum("direction", Direction.UP);
        this.OnDashCollide = this.OnDash;
        this.tChildComponent = new TemplateIop.TemplateChildComponent(this);
    }

    private static Vector2 VecFromDir(Direction dir) {
        switch (dir) {
            case Direction.LEFT: return new Vector2(-1, 0);
            case Direction.RIGHT: return new Vector2(1, 0);
            case Direction.UP: return new Vector2(0,-1);
            case Direction.DOWN: return new Vector2(0,1);
        }

        return new Vector2(0, 0);
    }
    private DashCollisionResults OnDash(Player player, Vector2 dashDir) {
        if(dashDir.Equals(-VecFromDir(this.dir))){
            this.Activate(player, dashDir);
            return DashCollisionResults.Rebound;
        }
        return DashCollisionResults.NormalCollision;
    }

    private void Activate(Player player, Vector2 dashDir) {
        Audio.Play("event:/game/06_reflection/crushblock_activate", this.Center);
        this.canActivate = false;
        
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        
        var direction = this.dir switch {
            Direction.LEFT => (float) Math.PI,
            Direction.RIGHT => 0,
            Direction.UP => 3*(float)Math.PI/2f,
            Direction.DOWN => (float)Math.PI/2f,
            _ => 0
        };
        var position = this.Center + VecFromDir(this.dir) * new Vector2(this.Width-2,this.Height-2)*0.5f;
        Vector2 positionRange = new Vector2(this.Width-2, this.Height-2) * 0.5f * 
            ((this.dir == Direction.UP || this.dir == Direction.DOWN) ? Vector2.UnitX : Vector2.UnitY);
        var amount = (int)(((this.dir == Direction.UP || this.dir == Direction.DOWN) ? this.Width : this.Height) / 8.0) * 4 + 2;
        this.SceneAs<Level>().Particles.Emit(CrushBlock.P_Activate, amount, position, positionRange, direction);

        this.tChildComponent.RegisterDashhit(player, dashDir);
        this.tChildComponent.TriggerParent();
    }
}