using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/FakeRoomTransition"), Tracked]
public class FakeRoomTransition : Entity {
    public readonly bool isHorizontal;
    public readonly int size;
    public readonly bool canGoBack;
    public FakeRoomTransition(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.isHorizontal = data.Bool("isHorizontal", false);
        this.size = (int) Math.Round(data.Float("size", this.isHorizontal ? 40 : 22.5f) * 8);
        this.canGoBack = data.Bool("canGoBack", true);
        
        this.Add(new PlayerCollider(this.OnTransition));
    }

    private bool isPlayerBehind = false;
    private void updateCollision() {
        if (this.isHorizontal) {
            if (this.Scene.Tracker.GetEntity<Player>()?.Center.Y > this.Y) {
                if (!this.isPlayerBehind) {
                    this.Collider.Position.Y = -this.Collider.Height;
                    this.isPlayerBehind = true;
                }
            } else {
                if (this.isPlayerBehind) {
                    this.Collider.Position.Y = 0;
                    this.isPlayerBehind = false;
                }
            }
        } else {
            if (this.Scene.Tracker.GetEntity<Player>()?.Center.X > this.X) {
                if (!this.isPlayerBehind) {
                    this.Collider.Position.X = -this.Collider.Width;
                    this.isPlayerBehind = true;
                }
            } else {
                if (this.isPlayerBehind) {
                    this.Collider.Position.X = 0;
                    this.isPlayerBehind = false;
                }
            }
        }
    }
    public override void Awake(Scene scene) {
        base.Awake(scene);
        this.Collider = new Hitbox(this.SceneAs<Level>().Bounds.Width, this.SceneAs<Level>().Bounds.Height);
        if (!this.isHorizontal) this.Collider.Height = this.size/8f;
        else this.Collider.Width = this.size/8f;
        
        this.updateCollision();
    }

    public override void Update() {
        base.Update();
        
        this.updateCollision();
    }

    private void OnTransition(Player player) {
        var level = this.SceneAs<Level>();
        level.transition = new Coroutine(this.OnTransitionCoroutine(player, level));
    }

    public static bool lockCam { get; private set; } = true;
    private IEnumerator OnTransitionCoroutine(Player player, Level level) {
        foreach (SoundSource component in level.Tracker.GetComponents<SoundSource>()) {
            if (component.DisposeOnTransition)
                component.Stop();
        }

        if (!this.isHorizontal) player.X = this.X + player.Width * (this.isPlayerBehind ? -1 : 1) / 2;
        else player.Y = this.Y + player.Height * (this.isPlayerBehind ? 0 : 1);

        Vector2 direc;
        Vector2 playerTo = player.Position;
        while (!this.isHorizontal && playerTo.Y >= level.Bounds.Bottom)
            --playerTo.Y;
        if (this.isHorizontal) {
            direc = Vector2.UnitY;
            if (this.isPlayerBehind) playerTo.Y = this.Y;
            else playerTo.Y = this.Y+player.Height;
        } else {
            direc = Vector2.UnitX;
            if (this.isPlayerBehind) playerTo.X = this.X-player.Width/2;
            else playerTo.X = this.X+player.Width/2;
        }
        direc *= this.isPlayerBehind?1:-1;

        lockCam = false;
        bool cameraFinished = player.CollideAll<RespectFakeTransitionCamera>().Count == 0;
        float cameraAt = cameraFinished ? 1 : 0;
        var cameraFrom = level.Camera.Position;
        Vector2 cameraTo;
        if (this.isHorizontal) {
            cameraTo = new Vector2(cameraFrom.X, this.Y + (this.isPlayerBehind ? -1 : 0)*level.Camera.Viewport.Height);
        } else {
            cameraTo = new Vector2(this.X + (this.isPlayerBehind ? -1 : 0)*level.Camera.Viewport.Width, cameraFrom.Y);
        }
        while (!player.TransitionTo(playerTo, direc) || cameraAt < 1.0) {
            yield return null;
            if (!cameraFinished) {
                cameraAt = Calc.Approach(cameraAt, 1f, Engine.DeltaTime / level.NextTransitionDuration);
                level.Camera.Position = cameraAt <= 0.9 ? Vector2.Lerp(cameraFrom, cameraTo, Ease.CubeOut(cameraAt)) : cameraTo;
                if (cameraAt >= 1.0)
                    cameraFinished = true;
            }
        }

        lockCam = true;
        player.Position = playerTo;
        
        player.OnTransition();
        this.updateCollision();
        level.NextTransitionDuration = 0.65f;
        level.transition = null;

        if (!this.canGoBack) {
            if (this.isHorizontal) {
                //todo
                level.Add(new InvisibleBarrier(this.Position + (this.isPlayerBehind?-Vector2.UnitY:Vector2.Zero),
                    this.size, 0));
            } else {
                level.Add(new InvisibleBarrier(this.Position + (this.isPlayerBehind?-Vector2.UnitX:Vector2.Zero),
                    0, this.size));
            }
        }
    }

    public override void DebugRender(Camera camera) {
        this.Collider.Render(camera, Color.Blue);
        
        if (this.isHorizontal) {
            Draw.Line(this.Position, this.Position+new Vector2(this.size/8f,0), Color.Magenta);
            var offs = (this.Collider.Position.Y > 0 ? -1 : 1);
            Draw.Line(this.Position+new Vector2(0,offs), this.Position+new Vector2(this.size/8f,offs), Color.Purple);
        } else {
            Draw.Line(this.Position, this.Position+new Vector2(0,this.size/8f), Color.Magenta);
            var offs = (this.Collider.Position.X > 0 ? -1 : 1);
            Draw.Line(this.Position+new Vector2(offs,0), this.Position+new Vector2(offs,this.size/8f), Color.Purple);
        }
    }
}