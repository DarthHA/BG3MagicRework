using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Reaction
{
    public class ShieldSpell : BaseSpell
    {
        public override string Name => "Shield";
        public override Color NameColor => Color.Yellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Abjuration;
        public override int InitialRing => 1;
        public override int SpellRange => 0;
        public override bool Concentration => false;
        public override int TimeSpan => 6;
        public override bool IsReaction => true;
        public override int ReactionCD => 6 * 60;

        public override bool ReactionEffect(Player player, int usedRing, float extraInfo1, float extraInfo2, float extraInfo3, float extraInfo4)
        {
            Projectile.NewProjectile(player.GetSource_FromThis("BG3Magic"), player.Center, Vector2.Zero, ModContent.ProjectileType<ShieldSpellProj>(), 0, 0, player.whoAmI);
            AdvancedCombatText.NewText(player.Hitbox, Color.White, string.Format(LangLibrary.TriggerReaction, GetName()));
            if (ModContent.GetInstance<BG3Config>().ShowCombatInfo)
            {
                Main.NewText(string.Format(LangLibrary.XUsedReactionX, player.name, GetName()));
            }
            return true;
        }

    }
}
