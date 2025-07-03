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
    public class SnillocsSnowballSwarmSpell : BaseSpell
    {
        public override string Name => "SnillocsSnowballSwarm";
        public override Color NameColor => Color.Cyan;
        public override int InitialRing => 2;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override DiceDamage BaseDamage => new(6, 3, DamageElement.Cold, CombatStat.Ring2Damage);

        public override DiceDamage RisingDamageAddition => new(6, 1, DamageElement.Cold, CombatStat.Ring2Damage);
        public override int SpellRange => 36;
        public override int AOERadius => 18;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {

            int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<SnillocsSnowballSwarmProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring); ;
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
            Texture2D LightTex = TextureLibrary.BloomFlare;
            List<CustomVertexInfo> bars1 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-40,-15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-40,15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(40,-15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(40,15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            List<CustomVertexInfo> bars2 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-15,-40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-15,40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(15,-40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(15,40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
            return false;
        }


        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            /*
            DrawUtils.DrawIndicatorRing(player.Center, player.GetSpellRange(Name) * 16);

            Vector2 mouseWorld = Main.MouseWorld;
            if (mouseWorld.Distance(player.Center) > player.GetSpellRange(Name) * 16)
            {
                mouseWorld = player.Center + Vector2.Normalize(mouseWorld - player.Center) * player.GetSpellRange(Name) * 16;
            }
            DrawUtils.DrawIndicatorFan(mouseWorld, player.GetAOERadius(Name) * 16f * 0.5f, -MathHelper.Pi, 0);
            Vector2 vec = Vector2.Normalize(new Vector2(-1 / 3f, 1));
            DrawUtils.DrawIndicatorLine(mouseWorld + new Vector2(-player.GetAOERadius(Name) * 16f * 0.5f, 0), mouseWorld + new Vector2(-player.GetAOERadius(Name) * 16f * 0.5f, 0) + vec * 2000);
            vec = Vector2.Normalize(new Vector2(1 / 3f, 1));
            DrawUtils.DrawIndicatorLine(mouseWorld + new Vector2(player.GetAOERadius(Name) * 16f * 0.5f, 0), mouseWorld + new Vector2(player.GetAOERadius(Name) * 16f * 0.5f, 0) + vec * 2000);
            */
            DrawUtils.DrawIndicatorRing(player.Center, player.GetSpellRange(Name) * 16);
            Vector2 mouseWorld = Main.MouseWorld;
            if (mouseWorld.Distance(player.Center) > player.GetSpellRange(Name) * 16)
            {
                mouseWorld = player.Center + Vector2.Normalize(mouseWorld - player.Center) * player.GetSpellRange(Name) * 16;
            }
            DrawUtils.DrawIndicatorRing(mouseWorld, player.GetAOERadius(Name) * 16 * 0.5f);
            return false;
        }
    }
}
