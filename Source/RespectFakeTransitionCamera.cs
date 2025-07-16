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
                var frt = transition as FakeRoomTransition;
                if (frt.isHorizontal) {
                    if (cam.Y < frt.Y && cam.Y + cam.Viewport.Height > frt.Y &&
                        cam.X - frt.size/8f < frt.X && cam.X + cam.Viewport.Width > frt.X) {
                        if (self.Center.Y > transition.Y) cam.Y = Math.Max(cam.Y, transition.Y);
                        else cam.Y = Math.Min(cam.Y, transition.Y - cam.Viewport.Height);
                    }
                } else {
                    if (cam.X < frt.X && cam.X + cam.Viewport.Width > frt.X &&
                        cam.Y - frt.size/8f < frt.Y && cam.Y + cam.Viewport.Height > frt.Y) {
                        if (self.Center.X > transition.X) cam.X = Math.Max(cam.X, transition.X);
                        else cam.X = Math.Min(cam.X, transition.X - cam.Viewport.Width);
                    }
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