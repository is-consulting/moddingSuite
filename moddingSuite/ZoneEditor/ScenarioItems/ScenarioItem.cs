using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using moddingSuite.ViewModel.Ndf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moddingSuite.ZoneEditor.ScenarioItems
{ 

    public abstract class ScenarioItem
    {
        protected string Name="default";
        public Control propertypanel;
        bool selected;
        public override string ToString()
        {
            return Name; 
        }
        public abstract void attachTo(Control c);
        public abstract void setSelected(bool selected);
        public abstract void buildNdf(NdfEditorMainViewModel data,ref int i);
        protected static NdfObject createNdfObject(NdfEditorMainViewModel data,string str){
            var classView= data.Classes.Single(x => x.Object.Name.Equals(str));
            var inst=classView.Object.Manager.CreateInstanceOf(classView.Object, false);
            classView.Object.Instances.Add(inst);
            classView.Instances.Add(new NdfObjectViewModel(inst, data));
            return inst;
        }
        protected static NdfPropertyValue getProperty(NdfObject obj,string str){
            return obj.PropertyValues.Single(x => x.Property.Name.Equals(str));
        }
        protected static NdfFileNameString getAutoName(NdfEditorMainViewModel data, int i)
        {
            var nameStr = string.Format("P0_AutoName_{0}", i);
            return getString(data,nameStr);
        }
        protected static NdfFileNameString getString(NdfEditorMainViewModel data, string nameStr)
        {
            
            var nameRef = data.Strings.Single(x => x.Value.Equals(nameStr));
            return new NdfFileNameString(nameRef);
        }
        
    }
}
