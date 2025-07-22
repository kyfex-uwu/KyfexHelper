using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Celeste.Mod.Entities;
using Monocle;
using MonoMod.Cil;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/CustomCollidableBadeline")]
public class CustomCollidableBadeline : FinalBoss{
    public static Dictionary<Type, Action<Entity>> onKill = new ();
    public static Dictionary<Type, Action<Entity>> onBounce = new ();
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
    public CustomCollidableBadeline(EntityData data, Vector2 offset) : base(data, offset) {
        this.playerHitType = data.Enum("playerHitType", HitType.DAMAGE);
        this.holdableHitType = data.Enum("holdableHitType", HitType.DAMAGE);
        this.customSprite = data.String("customSprite", "badeline_boss");
        this.flagList = data.String("flagList").Split(",");
        if (this.flagList.Length == 1 && this.flagList[0] == "") this.flagList = [];
        
        this.Add(new HoldableCollider(this.OnHoldable));
    }

    private void OnHoldable(Holdable holdable) {
        this.collide(holdable.Entity, this.holdableHitType);
    }

    private bool trueOnPlayer = false;
    private void collide(Entity entity, HitType type) {
        switch (type) {
            case HitType.NONE: break;
            case HitType.KILL: {
                if (entity is Player playerEntity)
                    playerEntity.Die(-playerEntity.Speed);
                else if (entity is TheoCrystal crystalEntity)
                    crystalEntity.Die();
                else if (onKill.TryGetValue(entity.GetType(), out var run)) run(entity);
                else entity.RemoveSelf();
            }break;
            case HitType.BOUNCE_AWAY: {
                if (entity is Player playerEntity)
                    playerEntity.PointBounce(this.Center);
                else if (entity is TheoCrystal crystalEntity)
                    crystalEntity.Speed = (crystalEntity.Center - this.Center).SafeNormalize(120f);
                else if (onBounce.TryGetValue(entity.GetType(), out var run)) run(entity);
            }break;
            case HitType.DAMAGE: {
                this.trueOnPlayer = true;
                if(this.nodeIndex<this.flagList.Length)
                    this.SceneAs<Level>().Session.SetFlag(this.flagList[this.nodeIndex]);
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
    public static void Load() {
        On.Celeste.FinalBoss.OnPlayer += customPlayer;
        IL.Celeste.FinalBoss.CreateBossSprite += giveCustomSprite;
    }

    public static void Unload() {
        On.Celeste.FinalBoss.OnPlayer -= customPlayer;
        IL.Celeste.FinalBoss.CreateBossSprite += giveCustomSprite;
    }
}