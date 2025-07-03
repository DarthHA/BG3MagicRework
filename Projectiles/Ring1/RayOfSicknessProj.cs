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
    public class RayOfSicknessProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public Vector2 TargetPos = Vector2.Zero;
        public Vector2 RelaPos = Vector2.Zero;
        public override int MaxHits => 1;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
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
            //Projectile.Center = owner.Center + RelaPos;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 60) Projectile.Kill();
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 15; i++)
                {
                    float r = (TargetPos - Projectile.Center).ToRotation() + (Main.rand.NextFloat() * MathHelper.Pi - MathHelper.Pi / 2f);
                    Vector2 Pos = TargetPos + r.ToRotationVector2() * Main.rand.Next(1, 8);
                    Vector2 Vel = r.ToRotationVector2() * Main.rand.Next(5, 30);
                    float scale = 0.5f + Main.rand.NextFloat() * 0.75f;
                    Particles.NewParticle(Pos, Vel, scale);
                }
            }
            if (Projectile.ai[0] < 10f)
                Lighting.AddLight(Projectile.Center, 5, 5, 5);
            //更新粒子
            Particles.UpdateParticle(0.92f, 0.95f);

        }


        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D tex = TextureLibrary.Extra;
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D texBlobGlow2 = TextureLibrary.BlobGlow2;
            Texture2D texBloomFlare = TextureLibrary.BloomFlare;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;

            EasyDraw.AnotherDraw(BlendState.Additive);
            Vector2 unitX = Vector2.Normalize(TargetPos - Projectile.Center);
            Vector2 unitY = unitX.RotatedBy(MathHelper.Pi / 2f);
            float a = 0;
            if (Projectile.ai[0] >= 5 && Projectile.ai[0] <= 35)
            {
                a = MathHelper.Lerp(1.5f, 0, (Projectile.ai[0] - 5f) / 30f);     //正经激光
            }
            List<CustomVertexInfo> bars3 = new()
                {
                new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * 15 - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * 15 - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 + unitY * 12 - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 - unitY * 12 - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            List<CustomVertexInfo> bars4 = new()
                {
                new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * 15 - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * 15 - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 + unitY * 20 - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 - unitY * 20 - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            List<CustomVertexInfo> bars5 = new()
                {
                new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * 30 - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * 30 - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 + unitY * 30 - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 - unitY * 30 - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };

            DrawUtils.DrawRoSLaser(texRibbon, bars5, Color.Red * a * 1.1f, 0.4f, 1f, -Projectile.ai[0] / 240f, DrawUtils.ReverseSubtract);
            DrawUtils.DrawRoSLaser(texRibbon, bars4, Color.Green * a, 0.4f, 1f, -Projectile.ai[0] / 240f, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, bars3, Color.White * a, 0.4f, 1f, -Projectile.ai[0] / 240f, BlendState.Additive);


            EasyDraw.AnotherDraw(BlendState.Additive);
            Particles.DrawParticle(tex, Color.Green, true, new Vector2(2, 1));

            if (Projectile.ai[0] <= 7)
            {
                float length = MathHelper.Lerp(40, 15, Projectile.ai[0] / 7f);          //曳光
                List<CustomVertexInfo> bars1 = new()
                {
                    new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * length * 0.9f - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * length * 0.9f - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(TargetPos + unitX * 20 + unitY * length * 0.9f - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(TargetPos + unitX * 20 - unitY * length * 0.9f - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
                List<CustomVertexInfo> bars2 = new()
                {
                    new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * length - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * length - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(TargetPos + unitX * 20 + unitY * length - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(TargetPos + unitX * 20 - unitY * length - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
                DrawUtils.DrawTrail(texBlobGlow2, bars1, Color.White, BlendState.Additive);
                DrawUtils.DrawTrail(texBlobGlow2, bars2, Color.Green, BlendState.Additive);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texBloomFlare, Projectile.Center + unitX * 20 - Main.screenPosition, null, Color.Green, 0, texBloomFlare.Size() / 2f, 0.3f * MathHelper.Lerp(1, 0.1f, Projectile.ai[0] / 7f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBloomFlare, Projectile.Center + unitX * 20 - Main.screenPosition, null, Color.White, 0, texBloomFlare.Size() / 2f, 0.2f * MathHelper.Lerp(1, 0.1f, Projectile.ai[0] / 7f), SpriteEffects.None, 0);
            }

            //目标爆炸
            if (Projectile.ai[0] <= 5)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float a2 = MathHelper.Lerp(1, 0, Projectile.ai[0] / 5f);
                Main.spriteBatch.Draw(texBloomFlare, TargetPos - Main.screenPosition, null, Color.Green, 0, texBloomFlare.Size() / 2f, 0.15f * a2, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBloomFlare, TargetPos - Main.screenPosition, null, Color.White, 0, texBloomFlare.Size() / 2f, 0.1f * a2, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] <= 10)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float scale = MathHelper.Lerp(0, 1, Projectile.ai[0] / 10f);
                float light = 1;
                if (Projectile.ai[0] > 5) light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 5) / 5f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, TargetPos - Main.screenPosition, null, Color.Green * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0);
            }

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
            target.DeepAddCCBuff(ModContent.BuffType<PoisonedDNDBuff>(), GetTimeSpan<RayOfSicknessSpell>() * 60);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int width = 64;
            Rectangle rect = new((int)TargetPos.X - width, (int)TargetPos.Y - width, width * 2, width * 2);
            return targetHitbox.Intersects(rect);
        }
    }
}