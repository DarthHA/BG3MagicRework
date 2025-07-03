using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class KnockProj : BaseMagicProj
    {
        public bool IsNPC = false;
        public int TargetIndex = -1;
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
            if (TargetIndex == -1)
            {
                Projectile.Kill();
                return;
            }
            if (IsNPC)
            {
                if (!Main.npc[TargetIndex].active || Main.npc[TargetIndex].type != NPCID.BoundTownSlimeOld)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Center = Main.npc[TargetIndex].Center;
                if (Projectile.ai[1] == 40)
                {
                    NPC.unlockedSlimeOldSpawn = false;
                    NPC.TransformElderSlime(TargetIndex);
                    SoundEngine.PlaySound(SoundID.Unlock);
                }
            }
            else
            {
                if (Main.chest[TargetIndex] == null)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Center = new Vector2(Main.chest[TargetIndex].x, Main.chest[TargetIndex].y) * 16;
                if (Projectile.ai[1] == 40)
                {
                    if (Chest.IsLocked(Main.chest[TargetIndex].x, Main.chest[TargetIndex].y))
                    {
                        Chest.Unlock(Main.chest[TargetIndex].x, Main.chest[TargetIndex].y);
                    }
                }
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] > 60)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float alpha1 = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
            if (Projectile.ai[1] > 20)
            {
                alpha1 = MathHelper.Lerp(1, 0, MathHelper.Clamp((Projectile.ai[1] - 20) / 10f, 0, 1));
            }
            float alpha2 = MathHelper.Lerp(0, 1, MathHelper.Clamp((Projectile.ai[1] - 20) / 10f, 0, 1));
            if (Projectile.ai[1] > 40)
            {
                alpha2 = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 40) / 20f);
            }
            if (IsNPC)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Texture2D texChest = Terraria.GameContent.TextureAssets.Tile[21].Value;
                        Rectangle rectChest = new(18 * i, 18 * j, 18, 18);
                        Vector2 DrawPos = Projectile.Center + new Vector2(i - 1, j - 1) * 16;

                        int crackCounter = (int)(Projectile.ai[1] / 5);
                        if (crackCounter > 3) crackCounter = 3;
                        Rectangle rectCrack = new();
                        if (i == 0 && j == 0) rectCrack = new Rectangle(16 * 0, 16 * crackCounter, 16, 16);
                        if (i == 1 && j == 0) rectCrack = new Rectangle(16 * 1, 16 * crackCounter, 16, 16);
                        if (i == 0 && j == 1) rectCrack = new Rectangle(16 * 2, 16 * crackCounter, 16, 16);
                        if (i == 1 && j == 1) rectCrack = new Rectangle(16 * 3, 16 * crackCounter, 16, 16);
                        Color colorChest = Color.LightCyan * alpha2;
                        Color colorCrack = Color.LightCyan * alpha1;
                        DrawUtils.DrawCrack(texChest, TextureLibrary.TileCracks, rectChest, rectCrack, DrawPos, colorCrack);
                        DrawUtils.DrawWhite(texChest, DrawPos, rectChest, colorChest, 0, Vector2.Zero, 1f, SpriteEffects.None, BlendState.AlphaBlend);
                    }
                }
            }
            else
            {
                int x = Main.chest[TargetIndex].x;
                int y = Main.chest[TargetIndex].y;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Tile tile = Main.tile[x + i, y + j];
                        Texture2D texChest = Terraria.GameContent.TextureAssets.Tile[tile.TileType].Value;
                        Rectangle rectChest = new(tile.TileFrameX, tile.TileFrameY, 18, 18);
                        Vector2 DrawPos = new Vector2(x + i, y + j) * 16;

                        int crackCounter = (int)(Projectile.ai[1] / 5);
                        if (crackCounter > 3) crackCounter = 3;
                        Rectangle rectCrack = new();
                        if (i == 0 && j == 0) rectCrack = new Rectangle(16 * 0, 16 * crackCounter, 16, 16);
                        if (i == 1 && j == 0) rectCrack = new Rectangle(16 * 1, 16 * crackCounter, 16, 16);
                        if (i == 0 && j == 1) rectCrack = new Rectangle(16 * 2, 16 * crackCounter, 16, 16);
                        if (i == 1 && j == 1) rectCrack = new Rectangle(16 * 3, 16 * crackCounter, 16, 16);
                        Color colorChest = Color.LightCyan * alpha2;
                        Color colorCrack = Color.LightCyan * alpha1;
                        DrawUtils.DrawCrack(texChest, TextureLibrary.TileCracks, rectChest, rectCrack, DrawPos, colorCrack);
                        DrawUtils.DrawWhite(texChest, DrawPos, rectChest, colorChest, 0, Vector2.Zero, 1f, SpriteEffects.None, BlendState.AlphaBlend);
                    }
                }
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}
