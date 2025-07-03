using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class ScorchingRayProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public Vector2 TargetPos = Vector2.Zero;
        public override int MaxHits => 1;
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
                for (int i = 0; i < 10; i++)
                {
                    float r = (TargetPos - Projectile.Center).ToRotation() + (Main.rand.NextFloat() * MathHelper.Pi - MathHelper.Pi / 2f);
                    Vector2 Pos = TargetPos + r.ToRotationVector2() * Main.rand.Next(1, 8);
                    Vector2 Vel = r.ToRotationVector2() * Main.rand.Next(2, 10);
                    float scale = 0.2f + Main.rand.NextFloat() * 0.2f;
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
            float a1 = 0;
            float a2 = 0;
            if (Projectile.ai[0] <= 30)
            {
                a1 = MathHelper.Lerp(1f, 0, Projectile.ai[0] / 30f);
            }
            if (Projectile.ai[0] < 10)
            {
                a2 = MathHelper.Lerp(1f, 0f, Projectile.ai[0] / 10f);
            }
            List<CustomVertexInfo> bars1 = new()
                {
                new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * 7 - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * 7 - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 + unitY * 10 - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 - unitY * 10 - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            List<CustomVertexInfo> bars2 = new()
                {
                new CustomVertexInfo(Projectile.Center - unitX * 20 + unitY * 10 - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(Projectile.Center - unitX * 20 - unitY * 10 - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 + unitY * 15 - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(TargetPos + unitX * 20 - unitY * 15 - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.OrangeRed * a1, 0.1f, 1f, -Projectile.ai[0] / 120f, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.White * a1, 0.1f, 1f, -Projectile.ai[0] / 120f, BlendState.Additive);
            DrawUtils.DrawTrail(texBlobGlow2, bars2, Color.OrangeRed * a2, BlendState.Additive);


            EasyDraw.AnotherDraw(BlendState.Additive);
            Particles.DrawParticle(tex, Color.OrangeRed, true, new Vector2(2, 1));


            //目标爆炸
            if (Projectile.ai[0] <= 10)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float a3 = MathHelper.Lerp(1, 0, Projectile.ai[0] / 10f);
                Main.spriteBatch.Draw(texBloomFlare, TargetPos - Main.screenPosition, null, Color.OrangeRed, 0, texBloomFlare.Size() / 2f, 0.15f * a3, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBloomFlare, TargetPos - Main.screenPosition, null, Color.White, 0, texBloomFlare.Size() / 2f, 0.1f * a3, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] <= 10)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                float scale = MathHelper.Lerp(0, 1, Projectile.ai[0] / 10f);
                float light = 1;
                if (Projectile.ai[0] > 5) light = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 5) / 5f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, TargetPos - Main.screenPosition, null, Color.OrangeRed * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.15f, SpriteEffects.None, 0);
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int width = 32;
            Rectangle rect = new((int)TargetPos.X - width, (int)TargetPos.Y - width, width * 2, width * 2);
            return targetHitbox.Intersects(rect);
        }

    }
}
