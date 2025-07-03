using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs;
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
    public class MageArmorSpell : BaseSpell
    {
        public override string Name => "MageArmor";
        public override Color NameColor => Color.Gold;
        public override SchoolOfMagic Shool => SchoolOfMagic.Abjuration;
        public override int InitialRing => 1;
        public override int SpellRange => -1;
        public override bool Concentration => false;
        public override int TimeSpan => -1;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), player.Center, Vector2.Zero, ModContent.ProjectileType<RotateShield>(), 0, 0);     //注意，这个不是BaseMagicProj！
            player.GetModPlayer<DNDMagicPlayer>().MageArmorLevel = Ring;
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                Main.NewText(string.Format(LangLibrary.XGetXSecXBuff,
                    player.name,
                    LangLibrary.Infinite,
                    Lang.GetBuffName(ModContent.BuffType<MageArmorBuff>())
                    ));
            }
            AdvancedCombatText.NewText(player.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<MageArmorBuff>()));
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
                new CustomVertexInfo(owner.Center + new Vector2(0,-45) + new Vector2(-40,-20).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(owner.Center + new Vector2(0,-45)  + new Vector2(-40,20).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(owner.Center + new Vector2(0,-45)  + new Vector2(40,-40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(owner.Center + new Vector2(0,-45)  + new Vector2(40,40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars0, Color.LightBlue * 0.8f * light, 0.4f, 1f, miscTimer, BlendState.Additive);

            EasyDraw.AnotherDraw(BlendState.Additive);
            float r = miscTimer * owner.direction;
            float dist = scale * 30f;
            for (float r1 = 0; r1 <= MathHelper.Pi / 6f * 2f; r1 += MathHelper.Pi / 6f)
            {
                for (float r2 = 0; r2 < MathHelper.TwoPi; r2 += MathHelper.TwoPi / 3f)
                {
                    Utils.DrawLine(Main.spriteBatch, owner.Center + (r + r1 + r2).ToRotationVector2() * dist, owner.Center + (r + r1 + r2 + MathHelper.TwoPi / 3f).ToRotationVector2() * (dist + 2), Color.LightPink * light, Color.LightPink * 0.5f * light, 2);
                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

    }
}
