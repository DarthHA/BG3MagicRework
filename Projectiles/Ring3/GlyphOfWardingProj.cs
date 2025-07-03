using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class GlyphOfWardingProj : BaseMagicProj
    {
        public DamageElement damageElement = DamageElement.None;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.netImportant = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }


        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.rotation == 0)
            {
                Projectile.rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            SomeUtils.AddLightCircle(Projectile.Center, 300, Color.White, 0.01f);
            if (Projectile.ai[0] == 0)    //启动
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 15)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)  //维持
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy() || npc.immortal)
                    {
                        if (npc.Hitbox.Distance(Projectile.Center) <= GetAOERadius<GlyphOfWardingSpell>() * 16)
                        {
                            Projectile.ai[0] = 2;
                            Projectile.ai[1] = 0;
                        }
                    }
                }
            }
            else if (Projectile.ai[0] == 2)  //爆炸
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 60)
                {
                    Player owner = Main.player[Projectile.owner];
                    int protmp = owner.NewMagicProj(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlyphOfWardingExplosionProj>(), diceDamage, Projectile.knockBack, CurrentRing);
                    if (protmp >= 0 && protmp < 1000)
                    {
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).CopyMetaMagicFrom(this);
                        (Main.projectile[protmp].ModProjectile as GlyphOfWardingExplosionProj).damageElement = damageElement;
                    }
                }
                if (Projectile.ai[1] > 90)       //60帧反应，10帧爆炸，30帧归位
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[0] == 3)  //取消
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0 && Projectile.ai[1] == 0) return false;
            Texture2D tex = TextureLibrary.GoWRitual;
            float Radius = 500;
            float scale = Radius / tex.Width;

            Color MainColor = damageElement == DamageElement.None ? Color.White : SomeUtils.GetColor(damageElement);

            EasyDraw.AnotherDraw(BlendState.Additive);
            if (Projectile.ai[0] == 0)
            {
                float count = MathHelper.Lerp(60, 0, Projectile.ai[1] / 15f);
                for (int i = 0; i <= count; i++)
                {
                    Vector2 DrawPos = Projectile.Center + new Vector2(0, -4 * i);
                    float alpha = (1f - i / count) / 2f;
                    DrawUtils.DrawLoopMask(tex, TextureLibrary.Perlin, DrawPos, MainColor * alpha, Projectile.rotation, scale, BlendState.Additive, 1, Projectile.localAI[0] / 200f);
                }
                DrawUtils.DrawLoopMask(tex, TextureLibrary.Perlin, Projectile.Center, MainColor, Projectile.rotation, scale, BlendState.Additive, 1, Projectile.localAI[0] / 200f);
            }
            else if (Projectile.ai[0] == 1)
            {
                DrawUtils.DrawLoopMask(tex, TextureLibrary.Perlin, Projectile.Center, MainColor, Projectile.rotation, scale, BlendState.Additive, 1, Projectile.localAI[0] / 200f);
            }
            else if (Projectile.ai[0] == 2)
            {
                if (Projectile.ai[1] < 60)
                {
                    float alpha = MathHelper.Lerp(0, 1, Projectile.ai[1] / 60f);
                    DrawUtils.DrawLoopMask(tex, TextureLibrary.Perlin, Projectile.Center, MainColor * (1 - alpha), Projectile.rotation, scale, BlendState.Additive, 1, Projectile.localAI[0] / 200f);
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, MainColor * alpha, Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);
                }
                else
                {
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    float alpha0 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 70f) / 20f, 0, 1));
                    float count = MathHelper.Lerp(0, 60, (Projectile.ai[1] - 60f) / 10f);
                    if (Projectile.ai[1] > 70)
                    {
                        count = MathHelper.Lerp(60, 0, (Projectile.ai[1] - 70f) / 20f);
                    }
                    for (int i = 0; i <= count; i++)
                    {
                        Vector2 DrawPos = Projectile.Center + new Vector2(0, -4 * i);
                        float alpha = (1f - i / count) / 3f;
                        Main.spriteBatch.Draw(tex, DrawPos - Main.screenPosition, null, MainColor * alpha * alpha0, Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);
                    }
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, MainColor * alpha0, Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);
                }
            }
            else if (Projectile.ai[0] == 3)
            {
                float alpha0 = MathHelper.Lerp(1, 0, Projectile.ai[1] / 30f);
                DrawUtils.DrawLoopMask(tex, TextureLibrary.Perlin, Projectile.Center, MainColor * alpha0, Projectile.rotation, scale, BlendState.Additive, 1, Projectile.localAI[0] / 200f);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);

            return false;
        }

        public void Destroy()
        {
            Projectile.ai[0] = 3;
            Projectile.ai[1] = 0;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}