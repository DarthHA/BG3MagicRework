using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class SpiritGuardiansProj : BaseMagicProj
    {
        public Vector2 RelaPos = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.hide = true;
            Projectile.netImportant = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;
            if (Projectile.ai[0] == 0)    //启动
            {
                Projectile.ai[1]++;
                Projectile.localAI[0]++;
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
                    Projectile.Kill();
                    return;
                }
                if (owner.GetConcentration(ConUUID) == -1)                //断专注
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                    return;
                }
                Projectile.localAI[0]++;
                int radius = GetAOERadius<SpiritGuardiansSpell>() * 16 + (CurrentRing - 3) * 50;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy(null, true) && npc.Hitbox.Distance(Projectile.Center) < radius &&
                     (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, npc.TopLeft, npc.width, npc.height)))
                    {
                        npc.DeepAddCCBuff(ModContent.BuffType<SpiritGuardiansSlowBuff>(), 2);
                    }
                }
            }
            else if (Projectile.ai[0] == 2)  //取消
            {
                Projectile.ai[1]++;
                Projectile.localAI[0]++;
                if (Projectile.ai[1] > 30) Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            int radius = GetAOERadius<SpiritGuardiansSpell>() * 16 + (CurrentRing - 3) * 50;
            int SwordCount = 5 + (CurrentRing - 3);
            EasyDraw.AnotherDraw(BlendState.Additive);
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
                    Texture2D texLightField = TextureLibrary.LightField;
                    Texture2D texLight = TextureLibrary.BloomFlare;
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Main.spriteBatch.Draw(texLightField, owner.Center + RelaPos - Main.screenPosition, null, Color.Gold * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.75f, 0.2f), SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLightField, owner.Center + RelaPos - Main.screenPosition, null, Color.White * light2, MathHelper.Pi / 2f, texLightField.Size() / 2f, new Vector2(scaleY * 0.6f, 0.15f), SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, owner.Center + RelaPos - Main.screenPosition, null, Color.Gold * light2, 0, texLight.Size() / 2f, 0.15f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texLight, owner.Center + RelaPos - Main.screenPosition, null, Color.White * light2, 0, texLight.Size() / 2f, 0.12f, SpriteEffects.None, 0);
                    EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                }
                if (Projectile.ai[1] > 20)
                {
                    float scale1 = MathHelper.Lerp(0, 1, (Projectile.ai[1] - 20) / 10);
                    if (Projectile.ai[1] > 30) scale1 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 30) / 20f);
                    float alpha2 = MathHelper.Lerp(0, 1, MathHelper.Clamp((Projectile.ai[1] - 20) / 20f, 0, 1));
                    for (int i = 0; i < SwordCount; i++)
                    {
                        float rot = Projectile.localAI[0] / 50f + MathHelper.TwoPi / SwordCount * i;
                        Vector2 DrawPos = Projectile.Center + rot.ToRotationVector2() * radius;

                        List<CustomVertexInfo> bars1 = new();
                        List<CustomVertexInfo> bars2 = new();
                        for (int t = 0; t <= 20; t++)
                        {
                            float tmpr = MathHelper.Lerp(rot - MathHelper.TwoPi / (SwordCount + 2), rot + MathHelper.Pi / 24f, t / 20f);
                            bars1.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius - 10) - Main.screenPosition, Color.White, new Vector3(t / 20f, 0f, 1f)));
                            bars1.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius + 10) - Main.screenPosition, Color.White, new Vector3(t / 20f, 1f, 1f)));
                            bars2.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius - 7) - Main.screenPosition, Color.White, new Vector3(t / 20f, 0f, 1f)));
                            bars2.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius + 7) - Main.screenPosition, Color.White, new Vector3(t / 20f, 1f, 1f)));
                        }
                        DrawUtils.DrawRoSLaser(TextureLibrary.Ribbon, bars1, Color.Gold * alpha2, 0.3f, 1f, Projectile.localAI[0] / 30f, BlendState.Additive);
                        DrawUtils.DrawRoSLaser(TextureLibrary.Ribbon, bars2, Color.Gold * alpha2, 0.3f, 1f, Projectile.localAI[0] / 30f, BlendState.Additive);
                        EasyDraw.AnotherDraw(BlendState.Additive);
                        Main.instance.LoadProjectile(ProjectileID.SkyFracture);
                        Texture2D texSword = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.SkyFracture].Value;
                        Rectangle rect = new Rectangle((int)(texSword.Width / 14f) * i, 0, (int)(texSword.Width / 14f), texSword.Height);
                        float rotY = rot + MathHelper.Pi / 2f + MathHelper.Pi / 4f;
                        Main.spriteBatch.Draw(texSword, DrawPos - Main.screenPosition, rect, Color.Gold * alpha2, rotY, rect.Size() / 2f, 2f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(texSword, DrawPos - Main.screenPosition, rect, Color.White * alpha2, rotY, rect.Size() / 2f, 1.5f, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureLibrary.BloomFlare, DrawPos - Main.screenPosition, null, Color.Gold, 0, TextureLibrary.BloomFlare.Size() / 2f, 0.2f * scale1, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureLibrary.BloomFlare, DrawPos - Main.screenPosition, null, Color.White, 0, TextureLibrary.BloomFlare.Size() / 2f, 0.15f * scale1, SpriteEffects.None, 0);
                        DrawRing(Projectile.Center, radius * 0.7f, radius * 0.7f, Color.Gold * alpha2 * 0.2f);
                    }
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < SwordCount; i++)
                {
                    float rot = Projectile.localAI[0] / 50f + MathHelper.TwoPi / SwordCount * i;
                    Vector2 DrawPos = Projectile.Center + rot.ToRotationVector2() * radius;

                    List<CustomVertexInfo> bars1 = new();
                    List<CustomVertexInfo> bars2 = new();
                    for (int t = 0; t <= 20; t++)
                    {
                        float tmpr = MathHelper.Lerp(rot - MathHelper.TwoPi / (SwordCount + 2), rot + MathHelper.Pi / 24f, t / 20f);
                        bars1.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius - 10) - Main.screenPosition, Color.White, new Vector3(t / 20f, 0f, 1f)));
                        bars1.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius + 10) - Main.screenPosition, Color.White, new Vector3(t / 20f, 1f, 1f)));
                        bars2.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius - 7) - Main.screenPosition, Color.White, new Vector3(t / 20f, 0f, 1f)));
                        bars2.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius + 7) - Main.screenPosition, Color.White, new Vector3(t / 20f, 1f, 1f)));
                    }
                    DrawUtils.DrawRoSLaser(TextureLibrary.Ribbon, bars1, Color.Gold, 0.3f, 1f, Projectile.localAI[0] / 30f, BlendState.Additive);
                    DrawUtils.DrawRoSLaser(TextureLibrary.Ribbon, bars2, Color.Gold, 0.3f, 1f, Projectile.localAI[0] / 30f, BlendState.Additive);
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Texture2D texSword = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.SkyFracture].Value;
                    Rectangle rect = new Rectangle((int)(texSword.Width / 14f) * i, 0, (int)(texSword.Width / 14f), texSword.Height);
                    float rotY = rot + MathHelper.Pi / 2f + MathHelper.Pi / 4f;
                    Main.spriteBatch.Draw(texSword, DrawPos - Main.screenPosition, rect, Color.Gold, rotY, rect.Size() / 2f, 2f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texSword, DrawPos - Main.screenPosition, rect, Color.White, rotY, rect.Size() / 2f, 1.5f, SpriteEffects.None, 0);

                    DrawRing(Projectile.Center, radius * 0.7f, radius * 0.7f, Color.Gold * 0.2f);
                }
            }
            else if (Projectile.ai[0] == 2)
            {
                float alpha = MathHelper.Lerp(1, 0, Projectile.ai[1] / 30f);
                for (int i = 0; i < SwordCount; i++)
                {
                    float rot = Projectile.localAI[0] / 50f + MathHelper.TwoPi / SwordCount * i;
                    Vector2 DrawPos = Projectile.Center + rot.ToRotationVector2() * radius;

                    List<CustomVertexInfo> bars1 = new();
                    List<CustomVertexInfo> bars2 = new();
                    for (int t = 0; t <= 20; t++)
                    {
                        float tmpr = MathHelper.Lerp(rot - MathHelper.TwoPi / (SwordCount + 2), rot + MathHelper.Pi / 24f, t / 20f);
                        bars1.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius - 10) - Main.screenPosition, Color.White, new Vector3(t / 20f, 0f, 1f)));
                        bars1.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius + 10) - Main.screenPosition, Color.White, new Vector3(t / 20f, 1f, 1f)));
                        bars2.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius - 7) - Main.screenPosition, Color.White, new Vector3(t / 20f, 0f, 1f)));
                        bars2.Add(new CustomVertexInfo(Projectile.Center + tmpr.ToRotationVector2() * (radius + 7) - Main.screenPosition, Color.White, new Vector3(t / 20f, 1f, 1f)));
                    }
                    DrawUtils.DrawRoSLaser(TextureLibrary.Ribbon, bars1, Color.Gold * alpha, 0.3f, 1f, Projectile.localAI[0] / 30f, BlendState.Additive);
                    DrawUtils.DrawRoSLaser(TextureLibrary.Ribbon, bars2, Color.Gold * alpha, 0.3f, 1f, Projectile.localAI[0] / 30f, BlendState.Additive);
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    Texture2D texSword = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.SkyFracture].Value;
                    Rectangle rect = new Rectangle((int)(texSword.Width / 14f) * i, 0, (int)(texSword.Width / 14f), texSword.Height);
                    float rotY = rot + MathHelper.Pi / 2f + MathHelper.Pi / 4f;
                    Main.spriteBatch.Draw(texSword, DrawPos - Main.screenPosition, rect, Color.Gold * alpha, rotY, rect.Size() / 2f, 2f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texSword, DrawPos - Main.screenPosition, rect, Color.White * alpha, rotY, rect.Size() / 2f, 1.5f, SpriteEffects.None, 0);
                    DrawRing(Projectile.Center, radius * 0.7f, radius * 0.7f, Color.Gold * alpha * 0.2f);

                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, -Projectile.localAI[0] / 200f, BlendState.Additive);
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[0] != 1) return false;
            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int radius = GetAOERadius<SpiritGuardiansSpell>() * 16 + (CurrentRing - 3) * 50;
            return targetHitbox.Distance(Projectile.Center) < radius &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }

    }
}