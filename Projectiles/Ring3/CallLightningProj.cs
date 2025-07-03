using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class CallLightningProj : BaseMagicProj
    {
        public List<float> Rings = new();
        public List<float> Arcs = new();
        public List<float> ArcTimeLeft = new();

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
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 1 ||
                Projectile.ai[1] == 11 ||
                Projectile.ai[1] == 21)
            {
                Rings.Add(0);
            }
            for (int i = Rings.Count - 1; i >= 0; i--)
            {
                Rings[i] += 0.03f;
                if (Rings[i] >= 1)
                {
                    Rings.RemoveAt(i);
                }
            }
            if (Projectile.ai[1] == 1 ||
                Projectile.ai[1] == 8 ||
                Projectile.ai[1] == 15)
            {
                ArcTimeLeft.Add(18);
                Arcs.Add(Main.rand.NextFloat() * 2 - 1);
            }
            for (int i = ArcTimeLeft.Count - 1; i >= 0; i--)
            {
                ArcTimeLeft[i]--;
                if (ArcTimeLeft[i] <= 0)
                {
                    Arcs.RemoveAt(i);
                    ArcTimeLeft.RemoveAt(i);
                }
            }

            if (Projectile.ai[1] > 60) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;

            ArcSegments segs = new();
            foreach (float a in Arcs)
            {
                float maxWidth = 300;
                float trueWidth = MathHelper.Clamp((Projectile.Center.Y - Main.screenPosition.Y) / (float)Main.screenHeight, 0, 1) * maxWidth;
                Vector2 Top = new(Projectile.Center.X + trueWidth * a, Main.screenPosition.Y - 50);
                segs.GenerateSegs(Top, Projectile.Center, new Vector2(120, 60), 60f);
                segs.DrawSegs(Color.Blue);
            }

            Texture2D LightTex = TextureLibrary.BloomFlare;
            EasyDraw.AnotherDraw(BlendState.Additive);
            if (Projectile.ai[1] <= 15)
            {
                float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 15f);
                float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 15f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale, SpriteEffects.None, 0);
            }
            float alpha = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 5) / 15f, 0, 1));
            Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Blue * alpha, 0, LightTex.Size() / 2f, 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, 0, LightTex.Size() / 2f, 0.3f, SpriteEffects.None, 0);

            foreach (float r in Rings)
            {
                float alpha1 = 1;
                if (r > 0.2)
                {
                    alpha1 = MathHelper.Lerp(1, 0, (r - 0.2f) / 0.8f);
                }
                float radius = (float)Math.Sqrt(r) * 120;
                DrawRing(Projectile.Center, radius, 30, Color.Blue * alpha1);
                DrawRing(Projectile.Center, radius, 16, Color.White * alpha1);
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
            if (Projectile.ai[1] > 35) return false;
            return null;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 16 * GetAOERadius<CallLightningSpell>() &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, Projectile.ai[1] / 300f, BlendState.Additive);
        }


    }
}
