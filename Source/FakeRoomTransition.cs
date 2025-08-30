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

    private bool isPlayerOnPosSide = false;

    public bool getPositionOnPosSide(Vector2 pos) {
        if (this.isHorizontal) {
            return (pos.Y > this.Y);
        } else {
            return(pos.X > this.X);
        }
    }
    private void updateCollision() {
        var player = this.Scene.Tracker.GetEntity<Player>();
        if (player != null) {
            this.isPlayerOnPosSide = this.getPositionOnPosSide(player.Center);
            if (this.isHorizontal) {
                if (player.Center.Y > this.Y) {
                    this.Collider.Position.Y = -this.Collider.Height;
                } else {
                    this.Collider.Position.Y = 0;
                }
            } else {
                if (player.Center.X > this.X) {
                    this.Collider.Position.X = -this.Collider.Width;
                } else {
                    this.Collider.Position.X = 0;
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

        if (!this.isHorizontal) player.X = this.X + player.Width * (this.isPlayerOnPosSide ? -1 : 1) / 2;
        else player.Y = this.Y + player.Height * (this.isPlayerOnPosSide ? 0 : 1);

        Vector2 playerTo = player.Position;
        
        bool cameraShouldRespect = player.CollideAll<RespectFakeTransitionCamera>().Count > 0;
        foreach(var e in !cameraShouldRespect?new[]{this}:
                    this.Scene.Tracker.GetEntities<FakeRoomTransition>().ToArray()) {
            var wall = e as FakeRoomTransition;
            if (wall.isHorizontal) {
                if (this.isPlayerOnPosSide) playerTo.Y = Math.Max(playerTo.Y,wall.Y);
                else playerTo.Y = Math.Min(playerTo.Y, wall.Y+player.Height);
            } else {
                if (this.isPlayerOnPosSide)  playerTo.X = Math.Max(playerTo.X,wall.X-player.Width/2);
                else playerTo.X = Math.Min(playerTo.X, wall.X+player.Width/2);
            }
        }

        Vector2 cameraTo = level.Camera.Position + new Vector2();
        if (this.isHorizontal) {
            cameraTo.Y -= this.getPositionOnPosSide(playerTo) ? 0 : level.Camera.Zoom * 22.5f;
        } else {
            cameraTo.X -= this.getPositionOnPosSide(playerTo) ? 0 : level.Camera.Zoom * 40f;
        }
        Vector2 topLeftest = new Vector2(0,0);
        Vector2 bottomRightest = new Vector2(level.Bounds.Width-40*8, level.Bounds.Height-22.5f*8);
        if (cameraShouldRespect) {
            foreach (FakeRoomTransition wall in this.Scene.Tracker.GetEntities<FakeRoomTransition>()) {
                if (wall.isHorizontal) {
                    if (!wall.getPositionOnPosSide(playerTo)) {
                        bottomRightest.X = Math.Min(bottomRightest.X, wall.X);
                    } else {
                        topLeftest.X = Math.Max(topLeftest.X, wall.X);
                    }
                } else {
                    if (!wall.getPositionOnPosSide(playerTo)) {
                        bottomRightest.Y = Math.Min(bottomRightest.Y, wall.Y);
                    } else {
                        topLeftest.Y = Math.Max(topLeftest.Y, wall.Y);
                    }
                }
            }

            if (topLeftest.X < cameraTo.X) cameraTo.X = topLeftest.X;
            if (topLeftest.Y < cameraTo.Y) cameraTo.Y = topLeftest.Y;
            if (bottomRightest.X-22.5f*8 > cameraTo.X) cameraTo.X = bottomRightest.X-22.5f*8;
            if (bottomRightest.Y-40*8 > cameraTo.Y+40*8) cameraTo.Y = bottomRightest.X-40*8;
            // cameraTo = bottomRightest;
        }
        // cameraTo += new Vector2(level.Bounds.Left, level.Bounds.Top);
        
        lockCam = false;
        bool cameraFinished = !cameraShouldRespect;
        float cameraAt = cameraFinished ? 1 : 0;
        var cameraFrom = level.Camera.Position;
        Vector2 direc = (this.isHorizontal ? Vector2.UnitY : Vector2.UnitX) * (this.isPlayerOnPosSide?1:-1);
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
                level.Add(new InvisibleBarrier(this.Position + (this.isPlayerOnPosSide?-Vector2.UnitY:Vector2.Zero),
                    this.size, 0));
            } else {
                level.Add(new InvisibleBarrier(this.Position + (this.isPlayerOnPosSide?-Vector2.UnitX:Vector2.Zero),
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