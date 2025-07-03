using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
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
    public class GreaseProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] == 20)
            {
                int count = 30 + 10 * (CurrentRing - 1);
                for (int i = 0; i < count; i++)
                {
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(20, 40) * (1f + 0.25f * (CurrentRing - 1));
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                    float scale = 0.5f + 0.5f * Main.rand.NextFloat();
                    Particles.NewParticle(Pos, Vel, scale);
                }
            }
            Particles.UpdateParticle(0.9f, 0.93f);
            if (Projectile.ai[0] > 40) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D tex = TextureLibrary.Extra;
            Particles.DrawParticle(tex, Color.Orange * 0.5f, false, new Vector2(1, 1));

            if (Projectile.ai[0] < 30)
            {
                float scaleY = 1;
                if (Projectile.ai[0] < 20)
                {
                    scaleY = MathHelper.Lerp(3, 1, Projectile.ai[0] / 20f);
                }
                float light2 = 1;
                if (Projectile.ai[0] < 5)
                {
                    light2 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 5f);
                }
                else if (Projectile.ai[0] > 20)
                {
                    light2 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 20) / 10f);
                }
                Texture2D texLightField = TextureLibrary.LightField;
                Texture2D texLight = TextureLibrary.BloomFlare;
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.6f, 0.2f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.5f, 0.15f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * light2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.1f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }

            float radius = (GetAOERadius<GreaseSpell>() + 4 * (CurrentRing - 1)) * 16f;
            float scale = MathHelper.Lerp(0, 1.25f, MathHelper.Clamp((Projectile.ai[0] - 10) / 30, 0, 1));
            float alpha = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 20) / 20, 0, 1));
            Color color1 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.Orange);
            Color color2 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.OrangeRed);
            Color color3 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.Yellow);
            Draw710(Projectile.Center, radius * scale, -Projectile.ai[0] / 200f, color1 * alpha * 0.75f, Projectile.ai[0] / 200f);
            Draw710(Projectile.Center, radius * scale, -Projectile.ai[0] / 200f, color2 * alpha * 0.75f, Projectile.ai[0] / 200f + 1);
            Draw710(Projectile.Center, radius * scale, -Projectile.ai[0] / 200f, color3 * alpha * 0.75f, Projectile.ai[0] / 200f + 2);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float radius = (GetAOERadius<GreaseSpell>() + 4 * (CurrentRing - 1)) * 16f;
            radius *= MathHelper.Lerp(0, 1, MathHelper.Clamp((Projectile.ai[0] - 10) / 30, 0, 1));
            return targetHitbox.Distance(Projectile.Center) < radius
                && (CarefulSpellMM || Collision.CanHit(targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height, Projectile.Center, 1, 1));
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddNormalBuff(ModContent.BuffType<GreaseBuff>(), GetTimeSpan<GreaseSpell>() * 60);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void Draw710(Vector2 Center, float radius, float progress, Color color, float rot = 0)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rot;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars, color, 0.2f, 0.6f, progress, BlendState.Additive);
        }
    }
}