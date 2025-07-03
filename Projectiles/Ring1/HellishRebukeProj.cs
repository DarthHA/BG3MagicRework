using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class HellishRebukeProj : BaseMagicProj
    {
        public override int MaxHits => 1;
        public List<TmpParticle> Particles = new();
        public int NPCTarget = -1;
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

            if (Projectile.ai[0] == 0)
            {
                if (NPCTarget != -1 && (Main.npc[NPCTarget].CanBeChasedBy() || Main.npc[NPCTarget].immortal))
                {
                    Projectile.Center = Main.npc[NPCTarget].Center;
                }
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 20)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(30);
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 20);
                        Particles.NewParticle(Pos, Vel, 0.75f + Main.rand.NextFloat() * 0.75f);
                    }
                }
                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
            Particles.UpdateParticle(0.92f, 0.93f);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texLightField = TextureLibrary.LightField;
            Texture2D texLight = TextureLibrary.BloomFlare;
            if (Projectile.ai[0] == 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float alpha0 = MathHelper.Lerp(0.5f, 1f, Projectile.ai[1] / 20f);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha0, 0, texLight.Size() / 2f, 0.08f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * alpha0, 0, texLight.Size() / 2f, 0.06f, SpriteEffects.None, 0);
                Particles.DrawParticle(TextureLibrary.Extra, Color.OrangeRed, true, new Vector2(1, 1));
                Particles.DrawParticle(TextureLibrary.Extra, Color.White, true, new Vector2(1, 1) * 0.75f);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            else if (Projectile.ai[0] == 1)
            {
                float TargetRadius = 180;
                float radius = MathHelper.Lerp(30, TargetRadius, MathHelper.Clamp(Projectile.ai[1] / 30f, 0, 1));
                float light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 15f) / 10f, 0, 1));
                DrawRing(Projectile.Center, radius, 30, Color.Orange * light);
                DrawRing(Projectile.Center, radius, 20, Color.White * light);
                DrawRing(Projectile.Center, radius * 0.5f, 30, Color.Orange * light);
                DrawRing(Projectile.Center, radius * 0.5f, 20, Color.White * light);

                float scaleY = MathHelper.Lerp(4, 0, Projectile.ai[1] / 30f);
                float light2 = 1;
                if (Projectile.ai[1] < 5)
                {
                    light2 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 5f);
                }
                else if (Projectile.ai[1] > 15)
                {
                    light2 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 15) / 5f, 0f, 1f));
                }

                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.Orange * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.8f, 0.3f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.6f, 0.2f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Orange * light2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.09f, SpriteEffects.None, 0);
                Particles.DrawParticle(TextureLibrary.Extra, Color.OrangeRed, true, new Vector2(1, 1));
                Particles.DrawParticle(TextureLibrary.Extra, Color.White, true, new Vector2(1, 1) * 0.75f);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }

            return false;
        }

        public void DrawRing(Vector2 Center, float radius, float width, Color color)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 Pos1 = Center + rot.ToRotationVector2() * (radius + width / 2f);
                Vector2 Pos2 = Center + rot.ToRotationVector2() * (radius - width / 2f);
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius + width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 0, 1f)));
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius - width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 1, 1f)));
            float len = radius * MathHelper.TwoPi;
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, Projectile.ai[1] / 150f, BlendState.Additive);
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[0] == 1) return null;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float TargetRadius = 100;
            float radius = MathHelper.Lerp(30, TargetRadius, MathHelper.Clamp(Projectile.ai[1] / 30f, 0, 1));
            return targetHitbox.Distance(Projectile.Center) <= radius;
        }
    }
}
