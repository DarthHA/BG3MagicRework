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
    public class CloudOfDaggersShow : BaseMagicProj
    {
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

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 10)
            {
                int protmp = owner.NewMagicProj(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CloudOfDaggersProj>(), diceDamage, 0, CurrentRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                    BaseConcentration con = owner.GenerateConcentration<ConCloudOfDaggers>(CurrentRing, GetTimeSpan<CloudOfDaggersSpell>() * 60, true);
                    if (con != null)
                    {
                        con.projIndex = protmp;
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                    }
                }
            }

            if (Projectile.ai[0] > 60) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 30)
            {
                float scaleY = 1;
                if (Projectile.ai[0] < 20)
                {
                    scaleY = MathHelper.Lerp(3, 1, Projectile.ai[0] / 20f);
                }
                float light2 = 1;
                if (Projectile.ai[0] < 5)
                {
                    light2 = MathHelper.Lerp(0, 1, Projectile.ai[0] / 5f);
                }
                else if (Projectile.ai[0] > 20)
                {
                    light2 = MathHelper.Lerp(1, 0, (Projectile.ai[0] - 20) / 10f);
                }
                Texture2D texLightField = TextureLibrary.LightField;
                Texture2D texLight = TextureLibrary.BloomFlare;
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.LightCyan * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 2.4f, 0.6f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLightField, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 2f, 0.4f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.LightCyan * light2, 0, texLight.Size() / 2f, 0.4f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.3f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }


    }
}
