using BG3MagicRework.BaseType;
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
    public class MagicMissileProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public List<Vector2> Trails = new();
        public Vector2? BeginPos = null;
        public bool HasTarget = true;
        public override int MaxHits => 1;
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
            if (Projectile.ai[0] == 0)            //魔法飞弹在有索敌时可以穿墙，无索敌时不会
            {
                int Target = -1;
                if (HasTarget) Target = SomeUtils.FindEnemyByOwner(Projectile.Center, Main.player[Projectile.owner].Center, GetSpellRange<MagicMissileSpell>() * 16f * 1.5f, CarefulSpellMM);
                if (Target != -1)
                {
                    Vector2 ShootVel = Vector2.Normalize(Main.npc[Target].Center - Projectile.Center) * 20f;
                    Projectile.velocity = Projectile.velocity * 0.9f + ShootVel * 0.2f;
                    if (Projectile.velocity.Length() > 20) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20;
                }
                else
                {
                    HasTarget = false;
                }
                Trails.Add(Projectile.Center);
                if (Trails.Count > 6)
                {
                    Trails.RemoveAt(0);
                }
                if (BeginPos == null)
                {
                    BeginPos = Projectile.Center;
                }
                Projectile.ai[1]++;
                if ((Target == -1 && Collision.SolidTiles(Projectile.position, Projectile.width, Projectile.height) && !CarefulSpellMM)
                    || TravelDistance > GetSpellRange<MagicMissileSpell>() * 16f)
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
                    Projectile.width = 64;
                    Projectile.height = 64;
                    Projectile.Center = Center;
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(5, 15);
                        Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 10);
                        float scale = 0.35f + 0.35f * Main.rand.NextFloat();
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
            Texture2D texLight = TextureLibrary.BloomFlare;
            Texture2D texExtra = TextureLibrary.Extra;
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.ai[1] < 10 && BeginPos.HasValue)
                {
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texLight, BeginPos.Value - Main.screenPosition, null, Color.Red * light, Projectile.rotation, texLight.Size() / 2f, 0.06f * 2.5f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, BeginPos.Value - Main.screenPosition, null, Color.White * light, Projectile.rotation, texLight.Size() / 2f, 0.03f * 2.5f, SpriteEffects.None, 0);
                }
                //轨迹
                if (Trails.Count > 1)
                {
                    List<CustomVertexInfo> bars1 = new();
                    Vector2 UnitX = Vector2.Normalize(Projectile.velocity);
                    bars1.Add(new CustomVertexInfo(Projectile.Center + UnitX.RotatedBy(MathHelper.Pi / 2f) * 3 - Main.screenPosition, Color.White, new Vector3(0f, 0, 1f)));
                    bars1.Add(new CustomVertexInfo(Projectile.Center - UnitX.RotatedBy(MathHelper.Pi / 2f) * 3 - Main.screenPosition, Color.White, new Vector3(0f, 1, 1f)));
                    for (int i = Trails.Count - 1; i >= 0; i--)
                    {
                        UnitX = -Vector2.Normalize(Projectile.velocity);
                        if (i != Trails.Count - 1 && Trails[i] != Trails[i + 1])
                        {
                            UnitX = Vector2.Normalize(Trails[i] - Trails[i + 1]);
                        }
                        Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                        bars1.Add(new CustomVertexInfo(Trails[i] + UnitY * 3 - Main.screenPosition, Color.White, new Vector3(1f - (float)i / Trails.Count, 0, 1f)));
                        bars1.Add(new CustomVertexInfo(Trails[i] - UnitY * 3 - Main.screenPosition, Color.White, new Vector3(1f - (float)i / Trails.Count, 1, 1f)));
                    }
                    DrawUtils.DrawTrail(TextureLibrary.BlobGlow2, bars1, Color.Red, BlendState.Additive);
                    EasyDraw.AnotherDraw(BlendState.Additive);
                }
                Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Red, 0, texExtra.Size() / 2f, new Vector2(1.75f, 0.25f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Red, Projectile.rotation, texLight.Size() / 2f, 0.06f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texLight.Size() / 2f, 0.04f, SpriteEffects.None, 0);
            }
            else if (Projectile.ai[0] == 1)
            {
                if (Projectile.ai[1] <= 10)
                {
                    float r = Projectile.rotation;
                    float scale = 4;
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Red * light, Projectile.rotation, texLight.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light, Projectile.rotation, texLight.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
                }
                Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
                if (Projectile.ai[1] <= 10)
                {
                    float scale = MathHelper.Lerp(0, 1, Projectile.ai[1] / 10f);
                    float light = MathHelper.Lerp(1, 0, Projectile.ai[1] / 10f);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.Red * light, 0, texHollowCircleSoftEdge.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0);
                }

                Particles.DrawParticle(texExtra, Color.Red, true, new Vector2(1, 1));
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
