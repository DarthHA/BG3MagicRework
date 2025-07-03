using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class SleetStormProj : BaseMagicProj
    {
        public override int MaxHits => -1;
        public List<IceDripParticle> iceParticles = new();
        public List<TmpParticle> tmpParticles = new();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 9999;
        }

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
            Projectile.DamageType = DamageClass.Magic;
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
            //前15帧冒光，后面下雨
            if (Projectile.ai[0] > 15 && Projectile.ai[0] < 120)
            {
                int count = (CurrentRing - 3) / 2 + 2;
                for (int i = 0; i < count; i++)
                {
                    float TopPosY = Math.Max(Main.screenPosition.Y - 30, Projectile.Center.Y - Main.screenHeight - 30);
                    if (TopPosY > Projectile.Center.Y - 30) TopPosY = Projectile.Center.Y - 30;
                    float X = Projectile.Center.X + (Main.rand.NextFloat() * 2 - 1) * GetSleetRadius();
                    Vector2 TopPos;
                    if (CarefulSpellMM)
                    {
                        TopPos = new(X, TopPosY);
                    }
                    else
                    {
                        TopPos = GetTileBlockedEndPos(new(X, Projectile.Center.Y), new(X, TopPosY));
                    }
                    Vector2 Vel = new Vector2(0, 1) * Main.rand.Next(20, 25);
                    if (Main.rand.NextBool(2))
                    {
                        iceParticles.NewParticle(TopPos, Vel, Main.rand.NextFloat() + 1f);
                    }
                    else
                    {
                        iceParticles.NewParticle(TopPos, Vel * 0.6f, Main.rand.NextFloat() + 1f, true);
                    }
                }
            }

            if (Projectile.ai[0] < 180)
            {
                SomeUtils.AddLightLine(Projectile.Center, Projectile.Center - new Vector2(0, 1200), Color.White, 12);
            }

            if (Projectile.ai[0] > 240)
            {
                Projectile.Kill();
            }

            //更新雨点
            iceParticles.UpdateParticle(Projectile.Center, !CarefulSpellMM, tmpParticles);
            tmpParticles.UpdateParticle(0.93f, 0.95f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            tmpParticles.DrawParticle(TextureLibrary.Extra, Color.White * 0.5f, false, new Vector2(1, 1));
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            iceParticles.DrawParticle();

            if (Projectile.ai[0] < 30)
            {
                float scale = MathHelper.Lerp(0, 1, Math.Clamp(Projectile.ai[0] / 5f, 0, 1));
                float light = 1f;
                if (Projectile.ai[0] < 20)
                {
                    light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 20) / 10f);
                }
                Vector2 TopPos = Projectile.Center - new Vector2(0, 1500);
                Vector2 unitY = new Vector2(1, 0);
                List<CustomVertexInfo> bars0 = new()
                {
                    new CustomVertexInfo(TopPos + unitY * (GetSleetRadius() + 16) * 0.4f * scale - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(TopPos - unitY * (GetSleetRadius() + 16) * 0.4f * scale - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(Projectile.Center + unitY * (GetSleetRadius() + 16) * scale - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center - unitY * (GetSleetRadius() + 16) * scale - Main.screenPosition, Color.White, new Vector3(1, 1f, 1)),
                };
                DrawUtils.DrawRoSLaser(Terraria.GameContent.TextureAssets.MagicPixel.Value, bars0, Color.SkyBlue * 0.5f * light, 0.4f, 1f, 0, BlendState.Additive);
            }
            if (Projectile.ai[0] > 20)
            {
                float light = 1f;
                if (Projectile.ai[0] < 40)
                {
                    light = MathHelper.Lerp(0, 1, (Projectile.ai[0] - 20) / 20f);
                }
                else if (Projectile.ai[0] > 140)
                {
                    light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 140) / 20f, 0, 1));
                }
                Vector2 TopPos = Projectile.Center - new Vector2(0, 1500);
                Vector2 unitY = new(1, 0);
                Vector2 BottomPos = GetTileBlockedEndPos(Projectile.Center, Projectile.Center + new Vector2(0, Main.screenHeight / 2f));
                List<CustomVertexInfo> bars0 = new()
                {
                    new CustomVertexInfo(TopPos + unitY * (GetSleetRadius() + 16) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(TopPos - unitY * (GetSleetRadius() + 16) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(BottomPos + unitY * (GetSleetRadius() + 16) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(BottomPos - unitY * (GetSleetRadius() + 16) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1)),
                };
                float lengthX = (GetAOERadius<SleetStormSpell>() * 16f) / GetSleetRadius() * 0.75f;
                DrawUtils.DrawRoSVert(TextureLibrary.Perlin, bars0, Color.SkyBlue * light, new Vector2(0.3f, 0.15f), new Vector2(2f, lengthX), new Vector2(-Projectile.ai[0] / 50f, 0), BlendState.Additive);
            }
            return false;
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.DeepAddChilled(GetTimeSpan<SleetStormSpell>() * 60);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (IceDripParticle particle in iceParticles)
            {
                if (targetHitbox.Contains(particle.Position.ToPoint())) return true;
            }
            return false;
        }

        private float GetSleetRadius()
        {
            return GetAOERadius<SleetStormSpell>() * 16 + (CurrentRing - 3) * 16 * 3;
        }

        public static Vector2 GetTileBlockedEndPos(Vector2 start, Vector2 end)
        {
            bool HitTile = true;
            float maxLen = end.Distance(start);
            if (maxLen <= 16) return end;
            float currentLen;
            float result = maxLen;
            for (currentLen = 16; currentLen <= maxLen; currentLen += 16)
            {
                Vector2 pos = start + Vector2.Normalize(end - start) * currentLen;
                if (!Collision.SolidCollision(pos, 1, 1))        //无物块
                {
                    HitTile = false;
                }
                else
                {
                    if (!HitTile)
                    {
                        result = currentLen;
                        break;
                    }
                }
            }
            if (HitTile) return start;
            return start + Vector2.Normalize(end - start) * result;
        }
    }
}
