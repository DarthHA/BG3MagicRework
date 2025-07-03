using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Concentrations;
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
    public class FaerieFireProj : BaseMagicProj
    {
        public List<TmpParticle> Particles = new();
        public long UsedUUID = 0;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }

        public override void AI()
        {
            if (UsedUUID == 0) UsedUUID = SomeUtils.GenerateUUID();
            if (Projectile.rotation == 0) Projectile.rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
            Projectile.ai[0]++;

            if (Projectile.ai[0] == 30)
            {
                for (int i = 0; i < 40; i++)
                {
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(1, 5);
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * GetAOERadius<FaerieFireSpell>() * 16;
                    float scale = 0.3f + 0.3f * Main.rand.NextFloat();
                    Particles.NewParticle(Pos, Vel, scale);
                }
            }

            Particles.UpdateParticle(0.97f, 0.97f);
            if (Projectile.ai[0] > 120) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D tex = TextureLibrary.Extra;
            Texture2D texLight = TextureLibrary.BloomFlare;
            Particles.DrawParticle(tex, Color.Purple * 0.75f, false, new Vector2(1, 1));
            Particles.DrawParticle(tex, Color.White * 0.75f, false, new Vector2(1, 1));

            if (Projectile.ai[0] < 30)
            {
                float light1 = 1;
                if (Projectile.ai[0] < 20)
                {
                    light1 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 20f);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Purple * light1 * 0.75f, Projectile.rotation, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.75f, Projectile.rotation, texLight.Size() / 2f, 0.1f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            if (Projectile.ai[0] > 20 && Projectile.ai[0] < 80)
            {
                float light2 = 1;
                float scale2 = 1;
                if (Projectile.ai[0] < 35)
                {
                    scale2 = MathHelper.Lerp(0, 1, (Projectile.ai[0] - 20) / 15f);
                }
                else
                {
                    light2 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 35) / 45f);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Purple * light2, Projectile.rotation, texLight.Size() / 2f, 0.5f * scale2, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, Projectile.rotation, texLight.Size() / 2f, 0.4f * scale2, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }

            if (Projectile.ai[0] > 20 && Projectile.ai[0] < 80)
            {
                float light3 = 1;
                if (Projectile.ai[0] < 35)
                {
                    light3 = MathHelper.Lerp(0, 1, (Projectile.ai[0] - 20) / 15f);
                }
                else if (Projectile.ai[0] > 40)
                {
                    light3 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 40) / 40f);
                }
                DrawRing(Projectile.Center, GetAOERadius<FaerieFireSpell>() * 16, 40, Color.Purple * light3 * 0.75f);
                DrawRing(Projectile.Center, GetAOERadius<FaerieFireSpell>() * 16, 30, Color.White * light3 * 0.75f);
            }
            return false;
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 30 || Projectile.ai[0] > 40) return false;
            return null;
        }

        public override void SafeOnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff(ModContent.BuffType<FaerieFireBuff>())) return;
            if (!this.DeepAddCCBuffByDC(target, ModContent.BuffType<FaerieFireBuff>(), 2))
            {
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (owner.GetConcentration(UsedUUID) == -1)
            {
                BaseConcentration con = owner.GenerateConcentration<ConFaerieFire>(CurrentRing, GetTimeSpan<FaerieFireSpell>() * 60, true);
                if (con != null)
                {
                    con.projIndex = Projectile.whoAmI;
                    con.UUID = UsedUUID;
                }
            }
            int protmp = owner.NewMagicProj(target.Center, Vector2.Zero, ModContent.ProjectileType<FaeireFireLight>(), CurrentRing);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = UsedUUID;
                (Main.projectile[protmp].ModProjectile as FaeireFireLight).TargetNPC = target.whoAmI;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < GetAOERadius<FaerieFireSpell>() * 16
                && (CarefulSpellMM || Collision.CanHit(targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height, Projectile.Center, 1, 1));
        }

        public override bool ShouldUpdatePosition()
        {
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.2f, Projectile.ai[0] / 600f, BlendState.Additive);
        }

    }
}
