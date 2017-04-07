using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.ViewModel.Ndf
{
    class ArmourDamageViewModel: ObjectWrapperViewModel<NdfObject>
    {
        public ArmourDamageViewModel(NdfObject obj, ViewModelBase parentVm)
            : base(obj, parentVm)
        {
            switch (obj.Class.Name) { 
                case "TGameplayDamageResistanceContainer":
                    readTGameplayDamageResistanceContainer(obj);
                    break;
                default:
                    throw new Exception(string.Format("Cannot read object {0} as ArmourDamageViewModel", obj.Class.Name));
            }
        }

        private void readTGameplayDamageResistanceContainer(NdfObject obj)
        {


            var armourFamilies = obj.PropertyValues[1].Value as NdfCollection;
            var damageFamilies = obj.PropertyValues[0].Value as NdfCollection;
            var values=obj.PropertyValues[2].Value as NdfCollection;
            TableData.Clear();
            TableData.TableName = "table";
            foreach (var armourFamily in armourFamilies) {
                TableData.Columns.Add(armourFamily.Value.ToString());
            }
            int k = 0;
            for (var i=0;i< damageFamilies.Count;i++)
            {
                var row = TableData.NewRow();
                TableData.Rows.Add(row);
                var damageFamily = new ObservableCollection<NdfPropertyValue>();
                for (var j= 0;j < armourFamilies.Count;j++ )
                {
                    row[j]=values[k++].Value;
                }
                
            }
            TableData.WriteXml("data.xml");
            TableData.AcceptChanges();
        }
        public DataTable TableData { get; } = new DataTable();
        /*public ObservableCollection<ObservableCollection<NdfPropertyValue>> TableData { get; } =
            new ObservableCollection<ObservableCollection<NdfPropertyValue>>();*/
    }
}
