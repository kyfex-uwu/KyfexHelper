using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/CustomCollidableBadeline")]
public class CustomCollidableBadeline : FinalBoss{
    public static Dictionary<Type, Action<Entity, CustomCollidableBadeline>> onKill = new ();
    public static Dictionary<Type, Action<Entity, CustomCollidableBadeline>> onBounce = new ();
    private enum HitType {
        DAMAGE,
        KILL,
        BOUNCE_AWAY,
        NONE
    }

    private HitType playerHitType;
    private HitType holdableHitType;
    private string customSprite;
    private string[] flagList;
    private float bounceStrength;
    private Entity collidedHoldable;
    public CustomCollidableBadeline(EntityData data, Vector2 offset) : base(data, offset) {
        this.playerHitType = data.Enum("playerHitType", HitType.DAMAGE);
        this.holdableHitType = data.Enum("holdableHitType", HitType.DAMAGE);
        this.customSprite = data.String("customSprite", "badeline_boss");
        this.flagList = data.String("flagList","").Split(",");
        if (this.flagList.Length == 1 && this.flagList[0] == "") this.flagList = [];
        this.bounceStrength = data.Float("bounceStrength",1);
        
        this.Add(new HoldableCollider(this.OnHoldable));
    }

    private void OnHoldable(Holdable holdable) {
        this.collide(holdable.Entity, this.holdableHitType, true);
    }

    private bool trueOnPlayer = false;
    private void collide(Entity entity, HitType type, bool holdable=false) {
        switch (type) {
            case HitType.NONE: break;
            case HitType.KILL: {
                if (entity is Player playerEntity)
                    playerEntity.Die((-playerEntity.Speed).SafeNormalize()*0.1f);
                else if (entity is TheoCrystal crystalEntity)
                    crystalEntity.Die();
                else if (onKill.TryGetValue(entity.GetType(), out var run)) run(entity, this);
                else entity.RemoveSelf();
            }break;
            case HitType.BOUNCE_AWAY: {
                if (entity is Player playerEntity) {
                    playerEntity.PointBounce(this.Center);
                    playerEntity.Speed *= this.bounceStrength;
                }else if (entity is TheoCrystal crystalEntity)
                    crystalEntity.Speed = (crystalEntity.Center - this.Center).SafeNormalize(120f)*this.bounceStrength;
                else if (onBounce.TryGetValue(entity.GetType(), out var run)) run(entity, this);
            }break;
            case HitType.DAMAGE: {
                this.trueOnPlayer = true;
                if(this.nodeIndex<this.flagList.Length)
                    this.SceneAs<Level>().Session.SetFlag(this.flagList[this.nodeIndex]);
                if (holdable) this.collidedHoldable = entity;
                this.OnPlayer(entity is Player?(Player)entity:null);
                this.trueOnPlayer = false;
            }break;
        }
    }

    private static void customPlayer(On.Celeste.FinalBoss.orig_OnPlayer orig, FinalBoss boss, Player player) {
        if (boss is CustomCollidableBadeline customBoss && !customBoss.trueOnPlayer) {
            customBoss.collide(player, customBoss.playerHitType);
            return;
        }
        orig(boss, player);
    }

