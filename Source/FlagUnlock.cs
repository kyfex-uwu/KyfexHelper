using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using Alarm = IL.Monocle.Alarm;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/FlagUnlock")]
public class FlagUnlock : Entity {
    private readonly string unlockSfx;
    private readonly bool requireUnobstructed;
    private readonly string[] unlockables;
    private readonly EntityID ID;
    private readonly bool temporary;
    private readonly bool shake;
    private readonly string flagToSet;
    private readonly float time;

    private readonly Sprite sprite = null;

    private static readonly HashSet<Follower> currentlyUsing = new();
    private bool opening = false;
    
    public static readonly Dictionary<Type, Func<FlagUnlock, Follower, Coroutine>> unlockRoutines = new();
    public static readonly Dictionary<Type, Action<FlagUnlock, Follower>> onUnlockActions = new();
    
    public FlagUnlock(EntityData data, Vector2 offset, EntityID id) : base(data.Position + offset) {
        this.ID = id;
        this.unlockSfx = data.String("unlockSfx","");
        this.unlockables = data.String("unlockables", "Celeste.Key").Split(",");
        this.requireUnobstructed = data.Bool("requireUnobstructed", true);
        this.temporary = data.Bool("temporary", false);
        this.shake = data.Bool("shake", true);
        this.flagToSet = data.String("flag", "");
        this.time = data.Float("time", 1);

        var sprite = data.String("sprite","");
        if(sprite!="" && GFX.SpriteBank.Has(sprite)) this.Add(this.sprite = GFX.SpriteBank.Create(sprite));
        this.Add(new PlayerCollider(this.OnPlayer, new Circle(data.Float("radius", 60f))));
    }
    
    public void OnPlayer(Player player) {
        if (this.opening) return;
        foreach (Follower follower in player.Leader.Followers) {
            if (unlockables.Contains(follower.Entity.GetType().FullName) && 
                !currentlyUsing.Contains(follower) &&
                (!(follower.Entity is Key) || !((Key)follower.Entity).StartedUsing)) {//this could be causing errors
                this.TryOpen(player, follower);
                break;
            }
        }
    }
    public void TryOpen(Player player, Follower follower) {
        if (!this.requireUnobstructed || !this.Scene.CollideCheck<Solid>(player.Center, this.Center)) {
            this.opening = true;
            currentlyUsing.Add(follower);
            
            this.Add(new Coroutine(this.UnlockRoutine(follower)));
        }
    }
    
    public IEnumerator UnlockRoutine(Follower follower) {
        SoundEmitter emitter=null;

        Level level = this.SceneAs<Level>();
        if (unlockRoutines.ContainsKey(follower.Entity.GetType())) {
            var routine = unlockRoutines[follower.Entity.GetType()].Invoke(this, follower);
            this.Add(routine);
            while (!routine.Finished) yield return null;
        } else {
            var entity = follower.Entity;
            
            //move to lock
            var curve = new SimpleCurve(entity.Position, this.Center, (this.Center + entity.Position) / 2f + new Vector2(0.0f, -48f));
            var tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, this.time, true);
            tween.OnUpdate = t => {
                entity.Position = curve.GetPoint(t.Eased);
            };
            entity.Add(tween);
            yield return tween.Wait();
            
            //insert "key"
            if(this.shake) Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            for (int index = 0; index < 16; ++index) {
                entity.SceneAs<Level>().ParticlesFG.Emit(Key.P_Insert, entity.Center, 0.3926991f * index);
            }

            var tween2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.Linear, 0.5f, true);
            tween2.OnUpdate = t => {
                entity.Position = this.Center;
            };
            yield return tween2.Wait();
            
            //remove "key"
            Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            for (int index = 0; index < 8; ++index) {
                entity.SceneAs<Level>().ParticlesFG.Emit(Key.P_Insert, entity.Center, 0.7853982f * index);
            }
            if (this.unlockSfx != "") {
                emitter = SoundEmitter.Play(this.unlockSfx, this);
                emitter.Source.DisposeOnTransition = true;
            }

            entity.RemoveSelf();
        }

        if (!this.temporary) {
            level.Session.DoNotLoad.Add(this.ID);
        }

        if (onUnlockActions.ContainsKey(follower.Entity.GetType())) {
            onUnlockActions[follower.Entity.GetType()].Invoke(this, follower);
        } else {
            var field = follower.Entity.GetType().GetField("ID");
            if (field!=null && field.FieldType == typeof(EntityID))
                level.Session.DoNotLoad.Add((EntityID)field.GetValue(follower.Entity));
        }
        
        this.Tag |= (int) Tags.TransitionUpdate;
        
        if(emitter!=null) emitter.Source.DisposeOnTransition = false;
        
        if(this.sprite!=null && this.sprite.animations["open"]!=null) 
            yield return this.sprite.PlayRoutine("open");
        
        if (this.shake) {
            level.Shake();
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        }
        
        if(this.sprite!=null && this.sprite.animations["burst"]!=null) 
            yield return this.sprite.PlayRoutine("burst");
        
        level.Session.SetFlag(this.flagToSet);//todo: make temporary work
        this.RemoveSelf();
    }
}