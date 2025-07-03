using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class CloudOfDaggersSpell : BaseSpell
    {
        public override string Name => "CloudOfDaggers";
        public override Color NameColor => Color.White;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override int InitialRing => 2;
        public override DiceDamage BaseDamage => new(4, 4, DamageElement.None, CombatStat.CantripDamage * 4);

        public override DiceDamage RisingDamageAddition => new(4, 2, DamageElement.None, CombatStat.CantripDamage * 4);
        public override int SpellRange => 36;
        public override int AOERadius => 10;
        public override bool Concentration => true;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<CloudOfDaggersShow>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, true, false, false);
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
            return false;
        }

        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            DrawUtils.DrawIndicatorRing(player.Center, player.GetSpellRange(Name) * 16);
            Vector2 mouseWorld = Main.MouseWorld;
            if (player.GetSpellRange(Name) > 0)
            {
                DrawUtils.DrawIndicatorRing(player.Center, player.GetSpellRange(Name) * 16);
                if (mouseWorld.Distance(player.Center) > player.GetSpellRange(Name) * 16)
                {
                    mouseWorld = player.Center + Vector2.Normalize(mouseWorld - player.Center) * player.GetSpellRange(Name) * 16;
                }
            }
            int width = player.GetAOERadius(Name) * 16;
            DrawUtils.DrawIndicatorLine(mouseWorld + new Vector2(-width, -width), mouseWorld + new Vector2(width, -width));
            DrawUtils.DrawIndicatorLine(mouseWorld + new Vector2(width, -width), mouseWorld + new Vector2(width, width));
            DrawUtils.DrawIndicatorLine(mouseWorld + new Vector2(width, width), mouseWorld + new Vector2(-width, width));
            DrawUtils.DrawIndicatorLine(mouseWorld + new Vector2(-width, width), mouseWorld + new Vector2(-width, -width));
            return false;
        }
    }
}
