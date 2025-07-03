using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Cantrips;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace BG3MagicRework.Projectiles.Cantrips
{
    public class LaserAttackProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public override int MaxHits => 1;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
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
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (TravelDistance > GetSpellRange<LaserAttackSpell>() * 16f)
                {
                    Projectile.localAI[0] = 1;
                }
                if ((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) && !CarefulSpellMM)
                    || TravelDistance > GetSpellRange<LaserAttackSpell>() * 16f * 2f)
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
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(5, 10);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.25f + 0.25f * Main.rand.NextFloat();
                        Particles.NewParticle(Pos, Vel, scale);
                    }
                }
                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
            Particles.UpdateParticle(0.9f, 0.9f);
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 && Projectile.ai[1] >= 3) return false;
            return null;
        }

        public override void SafeModifyHit(NPC target, ref NPC.HitModifiers modifiers, ref DiceDamage diceUsed, ref float damageModifier, ref Dictionary<DamageElement, float> resistance)
        {
            if (Projectile.localAI[0] == 1)
            {
                damageModifier *= 0.5f;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texExtra = TextureLibrary.Extra;
            Texture2D LightTex = TextureLibrary.BloomFlare;

            if (Projectile.ai[0] == 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Green, Projectile.rotation, texExtra.Size() / 2f, new Vector2(1, 0.5f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texExtra.Size() / 2f, new Vector2(1, 0.5f) * 0.8f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            else if (Projectile.ai[0] == 1)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                if (Projectile.ai[1] <= 20)
                {
                    float scale = 3;
                    float light = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 5) / 15f, 0f, 1f));
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Green * light, Projectile.rotation, LightTex.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * light * 1.1f, Projectile.rotation, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
                }
                Texture2D tex = TextureLibrary.Extra;
                Particles.DrawParticle(tex, Color.Green, true, new Vector2(1, 1));
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            return false;
        }
    }
}
