using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BG3MagicRework.UI.ResourceOverlay
{
    public class ModifyVanillaLifeOverlay : ModResourceOverlay
    {
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        private Asset<Texture2D> heartTexture, fancyPanelTexture, barsFillingTexture, barsPanelTexture;

        /*
        public override bool PreDrawResourceDisplay(PlayerStatsSnapshot snapshot, IPlayerResourcesDisplaySet displaySet, bool drawingLife, ref Color textColor, out bool drawText)
        {
            int realLifeMax = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().ExtraLife;
            if (realLifeMax < 1) realLifeMax = 1;
            int realLife = Main.LocalPlayer.statLife - Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().ExtraLife;
            if (realLife < 1) realLife = 1;
            snapshot.LifeMax = realLifeMax;
            snapshot.Life = realLife;
            float num = 20f;
            int num2 = Main.LocalPlayer.statLifeMax / 20;
            int num3 = Main.LocalPlayer.ConsumedLifeFruit;
            if (num3 < 0)
                num3 = 0;
            if (num3 > 0)
            {
                num2 = Main.LocalPlayer.statLifeMax / (20 + num3 / 4);
                num = (float)Main.LocalPlayer.statLifeMax / 20f;
            }
            int num4 = realLifeMax - Main.LocalPlayer.statLifeMax;
            num += (float)(num4 / num2);
            snapshot.LifeFruitCount = num3;
            snapshot.AmountOfLifeHearts = (int)(snapshot.LifeMax / num);
            drawText = true;
            return true;
        }
        */

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            bool drawingBarsPanels = CompareAssets(asset, barsFolder + "HP_Panel_Middle");

            float realLifeMax = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().ExtraLife;
            if (realLifeMax < 1) realLifeMax = 1;

            int shouldModify = (int)(Main.LocalPlayer.GetModPlayer<DNDMagicPlayer>().ExtraLife / (realLifeMax / context.snapshot.AmountOfLifeHearts));

            if (shouldModify == 0 || context.resourceNumber > shouldModify)
                return;

            if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
            {
                DrawClassicFancyOverlay(context);
            }
            else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
            {
                DrawClassicFancyOverlay(context);
            }
            else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
            {
                DrawBarsOverlay(context);
            }
            else if (CompareAssets(asset, fancyFolder + "Heart_Left") || CompareAssets(asset, fancyFolder + "Heart_Middle") || CompareAssets(asset, fancyFolder + "Heart_Right") || CompareAssets(asset, fancyFolder + "Heart_Right_Fancy") || CompareAssets(asset, fancyFolder + "Heart_Single_Fancy"))
            {
                DrawFancyPanelOverlay(context);
            }
            else if (drawingBarsPanels)
            {
                DrawBarsPanelOverlay(context);
            }
        }

        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);
            return existingAsset == asset;
        }

        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            context.texture = heartTexture ??= ModContent.Request<Texture2D>("BG3MagicRework/UI/ResourceOverlay/ClassicLifeOverlay");
            context.Draw();
        }

        private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context)
        {
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            Vector2 positionOffset;

            if (context.resourceNumber == context.snapshot.AmountOfLifeHearts)
            {
                if (CompareAssets(context.texture, fancyFolder + "Heart_Single_Fancy"))
                {
                    positionOffset = new Vector2(8, 8);
                }
                else
                {
                    positionOffset = new Vector2(8, 8);
                }
            }
            else if (CompareAssets(context.texture, fancyFolder + "Heart_Left"))
            {
                positionOffset = new Vector2(4, 4);
            }
            else if (CompareAssets(context.texture, fancyFolder + "Heart_Middle"))
            {
                positionOffset = new Vector2(0, 4);
            }
            else
            {
                positionOffset = new Vector2(0, 4);
            }

            context.texture = fancyPanelTexture ??= ModContent.Request<Texture2D>("BG3MagicRework/UI/ResourceOverlay/FancyLifeOverlay_Panel");
            context.source = context.texture.Frame();
            context.position += positionOffset;
            context.Draw();
        }

        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            context.texture = barsFillingTexture ??= ModContent.Request<Texture2D>("BG3MagicRework/UI/ResourceOverlay/BarsLifeOverlay_Fill");
            context.Draw();
        }

        private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context)
        {
            context.texture = barsPanelTexture ??= ModContent.Request<Texture2D>("BG3MagicRework/UI/ResourceOverlay/BarsLifeOverlay_Panel");
            context.source = context.texture.Frame();
            context.position.Y += 6;
            context.Draw();
        }

    }
}
