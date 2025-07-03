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
    public class IceKnifeProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public List<SmokeParticle> smokeParticles = new();
        public override int MaxHits => -1;
        public List<float> IceRot = new();
        public List<float> IceScale = new();
        public List<int> IceFrame = new();
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
            Projectile.localNPCHitCooldown = 0;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                if ((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) && !CarefulSpellMM) || TravelDistance > GetSpellRange<IceKnifeSpell>() * 16f)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.localNPCHitCooldown = 9999;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    Vector2 Center = Projectile.Center;
                    Projectile.width = 16 * 2 * GetAOERadius<IceKnifeSpell>();
                    Projectile.height = 16 * 2 * GetAOERadius<IceKnifeSpell>();
                    Projectile.Center = Center;
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 20);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.5f + 0.5f * Main.rand.NextFloat();
                        Particles.NewParticle(Pos, Vel, scale);
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        IceRot.Add(Main.rand.NextFloat() * MathHelper.TwoPi);
                        IceScale.Add(0.75f + 1.5f * Main.rand.NextFloat());
                        IceFrame.Add(Main.rand.Next(3));
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 15;
                        smokeParticles.NewParticle(Projectile.Center, ShootVel, Main.rand.NextFloat() * 0.25f + 0.25f, Color.LightBlue);
                    }
                }
                Particles.UpdateParticle(0.9f, 0.93f);
                smokeParticles.UpdateParticle();
                if (Projectile.ai[1] > 60) Projectile.Kill();
            }
        }


        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
                Projectile.velocity = Vector2.Zero;
                Projectile.localNPCHitCooldown = 9999;
            }
            else      //概率减速
            {
                this.DeepAddCCBuffByDC(target, ModContent.BuffType<IceSlowBuff>(), GetTimeSpan<IceKnifeSpell>() * 60);
            }
        }

        public override void SafeModifyHit(NPC target, ref NPC.HitModifiers modifiers, ref DiceDamage diceUsed, ref float damageModifier, ref Dictionary<DamageElement, float> resistance)
        {
            if (Projectile.ai[0] == 1)
            {
                diceUsed = extraDiceDamage;
            }
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 && Projectile.ai[1] >= 3) return false;
            return null;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texIceShard = TextureLibrary.IceShard;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;

            if (Projectile.ai[0] == 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                int frame = (int)Projectile.ai[1] % 15 / 5;
                Rectangle rect = new(0, texIceShard.Height / 3 * frame, texIceShard.Width, texIceShard.Height / 3);
                Main.spriteBatch.Draw(texIceShard, Projectile.Center - Main.screenPosition, rect, Color.White, Projectile.velocity.ToRotation() - MathHelper.Pi / 2f, rect.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
            else if (Projectile.ai[0] == 1)
            {
                EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
                smokeParticles.DrawParticle(false);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                float alpha = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 30) / 10f, 0, 1));
                for (int i = 0; i < IceRot.Count; i++)
                {
                    Rectangle rect = new(0, texIceShard.Height / 3 * IceFrame[i], texIceShard.Width, texIceShard.Height / 3);
                    Vector2 origin = new(rect.Size().X / 2f, rect.Size().Y / 5);
                    Vector2 DrawPos = Projectile.Center + IceRot[i].ToRotationVector2();// * rect.Size().Y / 5f * IceScale[i];
                    Main.spriteBatch.Draw(texIceShard, DrawPos - Main.screenPosition, rect, Color.Black * 0.45f * alpha, IceRot[i] - MathHelper.Pi / 2f, origin, IceScale[i], SpriteEffects.None, 0);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                for (int i = 0; i < IceRot.Count; i++)
                {
                    Rectangle rect = new(0, texIceShard.Height / 3 * IceFrame[i], texIceShard.Width, texIceShard.Height / 3);
                    Vector2 origin = new(rect.Size().X / 2f, rect.Size().Y / 5);
                    Vector2 DrawPos = Projectile.Center + IceRot[i].ToRotationVector2();// * rect.Size().Y / 5f * IceScale[i];
                    Main.spriteBatch.Draw(texIceShard, DrawPos - Main.screenPosition, rect, Color.White * 0.75f * alpha, IceRot[i] - MathHelper.Pi / 2f, origin, IceScale[i], SpriteEffects.None, 0);
                }
                if (Projectile.ai[1] <= 15)
                {
                    float r = Projectile.rotation;
                    float scale = 4;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 15f);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.Cyan * light, Projectile.rotation, LightTex.Size() / 2f, 0.1f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, LightTex.Size() / 2f, 0.05f * scale, SpriteEffects.None, 0);
                }

                if (Projectile.ai[1] <= 10)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 10f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.75f, SpriteEffects.None, 0);
                }
                Texture2D tex = TextureLibrary.Extra;
                Particles.DrawParticle(tex, Color.Cyan, true, new Vector2(1, 1));
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
