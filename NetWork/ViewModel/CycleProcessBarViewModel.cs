using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetWork.ViewModel
{
    public class CycleProcessBarViewModel: INotifyPropertyChanged
    {
        public SortedDictionary<double,Color> DicColor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
