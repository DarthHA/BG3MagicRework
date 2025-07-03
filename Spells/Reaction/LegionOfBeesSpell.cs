using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Reaction
{
    public class LegionOfBeesSpell : BaseSpell
    {
        public override string Name => "LegionOfBees";
        public override Color NameColor => Color.Yellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Transmutation;
        public override DiceDamage BaseDamage => new(10, 2, DamageElement.None, CombatStat.Ring1Damage);

        public override DiceDamage RisingDamageAddition => new(10, 1, DamageElement.None, CombatStat.Ring1Damage);
        public override int InitialRing => 1;
        public override int SpellRange => 0;
        public override bool Concentration => false;
        public override int TimeSpan => 0;
        public override bool IsReaction => true;
        public override int ReactionCD => 6 * 60;

        public override bool ReactionEffect(Player player, int usedRing, float extraInfo1, float extraInfo2, float extraInfo3, float extraInfo4)
        {
            int Target = (int)extraInfo1;
            Vector2 ShootVel = (Main.npc[Target].Center - player.Center) * 30;
            int protmp = player.NewMagicProj(player.Center, ShootVel, ModContent.ProjectileType<LegionOfBeesProj>(), player.GetDiceDamage(BaseDamage, InitialRing, usedRing, RisingDamageAddition), 0, usedRing);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).CurrentRing = usedRing;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, false, false, false, false, true);  //注意：这里穿墙超魔在之前判过一次了
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM = extraInfo2 == 1;
                (Main.projectile[protmp].ModProjectile as LegionOfBeesProj).Target = Target;
            }
            AdvancedCombatText.NewText(player.Hitbox, Color.White, string.Format(LangLibrary.TriggerReaction, GetName()));
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                Main.NewText(string.Format(LangLibrary.XUsedReactionX, player.name, GetName()));
            }
            return true;
        }

    }
}
