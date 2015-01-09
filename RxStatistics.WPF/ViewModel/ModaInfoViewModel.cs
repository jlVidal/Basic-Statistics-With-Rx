using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RxStatistics.WPF.ViewModel
{
    public class ModaInfoViewModel : PairViewModel<string,decimal>
    {
        private bool _groupChanges;
        private string _originalKey;
        private IGrouping<int, decimal> _modaGroup;

        public IGrouping<int, decimal> ModaGroup
        {
            get { return _modaGroup; }
        }
        public override string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
                if (!_groupChanges)
                {
                    this._originalKey = value;
                }
            }
        }
        internal void Populate(IGrouping<int, decimal> modaItems)
        {
            if (modaItems == null)
                throw new ArgumentNullException("modaItems");

            this._modaGroup = modaItems;
            this.Value = modaItems.FirstOrDefault();
            _groupChanges = true;
            var count = modaItems.Count();
            if (count > 1)
                this.Key = _originalKey + " (" + count + ")";
            else
                this.Key = _originalKey;
            _groupChanges = false;
        }
    }
}
