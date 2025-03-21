using System.Collections.Generic;
using System.Text.RegularExpressions;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/ClearFlagsTrigger")]
public class ClearFlagsTrigger : Trigger {
    private readonly Regex matcher;
    private readonly bool onEnter;
    public ClearFlagsTrigger(EntityData data, Vector2 offset)
        : base(data, offset) {
        this.matcher = new Regex(data.String("matcher", ""));
        this.onEnter = data.Bool("onEnter",true);
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        if (this.onEnter) this.doMatch(player.level.Session);
    }

    public override void OnLeave(Player player) {
        base.OnLeave(player);
        if (!this.onEnter) this.doMatch(player.level.Session);
    }

    private void doMatch(Session session) {
        var flagsToChange = new HashSet<string>();
        foreach (var flag in session.Flags) {
            if (this.matcher.IsMatch(flag)) flagsToChange.Add(flag);
        }
        foreach (var flag in flagsToChange) {
            session.SetFlag(flag, false);
        }
    }
}