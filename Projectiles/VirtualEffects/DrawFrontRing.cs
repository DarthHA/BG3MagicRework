using BG3MagicRework.Projectiles.Channel;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class DrawFrontRing : ModProjectile
    {
        public override string Texture => "BG3MagicRework/Images/PlaceHolder";
        public List<float> ringY = new();
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
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
            overPlayers.Add(index);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead() || owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;

            Projectile source = Main.projectile[(int)Projectile.ai[0]];
            if (!source.active)
            {
                Projectile.Kill();
                return;
            }
            if (source.ModProjectile == null || source.ModProjectile is not BaseChannel)
            {
                Projectile.Kill();
                return;
            }
            BaseChannel modproj = source.ModProjectile as BaseChannel;
            while (true)
            {
                if (modproj.currentRing > ringY.Count)
                {
                    ringY.Add(0);
                }
                else if (modproj.currentRing < ringY.Count)
                {
                    ringY.RemoveAt(ringY.Count - 1);
                }
                else break;
            }
            if (ringY.Count > 1)
            {
                for (int i = 0; i < ringY.Count; i++)
                {
                    float TargetY = owner.width / 5f * i;
                    ringY[i] = TargetY;
                }
            }
            else if (ringY.Count == 1)
            {
                float TargetY = 0;
                ringY[0] = TargetY;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Projectile source = Main.projectile[(int)Projectile.ai[0]];
            BaseChannel modproj = source.ModProjectile as BaseChannel;
            EverythingLibrary.spells[modproj.Spell].DrawFront(owner, modproj.currentRing, modproj.Light, modproj.LightColor, modproj.Scale, modproj.miscTimer);
            Texture2D tex = TextureLibrary.Ring2;
            EasyDraw.AnotherDraw(BlendState.Additive);
            foreach (float y in ringY)
            {
                Main.spriteBatch.Draw(tex, owner.Bottom + new Vector2(0, owner.gfxOffY) + new Vector2(0, -owner.height / 8f - y) - Main.screenPosition, null, Color.White * modproj.Light, 0, tex.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public static void KillThis(int wmi)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<DrawFrontRing>() && proj.ai[0] == wmi)
                {
                    proj.Kill();
                }
            }
        }

        public static int SummonThis(int wmi)
        {
            int protmp = Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis("BG3Magic"), Main.projectile[wmi].Center, Vector2.Zero, ModContent.ProjectileType<DrawFrontRing>(), 0, 0, Main.projectile[wmi].owner, wmi);
            return protmp;
        }

        public static int GetThis(int wmi)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<DrawFrontRing>() && proj.ai[0] == wmi)
                {
                    return proj.whoAmI;
                }
            }
            return -1;
        }
    }
}
