using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Hitbox = Monocle.Hitbox;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/BubbleRedirector")]
public class BubbleRedirector : Entity {
    public enum Direction {
        UP, DOWN, LEFT, RIGHT, ANY
    }
    public enum Color {
        GREEN, RED, CUSTOM
    }
    private static readonly Dictionary<Color, string> colorNames = new ();
    private static readonly Dictionary<Direction, string> dirNames = new ();
    static BubbleRedirector() {
        colorNames.Add(Color.GREEN, "green");
        colorNames.Add(Color.RED, "red");
        colorNames.Add(Color.CUSTOM, "custom");
        
        dirNames.Add(Direction.UP, "up");
        dirNames.Add(Direction.DOWN, "down");
        dirNames.Add(Direction.LEFT, "left");
        dirNames.Add(Direction.RIGHT, "right");
        dirNames.Add(Direction.ANY, "any");
    }
    
    public readonly Direction direction;
    public readonly Color color;
    private readonly Sprite sprite;
    private readonly Sprite innerSprite;
    private Booster currentBooster;
    public BubbleRedirector(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.direction = data.Enum("direction", Direction.ANY);
        this.color = data.Enum("color", Color.CUSTOM);
        this.Add(this.sprite = GFX.SpriteBank.Create("KyfexHelper_bubbleRedirector_"+colorNames[this.color]));
        this.Add(this.innerSprite = GFX.SpriteBank.Create("KyfexHelper_bubbleRedirector_"+colorNames[this.color]+"_arrow"+dirNames[this.direction]));
        
        this.Add(new PlayerCollider(this.OnPlayer, new Hitbox(2f, 2f, -1f, -1f)));
    }

    public override void Update() {
        base.Update();
        if (this.canBoost && (Input.DashPressed || Input.CrouchDashPressed)) {
            Input.Dash.ConsumeBuffer();
            Input.CrouchDash.ConsumeBuffer();
            this.shouldBoost = true;
        }
    }

    private bool canBoost=false;
    private bool shouldBoost=false;
    private void OnPlayer(Player player) {
        if (this.currentBooster == null && player.LastBooster != null && player.LastBooster.BoostingPlayer) {
            this.currentBooster = player.LastBooster;
            this.Add(new Coroutine(this.OnPlayerCoroutine(player)));
        }
    }

    private IEnumerator OnPlayerCoroutine(Player player) {
        player.Center = this.Position;
        var playerSpeed = player.Speed.Length();
        player.Speed = new Vector2();
        this.canBoost = true;
        var elapsed = 0f;
        while (!this.shouldBoost && elapsed < 0.2) {
            yield return null;
            elapsed += Engine.DeltaTime;
        }
        this.canBoost = false;
        this.shouldBoost = false;
        
        if (this.direction == Direction.ANY) {
            Vector2 dashDir = player.lastAim;
            if (player.OverrideDashDirection.HasValue)
                dashDir = player.OverrideDashDirection.Value;
            dashDir = player.CorrectDashPrecision(dashDir);

            player.Speed = new Vector2((float)Math.Cos(dashDir.Angle()),(float)Math.Sin(dashDir.Angle())) * playerSpeed;
        } else {
            switch (this.direction) {
                case Direction.LEFT:
                    player.Speed = new Vector2(-playerSpeed, 0);
                    break;
                case Direction.RIGHT:
                    player.Speed = new Vector2(playerSpeed, 0);
                    break;
                case Direction.UP:
                    player.Speed = new Vector2(0, -playerSpeed);
                    break;
                case Direction.DOWN:
                    player.Speed = new Vector2(0, playerSpeed);
                    break;
            }
        }

        yield return 0.1;
        this.currentBooster = null;
    }
}