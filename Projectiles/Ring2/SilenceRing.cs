using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class SilenceRing : BaseMagicProj
    {
        public List<SmokeParticle> smokeParticles = new();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 9999;
        }

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
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[0] == 0)    //启动
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 25)
                {
                    float multi = (GetAOERadius<SilenceSpell>() + 6 * (CurrentRing - 2)) / GetAOERadius<SilenceSpell>();
                    for (int i = 0; i < 30 * multi; i++)
                    {
                        Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 25 * multi;
                        smokeParticles.NewParticle(Projectile.Center, ShootVel, (Main.rand.NextFloat() * 0.5f + 0.5f) * multi, Color.Black);
                    }
                }
                if (Projectile.ai[1] > 50)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)  //维持
            {
                if (owner.IsDead())
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                    return;
                }
                if (owner.GetConcentration(ConUUID) == -1)                //断专注
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                    return;
                }
                if (Projectile.ai[1] < 10)
                {
                    Projectile.ai[1]++;
                }
                float Radius = (GetAOERadius<SilenceSpell>() + 6 * (CurrentRing - 2)) * 16;
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.Distance(Projectile.Center) <= Radius)
                    {
                        player.AddBuff(BuffID.Silenced, 2);
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if ((npc.CanBeChasedBy(null, true) || npc.immortal) && npc.Hitbox.Distance(Projectile.Center) < Radius)
                    {
                        npc.DeepAddCCBuff(ModContent.BuffType<SilencedDNDBuff>(), 2);
                    }
                }
            }
            else if (Projectile.ai[0] == 2)  //取消
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 20) Projectile.Kill();
            }
            smokeParticles.UpdateParticle();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float Radius = (GetAOERadius<SilenceSpell>() + 6 * (CurrentRing - 2)) * 16;
            float multi = (GetAOERadius<SilenceSpell>() + 6 * (CurrentRing - 2)) / GetAOERadius<SilenceSpell>();
            Texture2D texExtra = TextureLibrary.Extra;
            Texture2D texLight = TextureLibrary.BloomFlare;
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
            Texture2D texCircle = TextureLibrary.Circle;
            Texture2D texCircleBlack = TextureLibrary.CircleBlack;
            float FinalScale = Radius * 2f / texHollowCircleSoftEdge.Width;

            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            smokeParticles.DrawParticle(true);

            if (Projectile.ai[0] == 0)
            {
                if (Projectile.ai[1] < 30)
                {
                    float scaleY = 1;
                    if (Projectile.ai[1] < 20)
                    {
                        scaleY = MathHelper.Lerp(3, 1, Projectile.ai[1] / 20f);
                    }
                    float light2 = 1;
                    if (Projectile.ai[1] < 5)
                    {
                        light2 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 5f);
                    }
                    else if (Projectile.ai[1] > 20)
                    {
                        light2 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 20) / 10f);
                    }

                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Red * light2, MathHelper.Pi / 2f, texExtra.Size() / 2f, 2f * new Vector2(scaleY, 1) * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texExtra.Size() / 2f, 1.5f * new Vector2(scaleY, 1) * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Red * light2, 0, texLight.Size() / 2f, 0.09f * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.075f * multi, SpriteEffects.None, 0);
                }
                if (Projectile.ai[1] > 25 && Projectile.ai[1] < 50)
                {
                    float light1;
                    if (Projectile.ai[1] < 30)
                    {
                        light1 = MathHelper.Lerp(0, 1, (Projectile.ai[1] - 25) / 5f);
                    }
                    else
                    {
                        light1 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 30) / 20f);
                    }

                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Red * light1, MathHelper.Pi / 2f, texExtra.Size() / 2f, 2f * new Vector2(2.5f, 0.5f) * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.White * light1, MathHelper.Pi / 2f, texExtra.Size() / 2f, 1.5f * new Vector2(2.5f, 0.5f) * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.Red * light1, 0, texExtra.Size() / 2f, 2f * new Vector2(2.5f, 0.5f) * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texExtra, Projectile.Center - Main.screenPosition, null, Color.White * light1, 0, texExtra.Size() / 2f, 1.5f * new Vector2(2.5f, 0.5f) * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.Red * light1, 0, texLight.Size() / 2f, 0.25f * multi, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light1, 0, texLight.Size() / 2f, 0.2f * multi, SpriteEffects.None, 0);
                }
                if (Projectile.ai[1] > 40)
                {
                    float light1 = MathHelper.Lerp(0, 0.5f, (Projectile.ai[1] - 40) / 10f);
                    EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                    Main.spriteBatch.Draw(texCircleBlack, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.25f, 0, texCircleBlack.Size() / 2, FinalScale * 0.95f, SpriteEffects.None, 0);
                    EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
                    Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light1, 0, texHollowCircleSoftEdge.Size() / 2, FinalScale, SpriteEffects.None, 0);
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(texCircle, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.5f, 0, texCircle.Size() / 2, FinalScale * 0.95f, SpriteEffects.None, 0);
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                float light1 = MathHelper.Lerp(0.5f, 1, Projectile.ai[1] / 10f);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                Main.spriteBatch.Draw(texCircleBlack, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.25f, 0, texCircleBlack.Size() / 2, FinalScale * 0.95f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light1, 0, texHollowCircleSoftEdge.Size() / 2, FinalScale, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texCircle, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.5f, 0, texCircle.Size() / 2, FinalScale * 0.95f, SpriteEffects.None, 0);
            }
            else if (Projectile.ai[0] == 2)
            {
                float light1 = MathHelper.Lerp(1, 0, Projectile.ai[1] / 20f);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                Main.spriteBatch.Draw(texCircleBlack, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.25f, 0, texCircleBlack.Size() / 2, FinalScale * 0.95f, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.White * light1, 0, texHollowCircleSoftEdge.Size() / 2, FinalScale, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(texCircle, Projectile.Center - Main.screenPosition, null, Color.White * light1 * 0.5f, 0, texCircle.Size() / 2, FinalScale * 0.95f, SpriteEffects.None, 0);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
