using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class ShatterSpell : BaseSpell
    {
        public override string Name => "Shatter";
        public override Color NameColor => Color.Purple;
        public override int InitialRing => 2;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override DiceDamage BaseDamage => new(8, 3, DamageElement.Thunder, CombatStat.Ring2Damage);
        public override DiceDamage RisingDamageAddition => new(8, 1, DamageElement.Thunder, CombatStat.Ring2Damage);
        public override int SpellRange => 36;
        public override int AOERadius => 18;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<ShatterProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring); ;
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
                Vector2 TargetPosition;
                if ((Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM)       //穿墙修正
                {
                    TargetPosition = SomeUtils.GetNoBlockEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                else
                {
                    TargetPosition = SomeUtils.GetTileBlockedEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                Main.projectile[protmp].Center = TargetPosition;
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
            }
            if (!Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1) && !owner.CarefulSpellMM())
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;

            List<CustomVertexInfo> bars2 = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + miscTimer;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * 40;
                bars2.Add(new CustomVertexInfo(tipPos + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars2.Add(new CustomVertexInfo(tipPos + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars2, Color.White * 0.8f * light, 0.4f, 2f, -miscTimer * 4, BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                List<CustomVertexInfo> bars = new()
                {
                    new CustomVertexInfo(tipPos + new Vector2(-30,-3).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(-30,3).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(0,-7).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(0,7).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
                DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * light, 0.4f, 1f, miscTimer * 2 + 0.4f * i, BlendState.Additive);
            }
            return false;
        }
    }
}
