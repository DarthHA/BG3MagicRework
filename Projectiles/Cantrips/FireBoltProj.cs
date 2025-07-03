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
    public class FireBoltProj : BaseMagicProj
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
            //Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                if (((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) || (Projectile.wet && !Projectile.lavaWet)) && !CarefulSpellMM)
                     || TravelDistance > GetSpellRange<FireBoltSpell>() * 16f)
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


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texBlobGlow = TextureLibrary.BlobGlow;
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;

            if (Projectile.ai[0] == 0)
            {
                float rot = Projectile.velocity.ToRotation();

                Vector2 UnitX = rot.ToRotationVector2();
                Vector2 UnitY = (rot + MathHelper.Pi / 2f).ToRotationVector2();
                List<CustomVertexInfo> bars1 = new()
                {
                    new CustomVertexInfo(Projectile.Center + UnitX * 15 - UnitY * 10 - Main.screenPosition, Color.White, new Vector3(0, 0.1f, 1)),
                    new CustomVertexInfo(Projectile.Center + UnitX * 15 + UnitY * 10 - Main.screenPosition, Color.White, new Vector3(0, 0.9f, 1)),
                    new CustomVertexInfo(Projectile.Center - UnitX * 60 - UnitY * 10 - Main.screenPosition, Color.White, new Vector3(1, 0.1f, 1)),
                    new CustomVertexInfo(Projectile.Center - UnitX * 60 + UnitY * 10 - Main.screenPosition, Color.White, new Vector3(1, 0.9f, 1)),
                };
                DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.DarkOrange, 0.4f, 1f, -Projectile.ai[1] / 10f, BlendState.Additive);

                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texBlobGlow, Projectile.Center - Main.screenPosition, null, Color.DarkOrange, rot, new Vector2(texBlobGlow.Width / 3 * 2, texBlobGlow.Height / 2), 0.15f, SpriteEffects.FlipHorizontally, 0);
                Main.spriteBatch.Draw(texBlobGlow, Projectile.Center - Main.screenPosition, null, Color.White, rot, new Vector2(texBlobGlow.Width / 3 * 2, texBlobGlow.Height / 2), 0.08f, SpriteEffects.FlipHorizontally, 0);
            }
            else if (Projectile.ai[0] == 1)
            {
                if (Projectile.ai[1] <= 20)
                {
                    List<CustomVertexInfo> bars1 = new()
                    {
                        new CustomVertexInfo(Projectile.Center + new Vector2(20, -60) - Main.screenPosition, Color.White, new Vector3(0, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + new Vector2(-20, -60) - Main.screenPosition, Color.White, new Vector3(0, 1, 1)),
                        new CustomVertexInfo(Projectile.Center + new Vector2(20, 60)  - Main.screenPosition, Color.White, new Vector3(1, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + new Vector2(-20, 60) - Main.screenPosition, Color.White, new Vector3(1, 1, 1)),
                    };
                    List<CustomVertexInfo> bars2 = new()
                    {
                        new CustomVertexInfo(Projectile.Center + new Vector2(15, -50) - Main.screenPosition, Color.White, new Vector3(0, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + new Vector2(-15, -50) - Main.screenPosition, Color.White, new Vector3(0, 1, 1)),
                        new CustomVertexInfo(Projectile.Center + new Vector2(15, 50)  - Main.screenPosition, Color.White, new Vector3(1, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + new Vector2(-15, 50) - Main.screenPosition, Color.White, new Vector3(1, 1, 1)),
                    };

                    float scale = 3;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 20f);
                    DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.DarkOrange * light, 0.3f, 1, -Projectile.ai[1] / 10f, BlendState.Additive);
                    DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White * light, 0.3f, 1, -Projectile.ai[1] / 10f, BlendState.Additive);

                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * light, Projectile.rotation, LightTex.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
                }

                EasyDraw.AnotherDraw(BlendState.Additive);
                Texture2D tex = TextureLibrary.Extra;
                Particles.DrawParticle(tex, Color.Orange, true, new Vector2(1, 1));
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

    }

}