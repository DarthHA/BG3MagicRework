using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class GlyphOfWardingExplosionProj : BaseMagicProj
    {
        public List<TmpParticle> tmpParticles = new();
        public List<FlameParticle> flameParticles = new();
        public List<SmokeParticle> smokeParticles = new();
        public List<float> IceRot = new();
        public List<float> IceScale = new();
        public List<int> IceFrame = new();
        public List<float> ArcRings = new();
        public DamageElement damageElement = DamageElement.None;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (damageElement == DamageElement.Fire)
            {
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 160; i++)
                    {
                        Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(5, 25);
                        float scale = 0.35f + Main.rand.NextFloat() * 0.35f;
                        flameParticles.NewParticle(Projectile.Center, ShootVel, scale);
                    }
                    for (int i = 0; i < 120; i++)
                    {
                        float rot = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 Pos = Projectile.Center + rot.ToRotationVector2() * Main.rand.Next(5, 45);
                        Vector2 Vel = rot.ToRotationVector2() * Main.rand.Next(10, 25);
                        float scale = 0.2f + Main.rand.NextFloat() * 0.5f;
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                }
                tmpParticles.UpdateParticle(0.95f, 0.95f);
                flameParticles.UpdateParticle(0.93f, !CarefulSpellMM);
                flameParticles.UpdateParticle(0.93f, !CarefulSpellMM);
            }
            else if (damageElement == DamageElement.Acid)
            {
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 120; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(20, 40);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.75f + 0.75f * Main.rand.NextFloat();
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                }
                tmpParticles.UpdateParticle(0.9f, 0.93f);
            }
            else if (damageElement == DamageElement.Cold)
            {
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(15, 40);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.5f + 0.5f * Main.rand.NextFloat();
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        IceRot.Add(Main.rand.NextFloat() * MathHelper.TwoPi);
                        IceScale.Add(1.5f + 2.5f * Main.rand.NextFloat());
                        IceFrame.Add(Main.rand.Next(3));
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 15;
                        smokeParticles.NewParticle(Projectile.Center, ShootVel, Main.rand.NextFloat() * 0.25f + 0.25f, Color.LightBlue);
                    }
                }
                tmpParticles.UpdateParticle(0.9f, 0.93f);
                smokeParticles.UpdateParticle();
            }
            else if (damageElement == DamageElement.Lightning)
            {
                if (Projectile.ai[1] == 1 ||
                    Projectile.ai[1] == 6 ||
                    Projectile.ai[1] == 11)
                {
                    ArcRings.Add(0);
                }
                for (int i = ArcRings.Count - 1; i >= 0; i--)
                {
                    ArcRings[i] += 0.05f;
                    if (ArcRings[i] >= 1)
                    {
                        ArcRings.RemoveAt(i);
                    }
                }
            }
            else if (damageElement == DamageElement.Thunder)
            {
                if (Projectile.ai[1] > 30) Projectile.Kill();
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 250);
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(15, 30);
                        float scale = 0.25f + Main.rand.NextFloat() * 0.5f;
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 150);
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(20, 80);
                        float scale = 0.25f + Main.rand.NextFloat() * 0.75f;
                        smokeParticles.NewParticle(Pos, Vel, scale, Color.White);
                    }

                }
                tmpParticles.UpdateParticle(0.9f, 0.95f);
                for (int i = 0; i < 2; i++) smokeParticles.UpdateParticle(0.75f);
            }
            if (Projectile.ai[1] > 30) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (damageElement == DamageElement.Fire)
            {
                Texture2D tex = TextureLibrary.Extra;
                EasyDraw.AnotherDraw(BlendState.Additive);
                flameParticles.DrawParticle();
                tmpParticles.DrawParticle(tex, Color.Orange, true, new Vector2(2, 1));
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            else if (damageElement == DamageElement.Acid)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                Texture2D tex = TextureLibrary.Extra;
                tmpParticles.DrawParticle(tex, Color.Green, false, new Vector2(1, 1));
                float radius = GetAOERadius<GlyphOfWardingSpell>() * 16f;
                float scale = MathHelper.Lerp(0.1f, 1.4f, MathHelper.Clamp(Projectile.ai[1] / 20, 0, 1));
                float alpha = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 5) / 25f, 0, 1));
                Color color1 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.Green);
                Color color2 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.GreenYellow);
                Color color3 = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, Color.Yellow);
                Draw710(Projectile.Center, radius * scale, -Projectile.ai[1] / 200f, color1 * alpha, Projectile.ai[1] / 200f);
                Draw710(Projectile.Center, radius * scale, -Projectile.ai[1] / 200f, color2 * alpha, Projectile.ai[1] / 200f + 1);
                Draw710(Projectile.Center, radius * scale, -Projectile.ai[1] / 200f, color3 * alpha, Projectile.ai[1] / 200f + 2);
            }
            else if (damageElement == DamageElement.Cold)
            {
                Texture2D texIceShard = TextureLibrary.IceShard;
                Texture2D LightTex = TextureLibrary.BloomFlare;
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
                smokeParticles.DrawParticle(false);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                float alpha = 1;
                if (Projectile.ai[1] < 5)
                {
                    alpha = MathHelper.Lerp(0, 1, Projectile.ai[1] / 5f);
                }
                else if (Projectile.ai[1] > 20)
                {
                    alpha = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 20) / 10f);
                }
                for (int i = 0; i < IceRot.Count; i++)
                {
                    Rectangle rect = new(0, texIceShard.Height / 3 * IceFrame[i], texIceShard.Width, texIceShard.Height / 3);
                    Vector2 origin = new(rect.Size().X / 2f, rect.Size().Y / 5);
                    Vector2 DrawPos = Projectile.Center + IceRot[i].ToRotationVector2();
                    Main.spriteBatch.Draw(texIceShard, DrawPos - Main.screenPosition, rect, Color.Black * 0.45f * alpha, IceRot[i] - MathHelper.Pi / 2f, origin, IceScale[i], SpriteEffects.None, 0);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                for (int i = 0; i < IceRot.Count; i++)
                {
                    Rectangle rect = new(0, texIceShard.Height / 3 * IceFrame[i], texIceShard.Width, texIceShard.Height / 3);
                    Vector2 origin = new(rect.Size().X / 2f, rect.Size().Y / 5);
                    Vector2 DrawPos = Projectile.Center + IceRot[i].ToRotationVector2();
                    Main.spriteBatch.Draw(texIceShard, DrawPos - Main.screenPosition, rect, Color.White * 0.75f * alpha, IceRot[i] - MathHelper.Pi / 2f, origin, IceScale[i], SpriteEffects.None, 0);
                }
                if (Projectile.ai[1] <= 15)
                {
                    float r = Projectile.rotation;
                    float scale = 4;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 15f);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Cyan * light, Projectile.rotation, LightTex.Size() / 2f, 0.2f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, LightTex.Size() / 2f, 0.1f * scale, SpriteEffects.None, 0);
                }

                if (Projectile.ai[1] <= 20)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 20f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 1.5f, SpriteEffects.None, 0);
                }
                Texture2D tex = TextureLibrary.Extra;
                tmpParticles.DrawParticle(tex, Color.Cyan, true, new Vector2(1, 1));
            }
            else if (damageElement == DamageElement.Lightning)
            {
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                float alpha = 1;
                if (Projectile.ai[1] > 10) alpha = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 10) / 20f);
                for (int i = 0; i < 3; i++)
                {
                    ArcSegments segs = new();
                    Vector2 End = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(50, 300);
                    segs.GenerateSegs(Projectile.Center, End, new Vector2(80, 40), 60f);
                    segs.DrawSegs(Color.Blue * alpha);
                }
                Texture2D LightTex = TextureLibrary.BloomFlare;
                EasyDraw.AnotherDraw(BlendState.Additive);
                if (Projectile.ai[1] <= 15)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 15f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 15f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 1.25f, SpriteEffects.None, 0);
                }

                foreach (float r in ArcRings)
                {
                    float alpha1 = 1;
                    if (r > 0.2)
                    {
                        alpha1 = MathHelper.Lerp(1, 0, (r - 0.2f) / 0.8f);
                    }
                    float radius = (float)Math.Sqrt(r) * 300;
                    DrawRing(Projectile.Center, radius, 60, Color.Blue * alpha1);
                    DrawRing(Projectile.Center, radius, 40, Color.White * alpha1);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Blue * alpha, 0, LightTex.Size() / 2f, 0.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, 0, LightTex.Size() / 2f, 0.15f, SpriteEffects.None, 0);
            }
            else if (damageElement == DamageElement.Thunder)
            {
                Texture2D tex = TextureLibrary.Extra;
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
                smokeParticles.DrawParticle(false);
                EasyDraw.AnotherDraw(BlendState.Additive);
                if (Projectile.ai[1] <= 20)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 20f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 1.25f, SpriteEffects.None, 0);
                }
                tmpParticles.DrawParticle(tex, Color.White, false, new Vector2(1, 1));
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void SafeModifyHit(NPC target, ref NPC.HitModifiers modifiers, ref DiceDamage diceUsed, ref float damageModifier, ref Dictionary<DamageElement, float> resistance)
        {
            diceUsed.ChangeUnknownElement(damageElement);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 16 * GetAOERadius<GlyphOfWardingSpell>() &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.2f, Projectile.ai[1] / 300f, BlendState.Additive);
        }
    }
}
