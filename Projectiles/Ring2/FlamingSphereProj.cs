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
    public class FlamingSphereProj : BaseMagicProj
    {
        public List<TmpParticle> Particles1 = new();
        public List<TmpParticle> Particles2 = new();
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.tileCollide = true;
            //Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0)
            {
                if (owner.IsDead())
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    return;
                }
                //断专注就会消失
                if (owner.GetConcentration(ConUUID) == -1)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    return;
                }
                if (Projectile.Distance(owner.Center) > GetSpellRange<FlamingSphereSpell>() * 16f * 6f)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    return;
                }
                //沾水即化
                if ((Projectile.wet && !Projectile.lavaWet) && !CarefulSpellMM)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                    return;
                }
            }

            if (Projectile.ai[0] == 0)       //常态运动
            {
                //发光
                SomeUtils.AddLight(Projectile.Center, Color.Orange, 5f);
                if (Projectile.Center.Distance(Main.MouseWorld) != 0)
                {
                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                }
                if (Projectile.Center.Distance(Main.MouseWorld) > 10)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * 5;
                    if (Projectile.oldPosition != Projectile.position)
                    {
                        Projectile.ai[1] += 0.02f;
                    }
                    //运动时生成灰烬
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 GeneratePos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 55;
                        Particles1.NewParticle(GeneratePos, Projectile.velocity * 0.01f, Main.rand.NextFloat() * 0.5f + 0.5f);
                    }
                }
                else
                {
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (Projectile.ai[0] == 1)              //消失
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        float r = Main.rand.NextFloat() * MathHelper.TwoPi;
                        Vector2 Pos = Projectile.Center + r.ToRotationVector2() * Main.rand.Next(1, 8);
                        Vector2 Vel = r.ToRotationVector2() * Main.rand.Next(5, 35);
                        float scale = 0.5f + Main.rand.NextFloat() * 0.5f;
                        Particles2.NewParticle(Pos, Vel, scale);
                    }
                    owner.DeleteConcentration(ConUUID);
                }
                if (Projectile.ai[1] > 60) Projectile.Kill();
            }

            Particles1.UpdateParticle(0.95f, 0.97f);
            Particles2.UpdateParticle(0.9f, 0.95f);

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureLibrary.CrispCircle;
            EasyDraw.AnotherDraw(BlendState.Additive);
            Particles1.DrawParticle(TextureLibrary.Extra, Color.OrangeRed, true, new Vector2(1, 1));
            Particles1.DrawParticle(TextureLibrary.Extra, Color.White, true, new Vector2(1, 1) * 0.75f);
            Particles2.DrawParticle(TextureLibrary.Extra, Color.Orange, true, new Vector2(1, 1));
            Particles2.DrawParticle(TextureLibrary.Extra, Color.White, true, new Vector2(1, 1) * 0.75f);
            if (Projectile.ai[0] == 0)
            {
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed, Projectile.rotation, tex.Size() / 2f, 0.7f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                DrawUtils.DrawASphere(Terraria.GameContent.TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, Color.Black * 0.75f, Color.Black, 97, Projectile.rotation, Projectile.ai[1], BlendState.AlphaBlend);
                DrawUtils.DrawASphere(TextureLibrary.Lava, Projectile.Center - Main.screenPosition, Color.Black, Color.Orange, 100, Projectile.rotation, Projectile.ai[1], BlendState.Additive);
                DrawUtils.DrawASphere(TextureLibrary.Lava, Projectile.Center - Main.screenPosition, Color.Black, Color.White, 100, Projectile.rotation, Projectile.ai[1] + 0.5f, BlendState.Additive);
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) <= 100;
        }
    }
}
