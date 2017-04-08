using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.View.Ndfbin.Viewer;
using moddingSuite.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace moddingSuite.ViewModel.Ndf
{
    class ArmourDamageViewModel : ObjectWrapperViewModel<NdfObject>
    {
        public ArmourDamageViewModel(NdfObject obj, ViewModelBase parentVm)
            : base(obj, parentVm)
        {
            Name = obj.Class.Name;
            switch (obj.Class.Name) {
                case "TGameplayArmeArmureContainer":
                    readTGameplayArmeArmureContainer(obj);
                    break;
                case "TGameplayDamageResistanceContainer":
                    readTGameplayDamageResistanceContainer(obj);
                    break;
                default:
                    throw new Exception(string.Format("Cannot read object {0} as ArmourDamageViewModel", obj.Class.Name));
            }

        }

        private void readTGameplayArmeArmureContainer(NdfObject obj)
        {
            var maps = obj.PropertyValues[0].Value as NdfCollection;
            for (var i = 0; i < 42; i++)
            {
                TableData.Columns.Add(i.ToString());
            }
            for (var i = 0; i < 70; i++)
            {
                var map = maps.FirstOrDefault(x => ((NdfMap)x.Value).Key.Value.ToString().Equals(i.ToString()));
                if (map != null)
                {
                    var collection = ((MapValueHolder)((NdfMap)map.Value).Value).Value as NdfCollection;
                    var row = TableData.NewRow();
                    TableData.Rows.Add(row);
                    for (var j = 0; j < collection.Count; j++)
                    {
                        row[j] = collection[j].Value;
                    }
                }
            }
            TableData.AcceptChanges();
        }

        private void readTGameplayDamageResistanceContainer(NdfObject obj)
        {


            var armourFamilies = obj.PropertyValues[1].Value as NdfCollection;
            var damageFamilies = obj.PropertyValues[0].Value as NdfCollection;
            var values = obj.PropertyValues[2].Value as NdfCollection;
            TableData.Clear();
            TableData.TableName = "table";
            foreach (var armourFamily in armourFamilies) {
                TableData.Columns.Add(armourFamily.Value.ToString());
            }
            int k = 0;
            for (var i = 0; i < damageFamilies.Count; i++)
            {
                RowHeaders.Add(damageFamilies[i].Value.ToString());
                var row = TableData.NewRow();
                TableData.Rows.Add(row);
                var damageFamily = new ObservableCollection<NdfPropertyValue>();
                for (var j = 0; j < armourFamilies.Count; j++)
                {
                    row[j] = values[k++].Value;
                }

            }
            TableData.AcceptChanges();
        }
        
        public DataTable TableData { get; } = new DataTable();
        public ObservableCollection<string> RowHeaders { get; } = new ObservableCollection<string>();
        public string Name;
        /*public ObservableCollection<ObservableCollection<NdfPropertyValue>> TableData { get; } =
            new ObservableCollection<ObservableCollection<NdfPropertyValue>>();*/
    }
}
