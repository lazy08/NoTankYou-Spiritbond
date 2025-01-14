﻿using NoTankYou.Data.Components;

namespace NoTankYou.Data.Modules
{
    public class FoodModuleSettings : GenericSettings
    {
        public int FoodEarlyWarningTime = 600;
        public bool SavageDuties = false;
        public bool UltimateDuties = false;
        public bool DisableInCombat = true;
        public bool ExtremeUnreal = false;
    }
}
