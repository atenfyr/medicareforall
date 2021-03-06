﻿using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace MedicareForAll.UI
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
    public class UnitsAttribute : Attribute
    {
        public Unit units;
        public UnitsAttribute(Unit unit)
        {
            units = unit;
        }
    }

    public enum Unit : byte
    {
        Coins,
        Time
    }

    public class SpecialIntRangeElement : RangeElement
    {
        public Unit units;
        public int min;
        public int max;
        public int increment;
        public IList<int> tList;
        public override int NumberTicks => ((max - min) / increment) + 1;
        public override float TickIncrement => (float)increment / (max - min);

        protected override float Proportion
        {
            get => (GetValue() - min) / (float)(max - min);
            set => SetValue((int)Math.Round((value * (max - min) + min) * (1f / increment)) * increment);
        }

        public override void OnBind()
        {
            base.OnBind();
            units = ConfigManager.GetCustomAttribute<UnitsAttribute>(memberInfo, item, list)?.units ?? Unit.Coins;
            tList = (IList<int>)list;
            TextDisplayFunction = () => TransformValue(GetValue(), memberInfo.Name);

            if (tList != null) TextDisplayFunction = () => TransformValue(tList[index], (index + 1).ToString());
            if (labelAttribute != null) TextDisplayFunction = () => TransformValue(GetValue(), labelAttribute.Label);
            if (rangeAttribute != null && rangeAttribute.min is int && rangeAttribute.max is int)
            {
                min = (int)rangeAttribute.min;
                max = (int)rangeAttribute.max;
            }
            if (incrementAttribute != null && incrementAttribute.increment is int) increment = (int)incrementAttribute.increment;
        }

        public string TransformValue(int val, string label)
        {
            string newLabel = label == "value" ? "rent" : label;
            if (units == Unit.Coins) return newLabel + ": Unimplemented";
            return newLabel + ": " + MedicareForAll.UsefulThings.SecondsToHMS(val, "yikes");
        }

        protected int GetValue()
        {
            return (int)GetObject();
        }

        protected void SetValue(object value)
        {
            if (value is int t) SetObject((int)value);
        }

        public SpecialIntRangeElement()
        {
            min = 30;
            max = 1800;
            increment = 50;
        }
    }
}
