using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class LightningBoltProj : BaseMagicProj
    {
        public List<ArcSegments> Arcs = new();
        public Vector2 RelaPos = Vector2.Zero;
        public int TargetNPC = -1;

        public override int MaxHits => -1;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 99999;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Arcs.Clear();
            for (int i = 0; i < 3; i++)
            {
                ArcSegments arc = new();
                arc.GenerateSegs(owner.Center + RelaPos, Projectile.Center, new Vector2(80, 40), 30);
                Arcs.Add(arc);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 30)
            {
                Projectile.Kill();
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            foreach (ArcSegments arc in Arcs)
            {
                arc.DrawSegs(Color.Blue, 10);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player owner = Main.player[Projectile.owner];
            float tmp = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, Projectile.Center, 80, ref tmp) &&
                (CarefulSpellMM || Collision.CanHit(owner.Center + RelaPos, 1, 1, targetHitbox.Top(), targetHitbox.Width, targetHitbox.Height));
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
