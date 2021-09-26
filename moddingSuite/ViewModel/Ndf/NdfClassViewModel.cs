using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.Model.Ndfbin;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Filter;

namespace moddingSuite.ViewModel.Ndf
{
    public class NdfClassViewModel : ObjectWrapperViewModel<NdfClass>
    {
        private ICollectionView _instancesCollectionView;
        private readonly ObservableCollection<NdfObjectViewModel> _instances = new ObservableCollection<NdfObjectViewModel>();
        private readonly ObservableCollection<PropertyFilterExpression> _propertyFilterExpressions = new ObservableCollection<PropertyFilterExpression>();

        public NdfClassViewModel(NdfClass obj, ViewModelBase parentVm)
            : base(obj, parentVm)
        {
            foreach (NdfObject instance in obj.Instances)
                Instances.Add(new NdfObjectViewModel(instance, parentVm));

            ApplyPropertyFilter = new ActionCommand(ApplyPropertyFilterExecute);
            AddInstanceCommand = new ActionCommand(AddInstanceExecute);
            RemoveInstanceCommand = new ActionCommand(RemoveInstanceExecute);
        }

        public ICommand AddInstanceCommand { get; protected set; }
        public ICommand RemoveInstanceCommand { get; protected set; }

        public string Name
        {
            get { return Object.Name; }
            set
            {
                Object.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public uint Id
        {
            get { return Object.Id; }
            set
            {
                Object.Id = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<NdfProperty> Properties
        {
            get { return Object.Properties; }
        }

        public ObservableCollection<NdfObjectViewModel> Instances
        {
            get { return _instances; }
        }

        public ICommand ApplyPropertyFilter { get; set; }

        public ObservableCollection<PropertyFilterExpression> PropertyFilterExpressions
        {
            get { return _propertyFilterExpressions; }
        }

        public ICollectionView InstancesCollectionView
        {
            get
            {
                if (_instancesCollectionView == null)
                {
                    _instancesCollectionView = CollectionViewSource.GetDefaultView(Instances);
                    OnPropertyChanged(() => InstancesCollectionView);
                    _instancesCollectionView.CurrentChanged += InstancesCollectionViewCurrentChanged;
                    _instancesCollectionView.Filter = FilterInstances;
                }

                return _instancesCollectionView;
            }
        }

        /// <summary>
        /// Easy instance indexing for scripts.
        /// </summary>
        public NdfObjectViewModel this[uint id] => Instances.FirstOrDefault(obj => obj.Id == id);

        /// <summary>
        /// Allows scripts to append a new instance.
        /// </summary>
        public NdfObjectViewModel AddInstance(bool isTopLevelInstance)
        {
            NdfObject inst = Object.Manager.CreateInstanceOf(Object, isTopLevelInstance);
            Object.Instances.Add(inst);

            var instModel = new NdfObjectViewModel(inst, ParentVm);
            Instances.Add(instModel);
            return instModel;
        }

        /// <summary>
        /// Allows scripts to delete an instance by ID.
        /// </summary>
        public void DeleteInstance(uint id)
        {
            NdfObjectViewModel inst = Instances.FirstOrDefault(obj => obj.Id == id);
            if (inst == null)
                throw new KeyNotFoundException("invalid instance");

            Object.Manager.DeleteInstance(inst.Object);
            Instances.Remove(inst);
        }

        private void RemoveInstanceExecute(object obj)
        {
            var inst = InstancesCollectionView.CurrentItem as NdfObjectViewModel;

            if (inst == null)
                return;

            Object.Manager.DeleteInstance(inst.Object);

            Instances.Remove(inst);
        }

        private void AddInstanceExecute(object obj)
        {
            MessageBoxResult mb = MessageBox.Show("Do you want the new instance to be top level?", "Question",
                                                  MessageBoxButton.YesNo, MessageBoxImage.Question);
            AddInstance(mb == MessageBoxResult.Yes);
        }

        public bool FilterInstances(object o)
        {
            var obj = o as NdfObjectViewModel;

            if (obj == null)
                return false;

            foreach (PropertyFilterExpression expr in PropertyFilterExpressions)
            {
                if (expr.PropertyName == null)
                    continue;

                NdfPropertyValue propVal = obj.PropertyValues.SingleOrDefault(x => x.Property.Name == expr.PropertyName);

                if (propVal == null)
                {
                    return false;
                }

                if (propVal.Value == null || propVal.Value.ToString().ToLower().Equals("null"))
                {
                    if (expr.Value.Length > 0)
                        return false;
                }

                int compare = String.Compare(propVal.Value.ToString(), expr.Value);

                if (expr.Discriminator == FilterDiscriminator.Equals)
                    if (compare == 0)
                        continue;
                    else
                        return false;

                else if (expr.Discriminator == FilterDiscriminator.Smaller)
                    if (propVal.Value.ToString().Length < expr.Value.Length || (propVal.Value.ToString().Length == expr.Value.Length && compare < 0))
                        continue;
                    else
                        return false;

                else if (expr.Discriminator == FilterDiscriminator.Greater)
                    if (propVal.Value.ToString().Length > expr.Value.Length || (propVal.Value.ToString().Length == expr.Value.Length && compare > 0))
                        continue;
                    else
                        return false;
                else if (expr.Discriminator == FilterDiscriminator.Contains)
                    if (propVal.Value.ToString().Contains(expr.Value))
                        continue;
                    else
                        return false;
                else
                    return false;
            }

            return true;
        }

        protected void InstancesCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            foreach (NdfProperty property in Object.Properties)
            {
                property.OnPropertyChanged("Value");
            }
        }

        private void ApplyPropertyFilterExecute(object obj)
        {
            InstancesCollectionView.Refresh();
        }
    }
}