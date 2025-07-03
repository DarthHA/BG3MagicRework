using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Reaction
{
    public class HellishRebukeSpell : BaseSpell
    {
        public override string Name => "HellishRebuke";
        public override Color NameColor => Color.OrangeRed;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 1;
        public override DiceDamage BaseDamage => new(10, 2, DamageElement.Fire, CombatStat.Ring1Damage);

        public override DiceDamage RisingDamageAddition => new(10, 1, DamageElement.Fire, CombatStat.Ring1Damage);
        public override bool Concentration => false;
        public override int TimeSpan => 0;
        public override bool IsReaction => true;
        public override int ReactionCD => 6 * 60;

        public override bool ReactionEffect(Player player, int usedRing, float extraInfo1, float extraInfo2, float extraInfo3, float extraInfo4)
        {
            int Target = (int)extraInfo1;
            bool twinned = extraInfo3 == 1;
            if (Main.npc[Target].CanBeChasedBy() || Main.npc[Target].immortal)
            {
                int protmp = player.NewMagicProj(Main.npc[Target].Center, Vector2.Zero, ModContent.ProjectileType<HellishRebukeProj>(), player.GetDiceDamage(BaseDamage, InitialRing, usedRing, RisingDamageAddition), 0, usedRing);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as HellishRebukeProj).NPCTarget = Target;
                    if (twinned)
                    {
                        (Main.projectile[protmp].ModProjectile as BaseMagicProj).TwinnedSpellMM = true;
                    }
                }
                AdvancedCombatText.NewText(player.Hitbox, Color.White, string.Format(LangLibrary.TriggerReaction, GetName()));
                if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
                {
                    Main.NewText(string.Format(LangLibrary.XUsedReactionX, player.name, GetName()));
                }
                return true;
            }
            return false;
        }

    }
}
