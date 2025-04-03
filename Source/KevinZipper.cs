using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/KevinZipper")]
public class KevinZipper : CrushBlock {
    public readonly Vector2 crushDir360;
    public readonly Vector2 endPos;
    public readonly Vector2 beginPos;
    private ZipMover fakeMover;
    private ZipMover.ZipMoverPathRenderer pathRenderer;
    private string faceDirection;

    public KevinZipper(
        Vector2 position,
        float width,
        float height,
        Axes axes,
        Vector2 endPos)
        : base(position, width, height, axes) {
        this.endPos = endPos;
        this.beginPos = this.Position;
        this.crushDir360 = (this.endPos - this.Position).SafeNormalize();

        if (Math.Abs(this.crushDir360.X) > Math.Abs(this.crushDir360.Y)) {
            this.faceDirection = Math.Sign(this.crushDir360.X) == 1 ? "right" : "left";
        } else {
            this.faceDirection = Math.Sign(this.crushDir360.Y) == 1 ? "down" : "up";
        }
    }

    public KevinZipper(EntityData data, Vector2 offset) : this(
        data.Position + offset,
        data.Width,
        data.Height,
        data.Enum<Axes>("axes"),
        data.Nodes[0]+offset) { }

    public override void Awake(Scene scene) {
        base.Awake(scene);

        this.Scene.Add(this.pathRenderer = new ZipMover.ZipMoverPathRenderer(this.fakeMover =
            new ZipMover(this.Position, (int)this.Width, (int)this.Height, this.endPos, ZipMover.Themes.Normal) {
                drawBlackBorder = false
            }));
    }

    private IEnumerator CustomAttackSequence() {
        Logger.Log("KyfexHelper","custom attack sequence");
        this.returnStack.Clear();
        
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        this.StartShaking(0.4f);
        yield return 0.4f;
        if (!this.chillOut) this.canActivate = true;
        this.StopPlayerRunIntoAnimation = false;
        var speed = 0.0f;
        while (true) {
            speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);

            var flag = this.ExactPosition == this.endPos;
            if (!flag) {
                if (this.Scene.OnInterval(0.02f)) {
                    var posx = Vector2.Zero;
                    float dirx = 0;
                    var posy = Vector2.Zero;
                    float diry = 0;
                    if (this.crushDir360.X > 0.35) {
                        posx = new Vector2(this.Left + 1f,
                            Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
                        dirx = 3.1415927f;
                    } else if (this.crushDir360.X < -0.35) {
                        posx = new Vector2(this.Right - 1f,
                            Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
                        dirx = 0.0f;
                    }

                    if (this.crushDir360.Y > 0.35) {
                        posy = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f),
                            this.Top + 1f);
                        diry = -1.5707964f;
                    } else if (this.crushDir360.Y < -0.35) {
                        posy = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f),
                            this.Bottom - 1f);
                        diry = 1.5707964f;
                    }

                    if (posx != Vector2.Zero) this.level.Particles.Emit(P_Crushing, posx, dirx);
                    if (posy != Vector2.Zero) this.level.Particles.Emit(P_Crushing, posy, diry);
                }
                
                this.MoveTowardsX(this.endPos.X, speed * Engine.DeltaTime*Math.Abs(this.crushDir360.X));
                this.MoveTowardsY(this.endPos.Y, speed * Engine.DeltaTime*Math.Abs(this.crushDir360.Y));
                if (this.Scene.OnInterval(0.1f)) this.pathRenderer.CreateSparks();
                yield return null;
            } else {
                break;
            }
        }

        
        if (Math.Abs(this.crushDir360.X) > 0.35) {
            for (var index = 0; index < this.Height / 8.0; ++index) {
                var point = new Vector2(this.Center.X + (this.Width / 2 + 1) * Math.Sign(this.crushDir360.X),
                    this.Top + 4f + index * 8);
                this.SceneAs<Level>().ParticlesFG.Emit(P_Impact, point, this.crushDir360.Angle());
            }
        }

        if (Math.Abs(this.crushDir360.Y) > 0.35) {
            for (var index = 0; index < this.Width / 8.0; ++index) {
                var point = new Vector2(this.Left + 4f + index * 8,
                    this.Center.Y + (this.Height / 2 + 1) * Math.Sign(this.crushDir360.Y));
                this.SceneAs<Level>().ParticlesFG.Emit(P_Impact, point, this.crushDir360.Angle());
            }
        }

        Audio.Play("event:/game/06_reflection/crushblock_impact", this.Center);
        this.level.DirectionalShake(this.crushDir);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        this.StartShaking(0.4f);
        this.StopPlayerRunIntoAnimation = true;
        var sfx = this.currentMoveLoopSfx;
        this.currentMoveLoopSfx.Param("end", 1f);
        this.currentMoveLoopSfx = null;
        Alarm.Set(this, 0.5f, () => sfx.RemoveSelf());
        this.crushDir = Vector2.Zero;
        this.TurnOffImages();
        this.face.Play("hurt");
        this.returnLoopSfx.Play("event:/game/06_reflection/crushblock_return_loop");
        yield return 0.4f;
        speed = 0.0f;
        var waypointSfxDelay = 0.0f;

        while (true) {
            yield return null;
            this.StopPlayerRunIntoAnimation = false;
            speed = Calc.Approach(speed, 60f, 160f * Engine.DeltaTime);
            waypointSfxDelay -= Engine.DeltaTime;
            if (true) this.MoveTowardsX(this.beginPos.X, speed * Engine.DeltaTime*Math.Abs(this.crushDir360.X));
            if (true) this.MoveTowardsY(this.beginPos.Y, speed * Engine.DeltaTime*Math.Abs(this.crushDir360.Y));
            if (this.ExactPosition.X == this.beginPos.X && this.ExactPosition.Y == this.beginPos.Y) {
                this.StopPlayerRunIntoAnimation = true;
                this.face.Play("idle");
                this.returnLoopSfx.Stop();
                if (waypointSfxDelay <= 0.0)
                    Audio.Play("event:/game/06_reflection/crushblock_rest", this.Center);

                this.StartShaking(0.2f);
                yield return 0.2f;
                break;
            }
        }
    }

    public override void Update() {
        base.Update();
        
        this.fakeMover.percent = this.crushDir360.X == 0
            ? (this.Position.Y - this.beginPos.Y) / (this.endPos.Y - this.beginPos.Y)
            : (this.Position.X - this.beginPos.X) / (this.endPos.X - this.beginPos.X);
        if (this.crushDir != Vector2.Zero) {
            this.face.Position = new Vector2(this.Width, this.Height) / 2f;
            this.face.Position.X += Math.Sign(this.crushDir360.X);
            if (this.crushDir360.Y < 0) this.face.Position.Y--;
        }
    }
    
    private static void CustomAttack(On.Celeste.CrushBlock.orig_Attack orig, CrushBlock block, Vector2 attackDir) {
        orig(block,attackDir);
        if (block is KevinZipper zipper) {
            zipper.attackCoroutine.Replace(zipper.CustomAttackSequence());
            zipper.nextFaceDirection = zipper.faceDirection;
        }
    }

    public static void Load() {
        On.Celeste.CrushBlock.Attack += CustomAttack;
    }

    public static void Unload() {
        On.Celeste.CrushBlock.Attack -= CustomAttack;
    }
}