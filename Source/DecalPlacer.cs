using System;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/DecalPlacer")]
public class DecalPlacer : Entity {
    private record DecalData(string name, double weight) { };

    private readonly DecalData[] data;
    private readonly int count;
    private readonly int depth;
    private readonly Rectangle rect;
    private readonly Random random;
    public DecalPlacer(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.data = data.String("decals", "").Split(",").Select(str => {
            var data = str.Split(":");
            if (data.Length >= 2)
                return new DecalData(data[0], double.TryParse(data[1], out var num) ? num : 0);
            return new DecalData("", 0);
        }).ToArray();

        this.count = data.Int("count");
        this.depth = data.Int("depth");
        this.random = new Random(data.Int("random")+(int)this.X*420+(int)this.Y);

        this.rect = new Rectangle((int) this.X, (int) this.Y, data.Width, data.Height);
    }
    
    public override void Added(Scene scene) {
        var area = this.rect.Width * this.rect.Height;
        double total = 0;
        foreach (var decal in this.data) total += decal.weight;
        
        for (int i = 0; i < this.count; i++) {
            int pos = this.random.Next(0, area);
            var which = this.random.NextDouble()*total;

            foreach (var decal in this.data) {
                if (which < decal.weight) {
                    scene.Add(new Decal(decal.name, 
                        this.Position + new Vector2((int) Math.Floor((float) pos/this.rect.Height), pos%this.rect.Height), 
                        Vector2.One, this.depth));
                    break;
                }

                which -= decal.weight;
            }
        }

        this.RemoveSelf();
    }
}