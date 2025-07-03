using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class MelfsAcidArrowDamageProj : BaseMagicProj
    {
        public List<TmpParticle> tmpParticles = new();
        public int TargetNPC = -1;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9999;
        }
        public override void AI()
        {
            if (TargetNPC == -1)
            {
                Projectile.Kill();
                return;
            }
            NPC target = Main.npc[TargetNPC];
            if (!target.CanBeChasedBy() && !target.immortal)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = target.Center;

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > GetTimeSpan<MelfsAcidArrowSpell>() * 60)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        float rot = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 Pos = Projectile.Center + rot.ToRotationVector2() * Main.rand.Next(5, 15);
                        Vector2 Vel = rot.ToRotationVector2() * Main.rand.Next(5, 20);
                        float scale = 0.25f + Main.rand.NextFloat() * 0.25f;
                        tmpParticles.NewParticle(Pos, Vel, scale);
                    }
                }

                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
            tmpParticles.UpdateParticle(0.93f, 0.93f);
        }


        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 0) return false;
            if (target.whoAmI != TargetNPC) return false;
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texExtra = TextureLibrary.Extra;
            EasyDraw.AnotherDraw(BlendState.Additive);
            tmpParticles.DrawParticle(texExtra, Color.YellowGreen, true, new Vector2(2, 1));
            tmpParticles.DrawParticle(texExtra, Color.White, true, new Vector2(1.5f, 0.5f));
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }



    }
}
