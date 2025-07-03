using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Cantrips
{
    public class TollTheDeadProj : BaseMagicProj
    {
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

            if (Projectile.ai[0] > 30) Projectile.Kill();

        }


        public override bool PreDraw(ref Color lightColor)
        {
            float TargetRadius = 120;
            float radius = MathHelper.Lerp(5, TargetRadius, MathHelper.Clamp(Projectile.ai[0] / 30f, 0, 1));
            float light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 15f) / 10f, 0, 1));
            DrawRing(Projectile.Center, radius, 30, Color.Green * light);
            DrawRing(Projectile.Center, radius, 20, Color.White * light);
            float scaleY = MathHelper.Lerp(4, 0, Projectile.ai[0] / 30f);
            float light2 = 1;
            if (Projectile.ai[0] < 5)
            {
                light2 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 5f);
            }
            else if (Projectile.ai[0] > 15)
            {
                light2 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[0] - 15) / 5f, 0f, 1f));
            }
            Texture2D texLightField = TextureLibrary.LightField;
            Texture2D texLight = TextureLibrary.BloomFlare;
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.Green * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.8f, 0.3f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.6f, 0.2f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Green * light2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.09f, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, Projectile.ai[0] / 150f, BlendState.Additive);
        }

        public override void SafeModifyHit(NPC target, ref NPC.HitModifiers modifiers, ref DiceDamage diceUsed, ref float damageModifier, ref Dictionary<DamageElement, float> resistance)
        {
            if (target.life < target.lifeMax / 2f)
                diceUsed = extraDiceDamage;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float TargetRadius = 80;
            float radius = MathHelper.Lerp(20, TargetRadius, MathHelper.Clamp(Projectile.ai[0] / 20f, 0, 1));
            return targetHitbox.Distance(Projectile.Center) <= radius;
        }

    }
}
