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
    public class GuidingBoltProj : BaseMagicProj
    {
        public override int MaxHits => 1;
        public List<TmpParticle> Particles = new();
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                if ((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) && !CarefulSpellMM) || TravelDistance > GetSpellRange<GuidingBoltSpell>() * 16f)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    Vector2 Center = Projectile.Center;
                    Projectile.width = 128;
                    Projectile.height = 128;
                    Projectile.Center = Center;
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 20);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.5f + 0.5f * Main.rand.NextFloat();
                        Particles.NewParticle(Pos, Vel, scale);
                    }
                }
                Particles.UpdateParticle(0.9f, 0.93f);
                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;
            }
            target.DeepAddCCBuff(ModContent.BuffType<GuidingBoltBuff>(), GetTimeSpan<GuidingBoltSpell>() * 60);
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 && Projectile.ai[1] >= 3) return false;
            return null;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D texBubble = TextureLibrary.LightBubble;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            if (Projectile.ai[0] == 0)
            {
                float r = Projectile.rotation;
                Main.spriteBatch.Draw(texBubble, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, r, texBubble.Size() / 2f, 0.06f * 1.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBubble, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, r + MathHelper.Pi / 2f, texBubble.Size() / 2f, 0.04f * 1.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.LightYellow, Projectile.rotation, LightTex.Size() / 2f, 0.08f * 1.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, LightTex.Size() / 2f, 0.04f * 1.2f, SpriteEffects.None, 0);
            }
            else if (Projectile.ai[0] == 1)
            {
                if (Projectile.ai[1] <= 10)
                {
                    float r = Projectile.rotation;
                    float scale = 4;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.LightYellow * light, Projectile.rotation, LightTex.Size() / 2f, 0.08f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
                }
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                if (Projectile.ai[1] <= 10)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 10f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0);
                }

                Texture2D tex = TextureLibrary.Extra;
                Particles.DrawParticle(tex, Color.Yellow, true, new Vector2(1, 1));
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
