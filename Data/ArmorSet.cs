using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace BG3MagicRework.Data
{
    public class TestArmorSet : BaseArmorSetModifier
    {
        public override string Name => "TestEffect";
        public override List<int> Head => new() { ItemID.NebulaHelmet };
        public override List<int> Body => new() { ItemID.NebulaBreastplate };
        public override List<int> Legs => new() { ItemID.NebulaLeggings };
        public override void UpdateArmorSet(Player player)
        {
            player.setNebula = false;
            DNDMagicPlayer magicPlayer = player.GetModPlayer<DNDMagicPlayer>();
            player.AddSpellSlot(1, 5);
            player.AddSpellSlot(2, 4);
            player.AddSpellSlot(3, 3);
            player.AddSpellSlot(4, 2);
            player.AddSpellSlot(5, 1);
            player.AddSpellSlot(6, 1);
            magicPlayer.MaxSorceryPoint += 4;
            magicPlayer.MaxConcentrationCount += 2;
            magicPlayer.CantripLevel += 4;
            /*
            magicPlayer.DamageAdditionA += new DiceDamage(4, 1, DamageElement.Fire, 1);
            magicPlayer.DamageAdditionA += new DiceDamage(4, 1, DamageElement.Cold, 1);
            magicPlayer.DamageAdditionB += new DiceDamage(4, 1, DamageElement.Force, 1);
            magicPlayer.DamageAdditionB += new DiceDamage(4, 1, DamageElement.Thunder, 1);
            magicPlayer.DamageAdditionC += new DiceDamage(4, 1, DamageElement.Radiant, 1);
            magicPlayer.DamageAdditionC += new DiceDamage(4, 1, DamageElement.Necrotic, 1);
            */
        }
    }
}