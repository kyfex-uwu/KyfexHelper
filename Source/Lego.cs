using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;
[CustomEntity("KyfexHelper/Lego")]
public class Lego : Solid{
    //safe
    private readonly Image sprite;
    private readonly Holdable hold;
    private readonly Actor gravityer;
    
    private Vector2 prevLiftSpeed;
    private float hardVerticalHitSoundCooldown;
    private readonly Collision onCollideH;
    private readonly Collision onCollideV;
    private bool dead;
    public new bool Collidable {
        get => base.Collidable && this.collisionCollidable;
        set => base.Collidable=value;
    }

    private bool collisionCollidable=true;
    private Level Level{ get=>this.SceneAs<Level>(); }
    public Lego(EntityData data, Vector2 offset) : base(data.Position + offset+new Vector2(0,1), data.Width, data.Height-1,
        data.Bool("safe", true)) {
        this.Add(this.sprite = new Image(GFX.Game["objects/KyfexHelper/lego/lego"]));
        this.sprite.Position = new Vector2(0, -1);
        this.Collidable = true;
        
        this.Add(this.hold = new Holdable(0));
        this.hold.PickupCollider = new Hitbox(this.Width+16, this.Height+16, -8f, -8f);
        this.hold.SlowFall = false;
        this.hold.SlowRun = true;
        this.hold.OnPickup = this.OnPickup;
        this.hold.OnRelease = this.OnRelease;
        this.hold.OnCarry = this.OnCarry;
        this.hold.OnHitSpring = this.HitSpring;
        this.hold.SpeedGetter = () => this.Speed;
        
        this.onCollideH = this.OnCollideH;
        this.onCollideV = this.OnCollideV;

        this.gravityer = new Actor(new Vector2(this.Position.X, this.Position.Y));
        this.gravityer.Collider = this.Collider.Clone();
        this.gravityer.Collidable = false;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        scene.Add(this.gravityer);
    }

    public void OnPickup() {
        this.Speed = Vector2.Zero;
        this.AddTag((int) Tags.Persistent);
        this.Collidable = false;
    }
    public void OnRelease(Vector2 force) {
        this.RemoveTag(Tags.Persistent);
        if (force.X != 0.0 && force.Y == 0.0)
            force.Y = -0.4f;
        this.Speed = force * 200f;
        if (!(this.Speed != Vector2.Zero))
            return;
        this.Collidable = true;
    }

    public void OnCarry(Vector2 pos) {
        this.Position = pos - new Vector2(this.Width / 2, this.Height);
    }
    public bool HitSpring(Spring spring) {
        if (!this.hold.IsHeld) {
            if (spring.Orientation == Spring.Orientations.Floor && this.Speed.Y >= 0.0) {
                this.Speed.X *= 0.5f;
                this.Speed.Y = -160f;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallLeft && this.Speed.X <= 0.0) {
                this.Speed.X = 220f;
                this.Speed.Y = -80f;
                return true;
            }
            if (spring.Orientation == Spring.Orientations.WallRight && this.Speed.X >= 0.0) {
                this.Speed.X = -220f;
                this.Speed.Y = -80f;
                return true;
            }
        }
        return false;
    }
    public bool OnGround(int downCheck = 1) {
        if (this.CollideCheck<Solid>(this.Position + Vector2.UnitY * downCheck))
            return true;
        return this.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * downCheck);
    }
    public bool OnGround(Vector2 at, int downCheck = 1) {
        Vector2 position = this.Position;
        this.Position = at;
        int num = this.OnGround(downCheck) ? 1 : 0;
        this.Position = position;
        return num != 0;
    }
    
    public void OnCollideH(CollisionData data) {
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", this.Position);
        this.Speed.X *= -0.4f;
    }
    public void OnCollideV(CollisionData data)
    {
        if (this.Speed.Y > 0.0){
            if (this.hardVerticalHitSoundCooldown <= 0.0) {
                Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", Calc.ClampedMap(this.Speed.Y, 0.0f, 200f));
                this.hardVerticalHitSoundCooldown = 0.5f;
            } else
                Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", this.Position, "crystal_velocity", 0.0f);
        }
        if (this.Speed.Y > 140.0 && !(data.Hit is SwapBlock))
            this.Speed.Y *= -0.6f;
        else
            this.Speed.Y = 0.0f;
    }
    
    public override void Update()
    {
        base.Update();
        if (this.dead)
            return;
        this.hardVerticalHitSoundCooldown -= Engine.DeltaTime;

        this.Depth = 100;
        if (this.hold.IsHeld)
        {
          this.prevLiftSpeed = Vector2.Zero;
        }
        else
        {
          if (this.OnGround())
          {
            this.Speed.X = Calc.Approach(this.Speed.X, this.OnGround(this.Position + Vector2.UnitX * 3f) ? (this.OnGround(this.Position - Vector2.UnitX * 3f) ? 0.0f : -20f) : 20f, 800f * Engine.DeltaTime);
            Vector2 liftSpeed = this.LiftSpeed;
            if (liftSpeed == Vector2.Zero && this.prevLiftSpeed != Vector2.Zero)
            {
              this.Speed = this.prevLiftSpeed;
              this.prevLiftSpeed = Vector2.Zero;
              this.Speed.Y = Math.Min(this.Speed.Y * 0.6f, 0.0f);
              if ((double) this.Speed.X != 0.0 && (double) this.Speed.Y == 0.0)
                this.Speed.Y = -60f;
            }
            else
            {
              this.prevLiftSpeed = liftSpeed;
              if (liftSpeed.Y < 0.0 && this.Speed.Y < 0.0)
                this.Speed.Y = 0.0f;
            }
          }
          else if (this.hold.ShouldHaveGravity)
          {
            float num1 = 800f;
            if (Math.Abs(this.Speed.Y) <= 30.0)
              num1 *= 0.5f;
            float num2 = 350f;
            if (this.Speed.Y < 0.0)
              num2 *= 0.5f;
            this.Speed.X = Calc.Approach(this.Speed.X, 0.0f, num2 * Engine.DeltaTime);
            this.Speed.Y = Calc.Approach(this.Speed.Y, 200f, num1 * Engine.DeltaTime);
          }
          // this.previousPosition = this.ExactPosition;
          this.collisionCollidable = false;
          this.gravityer.Position = new Vector2(this.Position.X, this.Position.Y);
          this.gravityer.MoveH(this.Speed.X * Engine.DeltaTime, this.onCollideH);
          this.gravityer.MoveV(this.Speed.Y * Engine.DeltaTime, this.onCollideV);
          this.collisionCollidable = true;
          
          if (this.Center.X > (double) this.Level.Bounds.Right)
          {
            this.MoveH(32f * Engine.DeltaTime);
            if (this.Left - 8.0 > (double) this.Level.Bounds.Right)
              this.RemoveSelf();
          }
          else if (this.Left < (double) this.Level.Bounds.Left)
          {
            this.Left = (float) this.Level.Bounds.Left;
            this.Speed.X *= -0.4f;
          }
          else if (this.Top < (this.Level.Bounds.Top - 4))
          {
            this.Top = (this.Level.Bounds.Top + 4);
            this.Speed.Y = 0.0f;
          }
          if (this.X < (this.Level.Bounds.Left + 10))
            this.MoveH(32f * Engine.DeltaTime);
        }
        if (!this.dead)
          this.hold.CheckAgainstColliders();
    }
}