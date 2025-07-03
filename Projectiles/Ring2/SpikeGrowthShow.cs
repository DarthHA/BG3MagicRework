using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class SpikeGrowthShow : BaseMagicProj
    {
        public int numVines = 10;
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

            if (Projectile.ai[0] == 40)    //20
            {
                int protmp = owner.NewMagicProj(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpikeGrowthProj>(), diceDamage, 0, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as SpikeGrowthProj).numVines = numVines;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                    BaseConcentration con = owner.GenerateConcentration<ConSpikeGrowth>(CurrentRing, GetTimeSpan<SpikeGrowthSpell>() * 60, true);
                    if (con != null)
                    {
                        con.projIndex = protmp;
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                    }
                }
            }

            if (Projectile.ai[0] > 60) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float TargetRadius = SpikeGrowthProj.GetRadius(numVines) * 1.3f;
            float radius = MathHelper.Lerp(10, TargetRadius, MathHelper.Clamp(Projectile.ai[0] / 50f, 0, 1));
            float light1 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 30f) / 30f, 0, 1));
            DrawRing(Projectile.Center, radius, 40, Color.Brown * light1);
            DrawRing(Projectile.Center, radius, 20, Color.White * light1);
            DrawRing(Projectile.Center, radius * 0.6f, 40 * 0.8f, Color.Brown * light1);
            DrawRing(Projectile.Center, radius * 0.6f, 20 * 0.8f, Color.White * light1);
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
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 1f, 0.3f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.8f, 0.2f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * light2, 0, texLight.Size() / 2f, 0.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            if (Projectile.ai[0] > 30)
            {
                float light3 = 1;
                if (Projectile.ai[0] < 35)
                {
                    light3 = MathHelper.Lerp(0, 1, (Projectile.ai[0] - 30) / 5f);
                }
                else if (Projectile.ai[0] > 45)
                {
                    light3 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 45) / 15f);
                }
                Texture2D tex = TextureLibrary.LightFieldVert;
                List<CustomVertexInfo> bars1 = new();
                for (int i = 0; i <= 60; i++)
                {
                    float rot = MathHelper.TwoPi / 60f * i;
                    Vector2 R = rot.ToRotationVector2();
                    Vector2 Pos1 = Projectile.Center + R * SpikeGrowthProj.GetRadius(numVines) * 0.9f + new Vector2(0, -1) * 240;
                    Vector2 Pos2 = Projectile.Center + R * SpikeGrowthProj.GetRadius(numVines) * 0.9f + new Vector2(0, 1) * 240;
                    bars1.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0f, 1f)));
                    bars1.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1f, 1f)));
                }
                DrawUtils.DrawLoopTrail(tex, bars1, Color.DarkOrange * light3, 0.33f, 0f, BlendState.Additive);
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, Projectile.ai[0] / 300f, BlendState.Additive);
        }


    }
}