using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/RespectFakeTransitionCamera"), Tracked]
public class RespectFakeTransitionCamera : Trigger {
    public RespectFakeTransitionCamera(EntityData data, Vector2 offset) : base(data, offset) { }
    private static void CameraConstrain(On.Celeste.Player.orig_Update orig, Player self) {
        orig(self);
        if (FakeRoomTransition.lockCam && self.CollideAll<RespectFakeTransitionCamera>().Count != 0) {
            var cam = self.SceneAs<Level>().Camera;
            foreach (var transition in self.Scene.Tracker.GetEntities<FakeRoomTransition>()) {
                if ((transition as FakeRoomTransition).isHorizontal) {
                    if (self.Center.Y > transition.Y) cam.Y = Math.Max(cam.Y, transition.Y);
                    else cam.Y = Math.Min(cam.Y, transition.Y - cam.Viewport.Height);
                } else {
                    if (self.Center.X > transition.X) cam.X = Math.Max(cam.X, transition.X);
                    else cam.X = Math.Min(cam.X, transition.X - cam.Viewport.Width);
                }
            }
        }
    }

    public static void Load() {
        On.Celeste.Player.Update += CameraConstrain;
    }

    public static void Unload() {
        On.Celeste.Player.Update -= CameraConstrain;
    }
}