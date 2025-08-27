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

    public static Vector2 dirOf(Direction direction) {
        switch (direction) {
            case Direction.UP: return -Vector2.UnitY;
            case Direction.DOWN: return Vector2.UnitY;
            case Direction.LEFT: return -Vector2.UnitX;
            case Direction.RIGHT: return Vector2.UnitX;
        }
        return Vector2.Zero;
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
    private Entity currentBooster;
    public BubbleRedirector(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.direction = data.Enum("direction", Direction.ANY);
        this.color = data.Enum("color", Color.CUSTOM);
        var tint = data.HexColor("tint", Microsoft.Xna.Framework.Color.White);
        this.Add(this.sprite = GFX.SpriteBank.Create("KyfexHelper_bubbleRedirector_"+colorNames[this.color]));
        this.Add(this.innerSprite = GFX.SpriteBank.Create("KyfexHelper_bubbleRedirector_"+colorNames[this.color]+"_arrow"+dirNames[this.direction]));
        this.sprite.Color = tint;
        this.innerSprite.Color = tint;
        
        this.Add(new PlayerCollider(this.OnPlayer, new Hitbox(2f, 2f, -1f, -1f)));
    }

    public override void Update() {
        base.Update();
        if (this.canBoost && (Input.DashPressed || Input.CrouchDashPressed)) {
            Input.Dash.ConsumeBuffer();
            Input.CrouchDash.ConsumeBuffer();
            this.shouldBoost = true;
        }

        if (checkCrystallineBoosters) CrystallineHelperCompat.LoadBoosters(this.Scene);
        if (checkAuspiciousBoosters) AuspiciousCompat.LoadBoosters(this.Scene);
    }

    private bool canBoost=false;
    private bool shouldBoost=false;
    public static bool checkFrostHelperBoosters = false;
    public static bool checkCommunalHelperBoosters = false;
    public static bool checkCrystallineBoosters = false;
    public static bool checkAuspiciousBoosters = false;
    private void OnPlayer(Player player) {
        if (this.currentBooster == null) {
            if (player.LastBooster != null && player.LastBooster.BoostingPlayer) {
                this.currentBooster = player.LastBooster;
                this.Add(new Coroutine(this.OnPlayerCoroutine(player)));
            }else{
                if (this.currentBooster == null && checkFrostHelperBoosters) {
                    this.currentBooster = FrostHelperCompat.PlayerIsInBubble(player);
                    if(this.currentBooster != null) this.Add(new Coroutine(this.OnPlayerCoroutine(player)));
                }
                if (this.currentBooster == null && checkAuspiciousBoosters) {
                    this.currentBooster = AuspiciousCompat.TryFindBooster(this.Scene);
                    if(this.currentBooster != null) this.Add(new Coroutine(this.OnPlayerCoroutine(player)));
                }
                if (this.currentBooster == null && checkCrystallineBoosters) {
                    this.currentBooster = CrystallineHelperCompat.TryFindBooster(this.Scene);
                    if (this.currentBooster != null) {
                        CrystallineHelperCompat.AdjustBooster(this.currentBooster, this.direction);
                        this.Add(new Coroutine(this.OnPlayerCoroutine(player)));
                    }
                }
            }
            if (this.currentBooster != null && checkCommunalHelperBoosters) {
                CommunalHelperCompat.DerailBooster(this.currentBooster, this.Position, player);
            }
        }
    }

    private IEnumerator OnPlayerCoroutine(Player player) {
        player.Center = this.Position + new Vector2(0,2);
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
            player.Speed = dirOf(this.direction) * playerSpeed;
        }

        while (player.LastBooster != null && player.LastBooster.BoostingPlayer) yield return null;
        if (checkFrostHelperBoosters) {
            while (FrostHelperCompat.PlayerIsInBubble(player) != null) yield return null;
        }
        
        this.currentBooster = null;
    }
}