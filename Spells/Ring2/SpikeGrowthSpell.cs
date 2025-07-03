using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class SpikeGrowthSpell : BaseSpell
    {
        public override string Name => "SpikeGrowth";
        public override Color NameColor => Color.Green;
        public override SchoolOfMagic Shool => SchoolOfMagic.Transmutation;
        public override int InitialRing => 2;
        public override DiceDamage BaseDamage => new(4, 2, DamageElement.None, CombatStat.CantripDamage * 4);
        public override DiceDamage RisingDamageAddition => new();
        public override int SpellRange => 12;
        public override int AOERadius => 20;
        public override bool Concentration => true;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int t = 10;
            int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<SpikeGrowthShow>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as SpikeGrowthShow).numVines = t;
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
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D LightTex = TextureLibrary.BloomFlare;
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            return false;
        }

        public override void DrawBehind(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D tex = TextureLibrary.Ritual;
            Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.DarkOrange * light * 1.2f, miscTimer * owner.direction, tex.Size() / 2f, scale * 0.5f, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

    }
}
