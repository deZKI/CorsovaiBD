using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace CorsovaiBD.Models
{
    public class ComboBoxDataSource : NSComboBoxDataSource
    {
        private readonly List<string> items;
        private readonly NSComboBox comboBox;

        public ComboBoxDataSource(List<string> items, NSComboBox comboBox)
        {
            this.items = items;
            this.comboBox = comboBox;
        }

        public override nint ItemCount(NSComboBox comboBox)
        {
            return items.Count;
        }

        public override NSObject ObjectValueForItem(NSComboBox comboBox, nint index)
        {
            return new NSString(items[(int)index]);
        }

      
    }
}

