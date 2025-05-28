using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/PlayerCloneRenderer"), Tracked]
public class PlayerCloneRenderer : Entity {
    private Vector2 offset;
    private string flag;
    private bool invert = false;
    public PlayerCloneRenderer(EntityData data, Vector2 offset) : base(Vector2.Zero) {
        this.offset = new Vector2(data.Float("xOffset", 0), data.Float("yOffset", 0));
        this.flag = data.String("flag", "");
        if (this.flag.StartsWith("!")) {
            this.invert = true;
            this.flag = this.flag.Substring(1);
        }

        if (this.flag == "") {
            this.invert = !this.invert;
        }
    }

    public static List<Hook> hooks = new();
    public static void Load() {
        hooks.Add(new Hook(typeof(Player).GetMethod("Render", BindingFlags.Instance | BindingFlags.Public),
            renderTwice));
        hooks.Add(new Hook(typeof(PlayerDeadBody).GetMethod("Render", BindingFlags.Instance | BindingFlags.Public),
            renderTwice));
    }

    public static void Unload() {
        foreach (var hook in hooks) {
            hook?.Dispose();
        }
    }

    private static void renderTwice(Action<Entity> orig, Entity entity) {
        foreach (var entityCloneRenderer in entity.Scene.Tracker.GetEntities<PlayerCloneRenderer>()) {
            var cloneRenderer = entityCloneRenderer as PlayerCloneRenderer;
            if (entity.SceneAs<Level>().Session.GetFlag(cloneRenderer.flag) == cloneRenderer.invert) continue;
            
            var oldPos = entity.Position;

            var nodes = new List<Vector2>();
            if (entity is Player player) {
                nodes = new List<Vector2>(player.Hair.Nodes);
                for (int i = 0; i < player.Hair.Nodes.Count; i++) {
                    player.Hair.Nodes[i]+=cloneRenderer.offset;
                }
            }
            
            entity.Position += cloneRenderer.offset;
            orig(entity);
            
            entity.Position = oldPos;
            if (entity is Player player2) {
                player2.Hair.Nodes = nodes;
            }
        }
        orig(entity);
    }
}