    private static void giveCustomSprite(ILContext ctx) {
        var cursor = new ILCursor(ctx);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchLdstr("badeline_boss"));
        cursor.EmitLdarg0();
        cursor.EmitDelegate(bossSprite);
    }

    private static string bossSprite(string orig, FinalBoss self) {//TODO
        // if (self is CustomCollidableBadeline customBoss) {
        //     return customBoss.customSprite;
        // }

        return orig;
    }

    private static IEnumerator customMoveSequence(On.Celeste.FinalBoss.orig_MoveSequence orig, FinalBoss boss,
        Player player, bool lastHit) {
        if (boss is CustomCollidableBadeline customBoss) {
            if (lastHit) {
                Audio.SetMusicParam("boss_pitch", 1f);
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.3f, start: true);
                tween.OnUpdate = t => Glitch.Value = 0.6f * t.Eased;
                customBoss.Add(tween);
            } else {
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.3f, start: true);
                tween.OnUpdate = t => Glitch.Value = (float) (0.5 * (1.0 - t.Eased));
                customBoss.Add(tween);
            }
            
            //TODO
            if (player != null && !player.Dead) player.StartAttract(customBoss.Center + Vector2.UnitY * 4f);
            //if(customBoss.collidedHoldable)
            float timer = 0.15f;
            while (player != null && !player.Dead && !player.AtAttractTarget) {
                yield return null;
                timer -= Engine.DeltaTime;
            }
            if (timer > 0.0) yield return timer;
            foreach (ReflectionTentacles entity in customBoss.Scene.Tracker.GetEntities<ReflectionTentacles>())
                entity.Retreat();
            if (player != null) {
                Celeste.Freeze(0.1f);
                Engine.TimeRate = !lastHit ? 0.75f : 0.5f;
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            }
            boss.PushPlayer(player);
            
            
            boss.level.Shake();
            yield return 0.05f;
            for (float direction = 0.0f; direction < Math.PI*2; direction += (float)(Math.PI / 18f)) {
                Vector2 position = boss.Center + boss.Sprite.Position + Calc.AngleToVector(direction + Calc.Random.Range(-1f * (float) Math.PI / 90f, (float) Math.PI / 90f), Calc.Random.Range(16, 20));
                boss.level.Particles.Emit(FinalBoss.P_Burst, position, direction);
            }
            yield return 0.05f;
            Audio.SetMusicParam("boss_pitch", 0.0f);
            float from1 = Engine.TimeRate;
            Tween tween1 = Tween.Create(Tween.TweenMode.Oneshot, duration: 0.35f / Engine.TimeRateB, start: true);
            tween1.UseRawDeltaTime = true;
            tween1.OnUpdate = t => {
                if (boss.bossBg != null &&  boss.bossBg.Alpha < t.Eased)
                    boss.bossBg.Alpha = t.Eased;
                Engine.TimeRate = MathHelper.Lerp(from1, 1f, t.Eased);
                if (!lastHit) return;
                Glitch.Value = (float) (0.6 * (1.0 - t.Eased));
            };
            boss.Add(tween1);
            yield return 0.2f;
            Vector2 from2 = boss.Position;
            Vector2 to = boss.nodes[boss.nodeIndex];
            float duration = Vector2.Distance(from2, to) / 600f;
            float dir = (to - from2).Angle();
            Tween tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, duration, true);
            tween2.OnUpdate =(t) => {
                boss.Position = Vector2.Lerp(from2, to, t.Eased);
                if (t.Eased < 0.1 || t.Eased > 0.9 || !boss.Scene.OnInterval(0.02f)) return;
                TrailManager.Add(boss, Player.NormalHairColor, 0.5f, false, false);
                boss.level.Particles.Emit(Player.P_DashB, 2, boss.Center, Vector2.One * 3f, dir);
            };
            tween2.OnComplete = (t) => {
                boss.Sprite.Play("recoverHit");
                boss.Moving = false;
                boss.Collidable = true;
                Player entity = boss.Scene.Tracker.GetEntity<Player>();
                if (entity != null) {
                    boss.facing = Math.Sign(entity.X - boss.X);
                    if (boss.facing == 0) boss.facing = -1;
                }
                boss.StartAttacking();
                boss.floatSine.Reset();
            };
            boss.Add(tween2);
            
            yield return null;
        } else {
            yield return new SwapImmediately(orig(boss, player, lastHit));
        }
    }
    public static void Load() {
        On.Celeste.FinalBoss.OnPlayer += customPlayer;
        IL.Celeste.FinalBoss.CreateBossSprite += giveCustomSprite;
        On.Celeste.FinalBoss.MoveSequence += customMoveSequence;
    }

    public static void Unload() {
        On.Celeste.FinalBoss.OnPlayer -= customPlayer;
        IL.Celeste.FinalBoss.CreateBossSprite -= giveCustomSprite;
        On.Celeste.FinalBoss.MoveSequence -= customMoveSequence;
    }
}