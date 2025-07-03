using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class ArmorOfAgathysSpell : BaseSpell
    {
        public override string Name => "ArmorOfAgathys";
        public override Color NameColor => Color.Cyan;
        public override SchoolOfMagic Shool => SchoolOfMagic.Abjuration;
        public override DiceDamage BaseDamage => new(5, CombatStat.Ring1Damage, DamageElement.Cold);
        public override int InitialRing => 1;
        public override int SpellRange => -1;
        public override bool Concentration => false;
        public override int TimeSpan => -1;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), player.Center, Vector2.Zero, ModContent.ProjectileType<RotateShieldIce>(), 0, 0);     //注意，这个不是BaseMagicProj！
            int shieldValue = 10 * 3 * Ring;
            AdvancedCombatText.NewText(player.getRect(), Color.White, GetName());
            DNDMagicPlayer modplayer = player.GetModPlayer<DNDMagicPlayer>();
            int oldShieldValue = modplayer.ExtraLife;
            if (oldShieldValue > 0)
            {
                AdvancedCombatText.NewText(player.getRect(), Color.White, GetName(), true);
                player.statLife -= oldShieldValue;
                if (player.statLife < 1) player.statLife = 1;
            }
            modplayer.ExtraLife = shieldValue;
            player.statLife += shieldValue;
            modplayer.AoALevel = Ring;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            return true;
        }

        public override void DrawBehind(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars0 = new()
                {
                new CustomVertexInfo(owner.Center + new Vector2(-60,-80).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(owner.Center + new Vector2(-60,80).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(owner.Center + new Vector2(40,-80).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(owner.Center + new Vector2(40,80).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars0, Color.LightCyan * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, bars0, Color.Cyan * light * 0.8f, 0.4f, 1f, miscTimer + 0.5f, BlendState.Additive);
        }
    }
}
