using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Items
{
    public class SomeTest : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ItemID.Sets.Deprecated[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Master;
            Item.shoot = ProjectileID.BeeArrow;
            Item.shootSpeed = 10;
        }
        public override void UpdateInventory(Player player)
        {
            if (Item.favorited)
            {
                player.GetModPlayer<DNDMagicPlayer>().ConsumedSpellSlot.Clear();
                player.GetModPlayer<DNDMagicPlayer>().ConsumedSorceryPoint = 0;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //player.GetModPlayer<DNDMagicPlayer>().ConsumedSpellSlot.Clear();
            //player.GetModPlayer<DNDMagicPlayer>().ConsumedSorceryPoint = 0;
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }
    }
}
