using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class BurningHandsProj : BaseMagicProj
    {
        public List<TmpParticle> tmpParticles = new();
        public List<TmpParticle> tmpParticles2 = new();
        public List<FlameParticle> flameParticles = new();
        public override int MaxHits => -1;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 60) Projectile.Kill();
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 60; i++)
                {
                    Vector2 ShootVel = Vector2.Normalize(Projectile.velocity);
                    ShootVel = ShootVel.RotatedBy(MathHelper.Pi / 3f * Main.rand.NextFloat() * 2 - MathHelper.Pi / 3f) * Main.rand.Next(5, 30);
                    float scale = 0.35f + Main.rand.NextFloat() * 0.35f;
                    flameParticles.NewParticle(Projectile.Center, ShootVel, scale);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(5, 10);
                    float scale = 0.35f + Main.rand.NextFloat() * 0.35f;
                    tmpParticles2.NewParticle(Projectile.Center, ShootVel, scale);
                }
                for (int i = 0; i < 50; i++)
                {
                    float rot = MathHelper.Pi / 3 * (Main.rand.NextFloat() * 2 - 1);
                    Vector2 Pos = Projectile.Center + rot.ToRotationVector2() * Main.rand.Next(5, 45);
                    Vector2 Vel = Vector2.Normalize(Projectile.velocity).RotatedBy(rot) * Main.rand.Next(10, 25);
                    float scale = 0.2f + Main.rand.NextFloat() * 0.5f;
                    tmpParticles.NewParticle(Pos, Vel, scale);
                }
            }
            if (Projectile.ai[0] < 10f)
                Lighting.AddLight(Projectile.Center, 5, 5, 5);
            //¸üÐÂÁ£×Ó
            tmpParticles.UpdateParticle(0.95f, 0.95f);
            tmpParticles2.UpdateParticle(0.93f, 0.93f);
            for (int i = 0; i < 2; i++) flameParticles.UpdateParticle(0.93f, !CarefulSpellMM);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureLibrary.Extra;
            EasyDraw.AnotherDraw(BlendState.Additive);
            flameParticles.DrawParticle();
            tmpParticles.DrawParticle(tex, Color.Orange, true, new Vector2(2, 1));
            tmpParticles2.DrawParticle(tex, Color.Orange, true, new Vector2(2, 1));
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] >= 5) return false;
            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 v1 = Vector2.Normalize((targetHitbox.Center - projHitbox.Center).ToVector2());
            Vector2 v2 = Vector2.Normalize(Projectile.velocity);
            float a = v1.X * v2.X + v1.Y * v2.Y;
            return targetHitbox.Distance(Projectile.Center) < 16 * GetAOERadius<BurningHandsSpell>() && a > 0.5 &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }
    }
}