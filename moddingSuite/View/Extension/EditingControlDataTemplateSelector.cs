using System.Windows;
using System.Windows.Controls;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types;
using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace moddingSuite.View.Extension
{
    public class EditingControlDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (!(item is IValueHolder) || element == null)
                return null;

            var propVal = item as IValueHolder;

            NdfValueWrapper wrap = propVal.Value;

            if (wrap == null)
                return null;

            switch (wrap.Type)
            {
                case NdfType.Float32:
                case NdfType.Float64:
                    return element.FindResource("FloatEditingTemplate") as DataTemplate;
                case NdfType.UInt16:
                case NdfType.UInt32:
                    return element.FindResource("UInt32EditingTemplate") as DataTemplate;
                case NdfType.Int32:
                case NdfType.Int8:
                case NdfType.Int16:
                    return element.FindResource("Int32EditingTemplate") as DataTemplate;
                case NdfType.Guid:
                    return element.FindResource("GuidEditingTemplate") as DataTemplate;
                case NdfType.Boolean:
                    return element.FindResource("BooleanEditingTemplate") as DataTemplate;

                case NdfType.Color32:
                    return element.FindResource("ColorPickerEditingTemplate") as DataTemplate;

                case NdfType.Vector:
                    return element.FindResource("VectorEditingTemplate") as DataTemplate;
                    

                case NdfType.Map:
                    return element.FindResource("MapEditingTemplate") as DataTemplate;

                case NdfType.ObjectReference:
                    return element.FindResource("ObjectReferenceEditingTemplate") as DataTemplate;

                case NdfType.LocalisationHash:
                    return element.FindResource("LocaleHashEditingTemplate") as DataTemplate;

                case NdfType.TableString:
                case NdfType.TableStringFile:
                    return element.FindResource("StringTableEditingTemplate") as DataTemplate;

                case NdfType.TransTableReference:
                    return element.FindResource("TransTableReferenceEditingTemplate") as DataTemplate;

                case NdfType.Blob:
                    return element.FindResource("BlobEditingTemplate") as DataTemplate;

                case NdfType.EugFloat2:
                    return element.FindResource("FloatPairEditingTemplate") as DataTemplate;

                case NdfType.List:
                    return null;
            }


            return null;
        }
    }
}