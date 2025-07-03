using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class FlamingSphereShow : BaseMagicProj
    {
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.tileCollide = true;
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

            //发光
            SomeUtils.AddLight(Projectile.Center, Color.Orange, 15f);

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30) Projectile.Kill();
            if (Projectile.ai[0] == 25)
            {
                int protmp = owner.NewMagicProj(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlamingSphereProj>(), diceDamage, 0, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                    BaseConcentration con = owner.GenerateConcentration<ConFlamingSphere>(CurrentRing, GetTimeSpan<FlamingSphereSpell>() * 60, true);
                    if (con != null)
                    {
                        con.projIndex = protmp;
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                    }
                }

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex1 = TextureLibrary.HollowCircleSoftEdge;
            Texture2D tex2 = TextureLibrary.BloomFlare;
            EasyDraw.AnotherDraw(BlendState.Additive);
            if (Projectile.ai[0] < 15)
            {
                float scale1 = MathHelper.Lerp(1, 0, Projectile.ai[0] / 15f);
                float light1 = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.ai[0] / 5f, 0, 1));
                Main.spriteBatch.Draw(tex1, Projectile.Center - Main.screenPosition, null, Color.Orange * light1, 0, tex1.Size() / 2f, scale1 * 0.75f, SpriteEffects.None, 0);
            }

            float scale2 = MathHelper.Lerp(0f, 1, MathHelper.Clamp(Projectile.ai[0] / 10f, 0, 1));
            float light2 = MathHelper.Lerp(0.5f, 1, Projectile.ai[0] / 20);
            if (Projectile.ai[0] >= 20)
            {
                light2 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 20) / 10);
                scale2 = 1;
            }
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Red * light2, 0, tex2.Size() / 2f, 0.5f * scale2, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Yellow * light2, 0, tex2.Size() / 2f, 0.4f * scale2, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, tex2.Size() / 2f, 0.3f * scale2, SpriteEffects.None, 0);

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

    }
}
