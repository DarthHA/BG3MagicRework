using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class MelfsMinuteMeteorsShot : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public List<Vector2> Trails = new();
        public int shotFrame = 0;
        public bool canTileCollide = false;
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
                Trails.Add(Projectile.Center);
                if (Trails.Count > 6)
                {
                    Trails.RemoveAt(0);
                }

                Projectile.velocity *= 1.1f;
                if (Projectile.velocity.Length() > 20) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20;

                Projectile.rotation += 1 / 200f;

                if (!Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height))
                {
                    canTileCollide = true;
                }
                if ((Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) && !CarefulSpellMM && canTileCollide)
                    || TravelDistance > GetSpellRange<MelfsMinuteMeteorsSpell>() * 16f)
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
                    Projectile.width = GetAOERadius<MelfsMinuteMeteorsSpell>() * 16 * 2;
                    Projectile.height = GetAOERadius<MelfsMinuteMeteorsSpell>() * 16 * 2;
                    Projectile.Center = Center;
                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 25);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 20);
                        float scale = 0.4f + 0.4f * Main.rand.NextFloat();
                        Particles.NewParticle(Pos, Vel, scale);
                    }
                }
                Particles.UpdateParticle(0.9f, 0.95f);
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
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1 && Projectile.ai[1] >= 3) return false;
            return null;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D texLight = TextureLibrary.BloomFlare;
            Texture2D texExtra = TextureLibrary.Extra;
            Texture2D tex = TextureLibrary.Meteor3;
            if (Projectile.ai[0] == 0)
            {
                int width = 13;
                if (Trails.Count > 1)
                {
                    List<CustomVertexInfo> bars0 = new();
                    Vector2 UnitX = Vector2.Normalize(Projectile.velocity);
                    Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                    bars0.Add(new CustomVertexInfo(Projectile.Center + UnitX * width - UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 0, 1f)));
                    bars0.Add(new CustomVertexInfo(Projectile.Center + UnitX * width + UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 1, 1f)));
                    for (int i = Trails.Count - 1; i >= 0; i--)
                    {
                        UnitX = -Vector2.Normalize(Projectile.velocity);
                        if (i != Trails.Count - 1 && Trails[i] != Trails[i + 1])
                        {
                            UnitX = Vector2.Normalize(Trails[i] - Trails[i + 1]);
                        }
                        UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                        bars0.Add(new CustomVertexInfo(Trails[i] + UnitY * width - Main.screenPosition, Color.White, new Vector3(1f - (float)i / Trails.Count * 0.8f, 0, 1f)));
                        bars0.Add(new CustomVertexInfo(Trails[i] - UnitY * width - Main.screenPosition, Color.White, new Vector3(1f - (float)i / Trails.Count * 0.8f, 1, 1f)));
                    }
                    DrawUtils.DrawTrail(TextureLibrary.BlobGlow2, bars0, Color.Orange, BlendState.Additive);
                }

                Rectangle rect = new(0, tex.Height / 3 * shotFrame, tex.Width, tex.Height / 3);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition
                    , rect, Color.White,
                    Projectile.rotation, rect.Size() / 2f, 0.5f, SpriteEffects.None, 0);
            }

            else if (Projectile.ai[0] == 1)
            {
                if (Projectile.ai[1] <= 10)
                {
                    float r = Projectile.rotation;
                    float scale = 4;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Orange * light, Projectile.rotation, texLight.Size() / 2f, 0.1f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, texLight.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
                }
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                if (Projectile.ai[1] <= 10)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 10f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.Orange * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.8f, SpriteEffects.None, 0);
                }

                Particles.DrawParticle(texExtra, Color.Orange, true, new Vector2(1, 1));
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
