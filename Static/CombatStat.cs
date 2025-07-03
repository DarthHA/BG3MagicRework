namespace BG3MagicRework.Static
{
    public static class CombatStat
    {
        private const float CantripExpectTime = 2f;
        private const float RingExpectTime = 10;
        private const float RepeaterExpectTime = 0.75f;
        private const float L0DPS = 3.5f;
        public const int CantripDamage = (int)(L0DPS * CantripExpectTime);
        public const int GunCantripDamage = (int)(L0DPS * RepeaterExpectTime);
        public const int Ring1Damage = (int)(L0DPS * 2f / 1f * RingExpectTime);
        public const int Ring2Damage = (int)(L0DPS * 4f / 2f * RingExpectTime);
        public const int Ring3Damage = (int)(L0DPS * 8f / 3f * RingExpectTime);
        public const int Ring4Damage = (int)(L0DPS * 16f / 4f * RingExpectTime);
        public const int Ring5Damage = (int)(L0DPS * 32f / 5f * RingExpectTime);
        public const int Ring6Damage = (int)(L0DPS * 64f / 6f * RingExpectTime);

        public const int BurningTimeCantrip = 6 * 60;
        public const int BurningTime = 30 * 60;
        public const int FrozenTime = 6 * 60;

        public const int StartRecoverSpellSlotTime = 12 * 60;
        public const int RecoverSpellSlotTime = 3 * 60;
        public const float SlotRecoverModifier = 1.25f;
        public const int RecoverSorceryPointTime = 12 * 60;

        public const int ManaPotionSicknessCD = 1;//60 * 60;

        public const float ManaRegenPotionValue = 0.2f;
        public const float StarInBottleValue = 0.2f;
        public const int MagicPowerValue = 3;
        public const float ManaRegenBandValue1 = 0.1f;
        public const float ManaRegenBandValue2 = 0.1f;
    }
}
