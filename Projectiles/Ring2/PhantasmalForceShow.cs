using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class PhantasmalForceShow : BaseMagicProj
    {
        public List<Vector2> NeedleRelaPos = new();
        public List<int> NeedleTimer = new();
        public List<float> NeedleScale = new();
        public int TargetNPC = -1;
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
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }

            if (TargetNPC == -1 || (!Main.npc[TargetNPC].CanBeChasedBy() && !Main.npc[TargetNPC].immortal))
            {
                Projectile.Kill();
                return;
            }
            NPC Target = Main.npc[TargetNPC];
            Projectile.Center = Target.Center;


            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 RandomPos = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(25, 100);
                    NeedleRelaPos.Add(RandomPos);
                    NeedleScale.Add(0.25f + 0.25f * Main.rand.NextFloat());
                    NeedleTimer.Add(0 - Main.rand.Next(1, 20));
                }
            }

            for (int i = 0; i < NeedleRelaPos.Count; i++)
            {
                if (NeedleTimer[i] < 40)             //0-15Ê±ºóÍË,15-20¾²Ö¹,20-25´Ì³ö
                {
                    NeedleTimer[i]++;
                }
            }

            if (Projectile.ai[0] > 60)
            {
                int protmp = owner.NewMagicProj(Target.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalForceDamageProj>(), diceDamage, 0, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as PhantasmalForceDamageProj).TargetNPC = TargetNPC;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);

                    BaseConcentration con = owner.GenerateConcentration<ConPhantasmalForce>(CurrentRing, GetTimeSpan<PhantasmalForceSpell>() * 60, true);
                    if (con != null)
                    {
                        con.projIndex = protmp;
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                    }

                }

                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            for (int i = 0; i < NeedleRelaPos.Count; i++)
            {
                Vector2 DrawPos = NeedleRelaPos[i];
                Vector2 Scale = NeedleScale[i] * new Vector2(2, 0.5f);
                float alpha0 = 1f;
                if (NeedleTimer[i] < 0)
                {
                    alpha0 = 0f;
                }
                else if (NeedleTimer[i] >= 0 && NeedleTimer[i] < 10)
                {
                    alpha0 = MathHelper.Lerp(0, 1, NeedleTimer[i] / 10f);
                }
                else if (NeedleTimer[i] >= 30)
                {
                    alpha0 = MathHelper.Lerp(1, 0, MathHelper.Clamp((NeedleTimer[i] - 30) / 10f, 0, 1));
                }
                if (NeedleTimer[i] >= 20)
                {
                    DrawPos = MathHelper.Lerp(1f, 0.2f, MathHelper.Clamp((NeedleTimer[i] - 20f) / 3f, 0f, 1f)) * NeedleRelaPos[i];
                }
                Texture2D tex = TextureLibrary.Extra;
                Main.spriteBatch.Draw(tex, Projectile.Center + DrawPos - Main.screenPosition, null, Color.White * alpha0, DrawPos.ToRotation(), tex.Size() / 2f, Scale, SpriteEffects.None, 0);
            }

            Texture2D texExtra = TextureLibrary.Extra;
            Texture2D texLight = TextureLibrary.BloomFlare;
            float alpha1 = 0;
            float alpha2 = 0;
            if (Projectile.ai[0] < 3)
            {
                alpha1 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 3f);
            }
            else if (Projectile.ai[0] < 20)
            {
                alpha1 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 3f) / 17f);
            }

            if (Projectile.ai[0] < 3)
            {
                alpha2 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 3f);
            }
            else if (Projectile.ai[0] < 50)
            {
                alpha2 = 1;
            }
            else if (Projectile.ai[0] < 60)
            {
                alpha2 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 50f) / 10f);
            }
            Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha1, 0, texExtra.Size() / 2f, new Vector2(9, 1f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha1, MathHelper.Pi / 2f, texExtra.Size() / 2f, new Vector2(9, 1f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.White * alpha1, 0, texExtra.Size() / 2f, new Vector2(6, 0.5f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.White * alpha1, MathHelper.Pi / 2f, texExtra.Size() / 2f, new Vector2(6, 0.5f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Purple * alpha2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * alpha2, 0, texLight.Size() / 2f, 0.1f, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }


    }
}