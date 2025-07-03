using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.Ring1
{
    public class ShieldSpellProj : BaseMagicProj
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
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            Projectile.localAI[0]++;
            if (Projectile.localAI[1] > 0) Projectile.localAI[1]--;
            if (Projectile.ai[0] == 0)     //0为护盾维持特效
            {
                Projectile.ai[1]++;
                if (owner.GetModPlayer<DNDMagicPlayer>().ShieldRingActive == 0)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)  //1为护盾消失特效
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 15) Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texHollowCircleSoftEdge = TextureLibrary.HollowCircleSoftEdge;
            Texture2D texLight = TextureLibrary.BloomFlare;
            if (Projectile.localAI[0] < 15)
            {
                float radius1 = MathHelper.Lerp(5, 40, Projectile.localAI[0] / 15f);
                float light1 = MathHelper.Lerp(3, 0, (Projectile.localAI[0] - 5f) / 10f);
                DrawRing(Projectile.Center, radius1, 10, Color.LightBlue * light1);
                DrawRing(Projectile.Center, radius1, 5, Color.White * light1);
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            if (Projectile.localAI[0] < 10)
            {
                float scale2 = MathHelper.Lerp(0, 1, Projectile.localAI[0] / 10f);
                float light2 = MathHelper.Lerp(2, 0, Projectile.localAI[0] / 10f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.LightBlue * light2, 0, texHollowCircleSoftEdge.Size() / 2f, scale2 * 0.25f, SpriteEffects.None, 0);
            }
            if (Projectile.localAI[0] < 15)
            {
                float light3 = 1;
                if (Projectile.localAI[0] > 5)
                {
                    light3 = MathHelper.Lerp(1, 0, (Projectile.localAI[0] - 5) / 10f);
                }
                if (Projectile.localAI[0] > 10)
                {
                    light3 = MathHelper.Lerp(0, 1, (Projectile.localAI[0] - 10) / 5f);
                }
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.LightYellow * light3, 0, texLight.Size() / 2f, 0.09f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texLight, Projectile.Center - Main.screenPosition, null, Color.White * light3, 0, texLight.Size() / 2f, 0.03f, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] == 0)
            {
                float light41 = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.ai[1] / 30f, 0, 1));
                float light42 = MathHelper.Lerp(0, 1, Projectile.localAI[1] / 20f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.LightBlue * light41 * (0.7f + 0.3f * light42), 0, texHollowCircleSoftEdge.Size() / 2f, 0.125f, SpriteEffects.None, 0);
            }
            else if (Projectile.ai[0] == 1)
            {
                float light51 = MathHelper.Lerp(1, 0, Projectile.ai[1] / 15f);
                float light52 = MathHelper.Lerp(0, 1, Projectile.localAI[1] / 20f);
                Main.spriteBatch.Draw(texHollowCircleSoftEdge, Projectile.Center - Main.screenPosition, null, Color.LightBlue * light51 * (0.7f + 0.3f * light52), 0, texHollowCircleSoftEdge.Size() / 2f, 0.125f, SpriteEffects.None, 0);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
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
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.2f, Projectile.ai[1] / 150f, BlendState.Additive);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
