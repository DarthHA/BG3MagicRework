using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class MirrorImageSpell : BaseSpell
    {
        public override string Name => "MirrorImage";
        public override Color NameColor => Color.Gold;
        public override SchoolOfMagic Shool => SchoolOfMagic.Illusion;
        public override int InitialRing => 2;
        public override int SpellRange => -1;
        public override int TimeSpan => 360;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            player.NewMagicProj(player.Center, Vector2.Zero, ModContent.ProjectileType<MirrorImageProj>(), Ring);
            if (player.GetProj(ModContent.ProjectileType<MirrorShadow>()) != -1)
            {
                Main.projectile[player.GetProj(ModContent.ProjectileType<MirrorShadow>())].Kill();
            }
            int protmp = Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), player.Center, Vector2.Zero, ModContent.ProjectileType<MirrorShadow>(), 0, 0);     //注意，这个不是BaseMagicProj！
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as MirrorShadow).MaxCount = Ring >= 4 ? 4 : 3;
                (Main.projectile[protmp].ModProjectile as MirrorShadow).CurrentCount = Ring >= 4 ? 4 : 3;
            }
            int timeMulti = player.ActivateMetaMagic(false, false, true, false, false).Extended ? 2 : 1;
            player.AddBuff(ModContent.BuffType<MirrorImageBuff>(), timeMulti * TimeSpan * 60);
            player.GetModPlayer<DNDMagicPlayer>().MirrorImageCount = Ring >= 4 ? 4 : 3;
            AdvancedCombatText.NewText(player.getRect(), Color.White, Lang.GetBuffName(ModContent.BuffType<MirrorImageBuff>()));
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            return true;
        }

        public override void DrawFront(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
            Texture2D tex = TextureLibrary.LightFieldVert;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 30; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 R = rot.ToRotationVector2();
                R.Y *= 0.5f;
                Vector2 Pos1 = owner.Center + R * 50 + new Vector2(0, -1) * 60 / 2f * (float)(Math.Sin(miscTimer * 5 + rot * 3) * 0.5f + 1f);
                Vector2 Pos2 = owner.Center + R * 50 + new Vector2(0, 1) * 60 / 2f;
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0f, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1f, 1f)));
            }
            DrawUtils.DrawLoopTrail(tex, bars, Color.White * light, 0.33f, 0f, BlendState.Additive);
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
            DrawUtils.DrawRoSLaser(texRibbon, bars0, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);

            Texture2D tex = TextureLibrary.LightFieldVert;
            List<CustomVertexInfo> bars1 = new();
            for (int i = 30; i <= 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 R = rot.ToRotationVector2();
                R.Y *= 0.5f;
                Vector2 Pos1 = owner.Center + R * 50 + new Vector2(0, -1) * 60 / 2f * (float)(Math.Sin(miscTimer * 5 + rot * 3) * 0.5f + 1f);
                Vector2 Pos2 = owner.Center + R * 50 + new Vector2(0, 1) * 60 / 2f;
                bars1.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0f, 1f)));
                bars1.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1f, 1f)));
            }
            DrawUtils.DrawLoopTrail(tex, bars1, Color.White * light, 0.33f, 0f, BlendState.Additive);

        }




    }
}
