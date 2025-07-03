using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class ColorSprayProj : BaseMagicProj
    {
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

            if (Projectile.ai[0] < 10f)
                Lighting.AddLight(Projectile.Center, 5, 5, 5);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D texLightField = TextureLibrary.LightField;
            Texture2D texBloomFlare = TextureLibrary.BloomFlare;
            List<CustomVertexInfo> bars = new();
            Vector2 UnitX = Vector2.Normalize(Projectile.velocity);
            Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
            float light1 = 1;
            if (Projectile.ai[0] < 10) light1 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 10f);
            if (Projectile.ai[0] > 10) light1 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 10f) / 50f);
            List<Color> colors = new() { Color.Red, Color.Yellow, Color.White, Color.Green, Color.Blue };
            for (int i = 0; i < 5; i++)
            {
                float r0 = -MathHelper.Pi / 12f + MathHelper.Pi / 24f * i;
                for (int j = 0; j < 20; j++)
                {
                    float r1 = Projectile.velocity.ToRotation() + r0 - MathHelper.Pi / 4f + MathHelper.Pi / 2f * j / 20f;
                    Vector2 EndPos;
                    if (CarefulSpellMM)
                    {
                        EndPos = Projectile.Center + r1.ToRotationVector2() * 400;
                    }
                    else
                    {
                        EndPos = SomeUtils.GetTileBlockedEndPos(Projectile.Center, Projectile.Center + r1.ToRotationVector2() * 400);
                    }
                    bars.Add(new CustomVertexInfo(Projectile.Center + r1.ToRotationVector2() * 1 - Main.screenPosition, Color.White, new Vector3(0, j / 20f, 1)));
                    bars.Add(new CustomVertexInfo(EndPos - Main.screenPosition, Color.White, new Vector3(1, j / 20f, 1)));
                }
                DrawUtils.DrawTrail(texLightField, bars, colors[i] * light1, BlendState.Additive);
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            float scale2 = 0;
            if (Projectile.ai[0] < 50) scale2 = MathHelper.Lerp(1, 0, Projectile.ai[0] / 50f);
            float light2 = 0.5f;
            if (Projectile.ai[0] > 40) light2 = MathHelper.Lerp(0.5f, 0, (Projectile.ai[0] - 40) / 20f);
            for (int i = 0; i < 5; i++)
            {
                Vector2 r = Projectile.Center + (i * MathHelper.TwoPi / 5f).ToRotationVector2() * 20f;
                Main.spriteBatch.Draw(texBloomFlare, r - Main.screenPosition, null, colors[i] * light2, 0, texBloomFlare.Size() / 2f, 0.6f * scale2, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(texBloomFlare, Projectile.Center - Main.screenPosition, null, Color.White * light2 * 2f, 0, texBloomFlare.Size() / 2f, 0.4f * scale2, SpriteEffects.None, 0);
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

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            this.DeepAddCCBuffByDC(target, ModContent.BuffType<BlindedDNDBuff>(), GetTimeSpan<ColorSpraySpell>() * 60);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 v1 = Vector2.Normalize((targetHitbox.Center - projHitbox.Center).ToVector2());
            Vector2 v2 = Vector2.Normalize(Projectile.velocity);
            float a = v1.X * v2.X + v1.Y * v2.Y;
            return targetHitbox.Distance(Projectile.Center) < 16 * GetAOERadius<ColorSpraySpell>() && a > 0.5 &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }
    }
}