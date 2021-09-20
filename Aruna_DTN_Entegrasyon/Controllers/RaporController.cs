using Aruna_DTN_Entegrasyon.Models;
using DevExpress.Export;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aruna_DTN_Entegrasyon.Controllers
{
    

    public class RaporController : Controller
    {
        BusinessLayer MyBll = new BusinessLayer();
        public static DataTable Rapor;

        [Authorize]
        public ActionResult Index(string Rapor = null)
        {
            if(Rapor != null)
            {
                TempData["Rapor"] = Rapor;
            }
            var raporlar = MyBll.GetTanimliRaporlar();
            return View(raporlar);
        }


        [ValidateInput(false)]
        public ActionResult GridViewPartialRapor()
        {
            DataTable dt = new DataTable();
            try
            {
                if(TempData["Rapor"] != null)
                {
                    var rapor = (string)TempData["Rapor"];
                    TempData["Rapor"] = rapor;
                    dt = MyBll.GetRapor(rapor);
                    Rapor = dt;
                }
                return PartialView("_GridViewPartialRapor", dt);
            }
            catch(Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                return PartialView("_GridViewPartialRapor", dt);
            }
            
        }


        public ActionResult ExportTo()
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime day = DateTime.Today;
                var rapor = (string)TempData["Rapor"];
                TempData["Rapor"] = rapor;
                dt = MyBll.GetRapor(rapor);
                Rapor = dt;

                return GridViewExtension.ExportToXls(GridViewHelper.ExportGridViewSettings, Rapor, "Rapor_" + day.Day + "_" + day.Month + "_" + day.Year + "_" + day.Hour + "_" + day.Minute + "_" + day.Second, new XlsExportOptionsEx() { ExportType = ExportType.WYSIWYG });
                
            }
            catch (Exception exc)
            {
                var rapor = (string)TempData["Rapor"];
                TempData["Rapor"] = rapor;

                ModelState.AddModelError("", exc.Message);
                return View();
            }


        }
        public static class GridViewHelper
        {
            private static GridViewSettings exportGridViewSettings;

            public static GridViewSettings ExportGridViewSettings
            {
                get
                {
                    exportGridViewSettings = CreateExportGridViewSettings();
                    return exportGridViewSettings;
                }
            }

            private static GridViewSettings CreateExportGridViewSettings()
            {
                GridViewSettings settings = new GridViewSettings();

                settings.Name = "GridViewRapor";
                settings.CallbackRouteValues = new { Controller = "Rapor", Action = "GridViewPartialRapor" };


                //settings.KeyFieldName = "FatNo";

                settings.EnableCallbackAnimation = false;
                settings.SettingsLoadingPanel.Text = "Lütfen Bekleyin";
                settings.SettingsPager.Visible = true;
                settings.Settings.ShowFooter = true;
                settings.Settings.ShowGroupPanel = true;
                settings.Settings.ShowFilterRow = true;
                settings.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                //settings.Settings.ShowHeaderFilterButton = true;
                //settings.SettingsBehavior.AllowSelectByRowClick = true;



                settings.SettingsBehavior.AllowGroup = true;
                settings.SettingsBehavior.AllowSort = true;
                settings.SettingsBehavior.FilterRowMode = GridViewFilterRowMode.Auto;
                settings.SettingsText.GroupPanel = "Kayıtları gruplamak için kolon başlığını bu alana sürükleyip bırakın";

                settings.SettingsPager.PageSize = 10;
                settings.Styles.AlternatingRow.BackColor = System.Drawing.Color.AliceBlue;
                settings.Styles.Header.BackColor = System.Drawing.Color.LightSkyBlue;
                settings.Styles.Header.ForeColor = System.Drawing.Color.Black;

                //settings.SettingsExport.ReportHeader = "ARUNA RAPORLAMA";


                foreach (DataColumn column in Rapor.Columns)
                {
                    settings.Columns.Add(column.ColumnName);
                }


                return settings;
            }
        }

        //private static GridViewSettings CreateGenelRaporGridViewSettings(string BaslangicTarihi, string BitisTarihi, string SubeKodu, string RaporTuru)
        //{

        //    GridViewSettings settings = new GridViewSettings();

        //    settings.Name = "gvTypedListDataBinding";

        //    settings.CallbackRouteValues = new { Controller = "RaporController", Action = "GridViewPartialRapor"};

        //    settings.SettingsPager.Visible = true;
        //    settings.Settings.ShowGroupPanel = true;
        //    settings.SettingsText.GroupPanel = "Kayıtları gruplamak için kolon başlığını bu alana sürükleyip bırakın";
        //    settings.Settings.ShowFilterRow = true;
        //    settings.SettingsBehavior.AllowSelectByRowClick = true;

        //    settings.SettingsAdaptivity.AdaptiveDetailColumnCount = 1;
        //    settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = false;
        //    settings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = 0;
        //    settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.Off;

        //    //if (TempData["Rapor"] != null)
        //    //{
        //    //    var rapor = (string)TempData["Rapor"];
        //    //    TempData["Rapor"] = rapor;
        //    //    dt = MyBll.GetRapor(rapor);
        //    //}

        //    //if (RaporTuru == "1")
        //    //{
        //    //    #region Calisma Saati

        //    //    settings.Columns.Add("MusteriAdi");
        //    //    settings.Columns.Add("AltMusteriAdi");
        //    //    settings.Columns.Add("ProjeAdi");
        //    //    settings.Columns.Add("IlAdi");
        //    //    settings.Columns.Add("SubeAdi");
        //    //    settings.Columns.Add("RotaAdi");
        //    //    settings.Columns.Add("SurucuAdi");
        //    //    settings.Columns.Add("RolAdi");
        //    //    settings.Columns.Add(column =>
        //    //    {
        //    //        column.FieldName = "Tarih";
        //    //        column.ColumnType = MVCxGridViewColumnType.DateEdit;
        //    //    });
        //    //    settings.Columns.Add(column =>
        //    //    {
        //    //        column.FieldName = "EvHareket_Tarihi";
        //    //        column.ColumnType = MVCxGridViewColumnType.DateEdit;
        //    //        column.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy HH:mm:ss";
        //    //    });

        //    //    settings.Columns.Add(column =>
        //    //    {
        //    //        column.FieldName = "DepoGiris_Tarihi";
        //    //        column.ColumnType = MVCxGridViewColumnType.DateEdit;
        //    //        column.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy HH:mm:ss";
        //    //    });

        //    //    settings.Columns.Add(column =>
        //    //    {
        //    //        column.FieldName = "SonSevkiyat_Cikis";
        //    //        column.ColumnType = MVCxGridViewColumnType.DateEdit;
        //    //        column.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy HH:mm:ss";
        //    //    });

        //    //    settings.Columns.Add(column =>
        //    //    {
        //    //        column.FieldName = "Son_DepoDonus";
        //    //        column.ColumnType = MVCxGridViewColumnType.DateEdit;
        //    //        column.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy HH:mm:ss";
        //    //    });

        //    //    settings.Columns.Add(column =>
        //    //    {
        //    //        column.FieldName = "EvPark_Tarihi";
        //    //        column.ColumnType = MVCxGridViewColumnType.DateEdit;
        //    //        column.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy HH:mm:ss";
        //    //    });

        //    //    #endregion

        //    //}

        //    return settings;
        //}




    }
}