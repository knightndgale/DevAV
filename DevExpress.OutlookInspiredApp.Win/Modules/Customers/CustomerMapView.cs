﻿namespace DevExpress.OutlookInspiredApp.Win.Modules {
    using System;
    using System.Linq;
    using DevExpress.DevAV;
    using DevExpress.DevAV.ViewModels;
    using DevExpress.OutlookInspiredApp.Win.Presenters;
    using DevExpress.OutlookInspiredApp.Win.ViewModel;
    using DevExpress.XtraBars.Docking2010;
    using DevExpress.XtraMap;

    public partial class CustomerMapView : BaseModuleControl, IRibbonModule {
        public CustomerMapView()
            : base(CreateViewModel<CustomerMapViewModel>) {
            InitializeComponent();
            //
            Presenter = CreatePresenter();
            //
            BindCommands();
            BindEditors();
            UpdateColors();
            LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
        }
        protected override void OnDisposing() {
            LookAndFeel.StyleChanged -= LookAndFeel_StyleChanged;
            Presenter.Dispose();
            base.OnDisposing();
        }
        public CustomerMapViewModel ViewModel {
            get { return GetViewModel<CustomerMapViewModel>(); }
        }
        protected CustomerSalesMapPresenter Presenter { get; private set; }
        protected virtual CustomerSalesMapPresenter CreatePresenter() {
            return new CustomerSalesMapPresenter(mapControl, ViewModel, UpdateUI, UpdateChart);
        }
        protected virtual void BindCommands() {
            //Save & Close
            biSave.BindCommand(() => ViewModel.Save(), ViewModel);
            biClose.BindCommand(() => ViewModel.Close(), ViewModel);
            biSaveAndClose.BindCommand(() => ViewModel.SaveAndClose(), ViewModel);
            //Delete
            biDelete.BindCommand(() => ViewModel.Delete(), ViewModel);
            //Period
            biLifetime.BindCommand(() => ViewModel.SetLifetimePeriod(), ViewModel);
            biThisYear.BindCommand(() => ViewModel.SetThisYearPeriod(), ViewModel);
            biThisMonth.BindCommand(() => ViewModel.SetThisMonthPeriod(), ViewModel);
            ((WindowsUIButton)periodButtons.Buttons[0]).BindCommand(() => ViewModel.SetThisMonthPeriod(), ViewModel);
            ((WindowsUIButton)periodButtons.Buttons[1]).BindCommand(() => ViewModel.SetThisYearPeriod(), ViewModel);
            ((WindowsUIButton)periodButtons.Buttons[2]).BindCommand(() => ViewModel.SetLifetimePeriod(), ViewModel);
            //Print&Export
            biPrint.ItemClick += (s, e) => mapControl.Print();
            biPrintPreview.ItemClick += (s, e) => mapControl.ShowRibbonPrintPreview();
            barExportItem.ItemClick += (s, e) => mapControl.Export("Customers.Map.png");
        }
        protected virtual void BindEditors() {
            bindingSource.DataSource = ViewModel;
        }
        void LookAndFeel_StyleChanged(object sender, EventArgs e) {
            UpdateColors();
        }
        void UpdateUI(Customer customer) {
            ribbonControl.ApplicationDocumentCaption = customer.Name;
        }
        void UpdateChart(Sales.MapItem salesItem) {
            chart.DataSource = ViewModel.GetSalesByCity(salesItem.City, ViewModel.Period).ToList();
        }
        void UpdateColors() {
            periodButtons.BackColor = ChartHelper.GetBackColor(chart);
            var paletteEntries = chart.GetPaletteEntries(CustomerSalesMapPresenter.PieSegmentColorProvider<ProductCategory>.Count);
            var colorItems = Presenter.PieChartColorizer.ColorItems;
            colorItems.Clear();
            colorItems.BeginUpdate();
            foreach(var item in paletteEntries)
                colorItems.Add(new ColorizerColorTextItem(item.Color));
            colorItems.EndUpdate();
        }
        #region IRibbonModule
        XtraBars.Ribbon.RibbonControl IRibbonModule.Ribbon { get { return ribbonControl; } }
        #endregion
    }
}
