using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class PhantasmalForceDamageProj : BaseMagicProj
    {
        public int TargetNPC = -1;
        public List<TmpParticle> Particles = new();
        private int miscTimer = 0;
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
            Projectile.localNPCHitCooldown = 60;
        }



        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            if (TargetNPC == -1 || (!Main.npc[TargetNPC].CanBeChasedBy() && !Main.npc[TargetNPC].immortal))
            {
                Projectile.Kill();
                return;
            }
            NPC Target = Main.npc[TargetNPC];
            Projectile.Center = Target.Center;

            if (Projectile.ai[0] != 2)
            {
                //断专注就会消失

                if (owner.GetConcentration(ConUUID) == -1)
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                    return;
                }

                if (owner.Distance(Target.Center) > GetSpellRange<PhantasmalForceSpell>() * 16f * 2f)
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                    Projectile.Kill();
                    return;
                }
                //距离过远或者无法看见也会消失
            }

            if (Projectile.ai[0] == 0)        //潜伏状态
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)   //判定
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 30)
                {
                    Particles.Clear();
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(5, 20);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.3f + 0.3f * Main.rand.NextFloat();
                        Particles.NewParticle(Pos, Vel, scale);
                    }
                }
                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 2)        //消失
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 30)
                {
                    Projectile.Kill();
                }
            }
            Particles.UpdateParticle(0.9f, 0.99f);
            miscTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            if (Projectile.ai[0] == 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float alpha0 = 0.5f + (float)Math.Sin(Projectile.ai[1] * MathHelper.TwoPi / 120) * 0.25f;
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha0, miscTimer / 50f, LightTex.Size() / 2f, 0.08f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * alpha0, miscTimer / 50f, LightTex.Size() / 2f, 0.06f, SpriteEffects.None, 0);
            }
            else if (Projectile.ai[0] == 1)
            {
                float alpha0 = 0; float alpha1 = 0.5f;
                if (Projectile.ai[1] < 30)
                {
                    alpha0 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 30f);
                    alpha1 = MathHelper.Lerp(0.5f, 1, Projectile.ai[1] / 30f);
                }
                else if (Projectile.ai[1] < 40)
                {
                    alpha0 = 1;
                    alpha1 = 1;
                }
                else if (Projectile.ai[1] < 50)
                {
                    alpha0 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 40) / 10f);
                    alpha1 = MathHelper.Lerp(1, 0.5f, (Projectile.ai[1] - 40) / 10f);
                }

                float t = miscTimer / 50f;
                for (int i = 0; i < 3; i++)
                {
                    List<CustomVertexInfo> bars = new();
                    Vector2 DeltaOffset = (-new Vector2(0, 20 * (float)Math.Sin(t))).RotatedBy(MathHelper.TwoPi / 3f * i + t);
                    for (int k = 0; k < 20; k++)
                    {
                        float x = MathHelper.Lerp(-80, 0, k / 19f);
                        float deltaY = 12 * (float)Math.Sin(t + k / 20f * MathHelper.TwoPi);
                        bars.Add(new CustomVertexInfo(Projectile.Center + DeltaOffset + new Vector2(x, -12 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + t) - Main.screenPosition, Color.White, new Vector3(k / 19f, 0f, 1)));
                        bars.Add(new CustomVertexInfo(Projectile.Center + DeltaOffset + new Vector2(x, 12 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + t) - Main.screenPosition, Color.White, new Vector3(k / 19f, 1f, 1)));
                    }
                    DrawUtils.DrawRoSLaser(texRibbon, bars, Color.Purple * alpha0, 0.4f, 1f, -t + 0.4f * i, BlendState.Additive);
                }

                for (int i = 0; i < 3; i++)
                {
                    List<CustomVertexInfo> bars = new();
                    Vector2 DeltaOffset = (-new Vector2(0, 20 * (float)Math.Sin(t))).RotatedBy(MathHelper.TwoPi / 3f * i + t);
                    for (int k = 0; k < 20; k++)
                    {
                        float x = MathHelper.Lerp(-80, 0, k / 19f);
                        float deltaY = 12 * (float)Math.Sin(t + k / 20f * MathHelper.TwoPi);
                        bars.Add(new CustomVertexInfo(Projectile.Center + DeltaOffset + new Vector2(x, -9 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + t) - Main.screenPosition, Color.White, new Vector3(k / 19f, 0f, 1)));
                        bars.Add(new CustomVertexInfo(Projectile.Center + DeltaOffset + new Vector2(x, 9 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + t) - Main.screenPosition, Color.White, new Vector3(k / 19f, 1f, 1)));
                    }
                    DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * alpha0, 0.4f, 1f, -t + 0.4f * i, BlendState.Additive);
                }

                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha1, t, LightTex.Size() / 2f, 0.08f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * alpha1, t, LightTex.Size() / 2f, 0.06f, SpriteEffects.None, 0);

                if (Projectile.ai[1] >= 30)          //Boom
                {
                    float alpha2 = 0;
                    if (Projectile.ai[1] < 35)
                    {
                        alpha2 = MathHelper.Lerp(0, 1, (Projectile.ai[1] - 30) / 5f);
                    }
                    else if (Projectile.ai[1] < 50)
                    {
                        alpha2 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 35) / 15f);
                    }
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha2, t, LightTex.Size() / 2f, 0.4f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * alpha2, t, LightTex.Size() / 2f, 0.3f, SpriteEffects.None, 0);
                }
            }
            else if (Projectile.ai[0] == 2)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float alpha0 = MathHelper.Lerp(1f, 0, Projectile.ai[1] / 30f);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha0, miscTimer / 50f, LightTex.Size() / 2f, 0.08f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * alpha0, miscTimer / 50f, LightTex.Size() / 2f, 0.06f, SpriteEffects.None, 0);
            }
            Particles.DrawParticle(TextureLibrary.Extra, Color.White, true, new Vector2(2, 0.5f));
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (TargetNPC == -1 || target.whoAmI != TargetNPC) return false;
            if (Projectile.ai[0] == 1 && Projectile.ai[1] == 30) return null;
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}
