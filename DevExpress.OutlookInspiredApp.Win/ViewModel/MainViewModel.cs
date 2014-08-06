﻿namespace DevExpress.OutlookInspiredApp.Win.ViewModel {
    using System;
    using DevExpress.DevAV.ViewModels;
    using DevExpress.Mvvm.DataAnnotations;
    using DevExpress.Mvvm.POCO;
    using DevExpress.OutlookInspiredApp.Services;

    public class MainViewModel : DevExpress.Mvvm.ViewModelBase, IZoomViewModel {
        #region static
        static MainViewModel() {
            DevExpress.Mvvm.ServiceContainer.Default.RegisterService(new Services.Win.ModuleResourceProvider());
            DevExpress.Mvvm.ServiceContainer.Default.RegisterService(new MessageBoxService());
            DevExpress.Mvvm.ServiceContainer.Default.RegisterService(new Services.ModuleTypesResolver());
            DevExpress.Mvvm.ServiceContainer.Default.RegisterService(new Services.ModuleResourceProvider());
        }
        #endregion static
        public MainViewModel(IMainModule mainModule) {
            RegisterServices(mainModule);
        }
        void RegisterServices(IMainModule mainModule) {
            var mainModuleType = mainModule.GetType();
            ServiceContainer.RegisterService(new Services.WaitingService());
            ServiceContainer.RegisterService(new Services.ModuleActivator(
                mainModuleType.Assembly, mainModuleType.Namespace + ".Modules"));
            ServiceContainer.RegisterService(new Services.ReportActivator());
            ServiceContainer.RegisterService(new Services.ModuleLocator(ServiceContainer));
            ServiceContainer.RegisterService(new Services.ReportLocator(ServiceContainer));
            ServiceContainer.RegisterService(new Services.TransitionService(mainModule));
            ServiceContainer.RegisterService(new Services.PeekModulesHostingService(mainModule));
            ServiceContainer.RegisterService(new Services.WorkspaceService(mainModule));
        }
        #region Properties
        public virtual ModuleType SelectedModuleType {
            get;
            set;
        }
        public virtual object SelectedModule {
            get;
            set;
        }
        public ModuleType SelectedNavPaneModuleType {
            get { return GetNavPaneModuleType(SelectedModuleType); }
        }
        public ModuleType SelectedNavPaneHeaderModuleType {
            get { return GetNavPaneModuleType(SelectedModuleType, true); }
        }
        public ModuleType SelectedExportModuleType {
            get { return GetExportModuleType(SelectedModuleType); }
        }
        public ModuleType SelectedPrintModuleType {
            get { return GetPrintModuleType(SelectedModuleType); }
        }
        public object SelectedModuleViewModel {
            get { return ((Modules.ISupportViewModel)SelectedModule).ViewModel; }
        }
        #endregion Properties
        #region Commands
        public bool CanSelectModule(ModuleType moduleType) {
            return SelectedModuleType != moduleType;
        }
        [Command(UseCommandManager = false)]
        public void SelectModule(ModuleType moduleType) {
            SelectedModuleType = moduleType;
        }
        public bool CanDockPeekModule(ModuleType moduleType) {
            var peekModuleType = GetPeekModuleType(moduleType);
            return !GetService<IPeekModulesHostingService>().IsDocked(peekModuleType);
        }
        [Command(UseCommandManager = false)]
        public void DockPeekModule(ModuleType moduleType) {
            var peekModuleType = GetPeekModuleType(moduleType);
            GetService<IPeekModulesHostingService>().DockModule(peekModuleType);
        }
        public bool CanUndockPeekModule(ModuleType moduleType) {
            var peekModuleType = GetPeekModuleType(moduleType);
            return GetService<IPeekModulesHostingService>().IsDocked(peekModuleType);
        }
        [Command(UseCommandManager = false)]
        public void UndockPeekModule(ModuleType moduleType) {
            var peekModuleType = GetPeekModuleType(moduleType);
            GetService<IPeekModulesHostingService>().UndockModule(peekModuleType);
        }
        public bool CanShowPeekModule(ModuleType moduleType) {
            var peekModuleType = GetPeekModuleType(moduleType);
            return !GetService<IPeekModulesHostingService>().IsDocked(peekModuleType);
        }
        [Command(UseCommandManager = false)]
        public void ShowPeekModule(ModuleType moduleType) {
            var peekModuleType = GetPeekModuleType(moduleType);
            GetService<IPeekModulesHostingService>().ShowModule(peekModuleType);
        }
        [Command]
        public void GetStarted() {
            AppHelper.ProcessStart(AssemblyInfo.DXLinkGetStarted);
        }
        [Command]
        public void GetSupport() {
            AppHelper.ProcessStart(AssemblyInfo.DXLinkGetSupport);
        }
        [Command]
        public void BuyNow() {
            AppHelper.ProcessStart(AssemblyInfo.DXLinkBuyNow);
        }
        [Command]
        public void About() {
            DevExpress.Utils.About.AboutForm.Show(DevExpress.Utils.About.ProductKind.DXperienceWin,
                new DevExpress.Utils.About.ProductStringInfo("Outlook Inspired App", "WinForms Controls"));
        }
        #endregion
        #region FiltersVisibility
        public virtual bool IsReadingMode { get; set; }
        [Command(UseCommandManager = false)]
        public void TurnOnReadingMode() {
            IsReadingMode = true;
        }
        public bool CanTurnOnReadingMode() {
            return !IsReadingMode;
        }
        [Command(UseCommandManager = false)]
        public void TurnOffReadingMode() {
            IsReadingMode = false;
        }
        public bool CanTurnOffReadingMode() {
            return IsReadingMode;
        }
        public virtual CollectionViewFiltersVisibility FiltersVisibility { get; set; }
        [Command(UseCommandManager = false)]
        public void ShowFilters() {
            FiltersVisibility = CollectionViewFiltersVisibility.Visible;
        }
        public bool CanShowFilters() {
            return FiltersVisibility != CollectionViewFiltersVisibility.Visible;
        }
        [Command(UseCommandManager = false)]
        public void MinimizeFilters() {
            FiltersVisibility = CollectionViewFiltersVisibility.Minimized;
        }
        public bool CanMinimizeFilters() {
            return FiltersVisibility != CollectionViewFiltersVisibility.Minimized;
        }
        [Command(UseCommandManager = false)]
        public void HideFilters() {
            FiltersVisibility = CollectionViewFiltersVisibility.Hidden;
        }
        public bool CanHideFilters() {
            return FiltersVisibility != CollectionViewFiltersVisibility.Hidden;
        }
        public event EventHandler IsReadingModeChanged;
        protected virtual void OnIsReadingModeChanged() {
            this.RaiseCanExecuteChanged(x => x.TurnOnReadingMode());
            this.RaiseCanExecuteChanged(x => x.TurnOffReadingMode());
            RaiseIsReadingModeChanged();
        }
        void RaiseIsReadingModeChanged() {
            EventHandler handler = IsReadingModeChanged;
            if(handler != null)
                handler(this, EventArgs.Empty);
        }
        public event EventHandler ViewFiltersVisibilityChanged;
        protected virtual void OnFiltersVisibilityChanged() {
            this.RaiseCanExecuteChanged(x => x.ShowFilters());
            this.RaiseCanExecuteChanged(x => x.MinimizeFilters());
            this.RaiseCanExecuteChanged(x => x.HideFilters());
            RaiseFiltersVisibilityChanged();
        }
        void RaiseFiltersVisibilityChanged() {
            EventHandler handler = ViewFiltersVisibilityChanged;
            if(handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion
        bool IsModuleLoaded(ModuleType type) {
            return GetService<Services.IModuleLocator>().IsModuleLoaded(type);
        }
        public object GetModule(ModuleType type) {
            return GetService<Services.IModuleLocator>().GetModule(type);
        }
        public object GetModule(ModuleType type, object viewModel) {
            return GetService<Services.IModuleLocator>().GetModule(type, viewModel);
        }
        public string GetModuleName(ModuleType type) {
            return GetService<Services.IModuleTypesResolver>().GetName(type);
        }
        public System.Guid GetModuleID(ModuleType type) {
            return GetService<Services.IModuleTypesResolver>().GetId(type);
        }
        public string GetModuleCaption(ModuleType type) {
            return GetService<Services.IModuleResourceProvider>().GetCaption(type);
        }
        public object GetModuleImage(ModuleType type) {
            return GetService<Services.IModuleResourceProvider>().GetModuleImage(type);
        }
        public ModuleType GetMainModuleType(ModuleType type) {
            return GetService<Services.IModuleTypesResolver>().GetMainModuleType(type);
        }
        public ModuleType GetNavPaneModuleType(ModuleType type, bool collapsed = false) {
            var resolver = GetService<Services.IModuleTypesResolver>();
            return collapsed ? resolver.GetNavPaneHeaderModuleType(type) : resolver.GetNavPaneModuleType(type);
        }
        public ModuleType GetPeekModuleType(ModuleType type) {
            return GetService<Services.IModuleTypesResolver>().GetPeekModuleType(type);
        }
        public ModuleType GetExportModuleType(ModuleType type) {
            return GetService<Services.IModuleTypesResolver>().GetExportModuleType(type);
        }
        public ModuleType GetPrintModuleType(ModuleType type) {
            return GetService<Services.IModuleTypesResolver>().GetPrintModuleType(type);
        }
        protected virtual void OnSelectedModuleTypeChanged(ModuleType oldType) {
            var transitionService = GetService<Services.ITransitionService>();
            bool effective = (SelectedModuleType != ModuleType.Unknown) && (oldType != ModuleType.Unknown);
            object waitParameter = !IsModuleLoaded(SelectedModuleType) ? (object)SelectedModuleType : null;
            using(transitionService.EnterTransition(effective, ((int)SelectedModuleType > (int)oldType), waitParameter)) {
                var workspaceService = GetService<Services.IWorkspaceService>();
                var resolver = GetService<IModuleTypesResolver>();
                if(oldType != ModuleType.Unknown)
                    workspaceService.SaveWorkspace(resolver.GetName(oldType));
                else
                    workspaceService.SetupDefaultWorkspace();
                SelectedModule = GetModule(SelectedModuleType);
                RaiseSelectedModuleTypeChanged();
                if(SelectedModuleType != ModuleType.Unknown)
                    workspaceService.RestoreWorkspace(resolver.GetName(SelectedModuleType));
            }
        }
        protected virtual void OnSelectedModuleChanged(object oldModule) {
            if(oldModule != null) {
                if(ModuleRemoved != null)
                    ModuleRemoved(oldModule, EventArgs.Empty);
            }
            if(SelectedModuleChanged != null)
                SelectedModuleChanged(this, EventArgs.Empty);
            if(SelectedModule != null) {
                ViewModelHelper.EnsureModuleViewModel(SelectedModule, this);
                if(ModuleAdded != null)
                    ModuleAdded(SelectedModule, EventArgs.Empty);
            }
        }
        protected virtual void RaiseSelectedModuleTypeChanged() {
            this.RaiseCanExecuteChanged(x => x.SelectModule(ModuleType.Unknown));
            RaisePropertyChanged(() => SelectedNavPaneModuleType);
            RaisePropertyChanged(() => SelectedNavPaneHeaderModuleType);
            if(SelectedModuleTypeChanged != null)
                SelectedModuleTypeChanged(this, EventArgs.Empty);
        }
        public event EventHandler ModuleAdded;
        public event EventHandler ModuleRemoved;
        public event EventHandler SelectedModuleChanged;
        public event EventHandler SelectedModuleTypeChanged;
        //
        public event EventHandler<PrintEventArgs> Print;
        public object ReportParameter { get; private set; }
        ModuleType currentReportModule;
        internal void BeforeReportShown(ModuleType moduleType) {
            if(ReportParameter != null)
                return;
            switch(moduleType) {
                case ModuleType.EmployeesExport:
                case ModuleType.EmployeesPrint:
                    ReportParameter = EmployeeReportType.Profile;
                    break;
                case ModuleType.CustomersExport:
                case ModuleType.CustomersPrint:
                    ReportParameter = CustomerReportType.Profile;
                    break;
                case ModuleType.ProductsExport:
                case ModuleType.ProductsPrint:
                    ReportParameter = ProductReportType.OrderDetail;
                    break;
                case ModuleType.OrdersExport:
                case ModuleType.OrdersPrint:
                    ReportParameter = SalesReportType.Invoice;
                    break;
            }
        }
        internal void AfterReportShown(ModuleType moduleType) {
            if(currentReportModule != moduleType) {
                bool reportChanged = currentReportModule != ModuleType.Unknown;
                this.currentReportModule = moduleType;
                if(reportChanged && moduleType != ModuleType.Unknown) {
                    var reportViewModel = ((Modules.ISupportViewModel)GetModule(moduleType)).ViewModel as ReportViewModelBase;
                    if(reportViewModel != null)
                        reportViewModel.OnReload();
                }
            }
        }
        internal void AfterReportHidden() {
            this.currentReportModule = ModuleType.Unknown;
            ReportParameter = null;
        }
        internal void RaisePrint(object parameter) {
            ReportParameter = parameter;
            EventHandler<PrintEventArgs> handler = Print;
            if(handler != null)
                handler(this, new PrintEventArgs(parameter));
        }
        public event EventHandler ShowAllFolders;
        internal void RaiseShowAllFolders() {
            EventHandler handler = ShowAllFolders;
            if(handler != null)
                handler(this, EventArgs.Empty);
        }
        #region ISupportZoomModule Members
        object IZoomViewModel.ZoomModule {
            get { return SelectedModule; }
        }
        event EventHandler IZoomViewModel.ZoomModuleChanged {
            add { SelectedModuleChanged += value; }
            remove { SelectedModuleChanged -= value; }
        }
        #endregion
    }
    public class PrintEventArgs : EventArgs {
        public PrintEventArgs(object parameter) {
            this.Parameter = parameter;
        }
        public object Parameter { get; private set; }
    }
}