using Aruna_DTN_Entegrasyon.ViewModels;
using NetOpenX.Rest.Client;
using NetOpenX.Rest.Client.BLL;
using NetOpenX.Rest.Client.Model;
using NetOpenX.Rest.Client.Model.Enums;
using NetOpenX.Rest.Client.Model.NetOpenX;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Aruna_DTN_Entegrasyon.Models
{
    public class Parametreler
    {
        public static string VTServer;
        public static string VTAdi;
        public static string VTKullanici;
        public static string VTParola;

        public static string MuhtelifCari;
        public static string NetUser;
        public static string NetPassword;
        public static int NetBranch;
        public static int NetDepoCode;
        public static int BlokeDepo;
        public static string RESTAPI;
        public static string FilePath;
    }
    public class BusinessLayer
    {
        Aruna.DataAccess.DataLayer MyDal;
        Aruna.Utilities.Utilities MyUtil;
        public DataTable dtParameters;


        public BusinessLayer()
        {
            #region Config Dosya Oku

            DataSet dsParameters = new DataSet();
            string strConfigFileName = string.Empty;

            try
            {
                strConfigFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Config.xml";
                Parametreler.FilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                dsParameters.ReadXml(strConfigFileName);
            }
            catch
            {
                strConfigFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "") + @"\Config.xml";
                Parametreler.FilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "");
                dsParameters.ReadXml(strConfigFileName);
            }


            Parametreler.VTServer = dsParameters.Tables[0].Rows[0]["DbServerName"].ToString();
            Parametreler.VTKullanici = dsParameters.Tables[0].Rows[0]["DbUserName"].ToString();
            Parametreler.VTParola = dsParameters.Tables[0].Rows[0]["DbUserPassword"].ToString();
            Parametreler.VTAdi = dsParameters.Tables[0].Rows[0]["DbName"].ToString();

            Parametreler.MuhtelifCari = dsParameters.Tables[1].Rows[0]["MuhtelifCari"].ToString();
            Parametreler.NetUser = dsParameters.Tables[1].Rows[0]["NetsisUserName"].ToString();
            Parametreler.NetPassword = dsParameters.Tables[1].Rows[0]["NetsisUserPassword"].ToString();
            Parametreler.NetBranch = Convert.ToInt32(dsParameters.Tables[1].Rows[0]["NetsisBranchCode"]);
            Parametreler.NetDepoCode = Convert.ToInt32(dsParameters.Tables[1].Rows[0]["NetsisDepoCode"]);
            Parametreler.BlokeDepo = Convert.ToInt32(dsParameters.Tables[1].Rows[0]["BlokeDepo"]);
            Parametreler.RESTAPI = dsParameters.Tables[1].Rows[0]["RESTAPI"].ToString();

            #endregion

            MyDal = new Aruna.DataAccess.DataLayer(Parametreler.VTServer, Parametreler.VTKullanici, Parametreler.VTParola, Parametreler.VTAdi);

            dtParameters = new DataTable();
            dtParameters.Columns.Add("ParameterName", typeof(string));
            dtParameters.Columns.Add("ParameterType", typeof(object));
            dtParameters.Columns.Add("ParameterValue", typeof(object));

            MyUtil = new Aruna.Utilities.Utilities();
        }

        public decimal StokPaydasiGetir(string stokKodu)
        {
            decimal result = Convert.ToDecimal(MyDal.ExecQueryScalar("select PAY_1*PAYDA_1 from TBLSTSABIT where STOK_KODU='" + stokKodu + "'", null));
            return result;
        }


        #region DELETE METHODLARI



        public void DeleteKullanici(string id)
        {
            MyDal.ExecQuery("DELETE FROM Arn_Tb_Kullanicilar WHERE UserName='" + id + "'", null);
        }

        public void DeleteFatKalem(string FatNo, string id)
        {
            dtParameters.Clear();
            DataRow drParam = dtParameters.NewRow();
            drParam["ParameterName"] = "SiparisNo";
            drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
            drParam["ParameterValue"] = FatNo;
            dtParameters.Rows.Add(drParam);

            drParam = dtParameters.NewRow();
            drParam["ParameterName"] = "SiraNo";
            drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
            drParam["ParameterValue"] = Convert.ToInt32(id);
            dtParameters.Rows.Add(drParam);

            MyDal.ExecQuery("EXEC ArnSp_Delete_SiparisKalem ?,?", dtParameters);
        }


        public void DeleteSaticiFatKalem(string FatNo, string id)
        {
            dtParameters.Clear();
            DataRow drParam = dtParameters.NewRow();
            drParam["ParameterName"] = "SiparisNo";
            drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
            drParam["ParameterValue"] = FatNo;
            dtParameters.Rows.Add(drParam);

            drParam = dtParameters.NewRow();
            drParam["ParameterName"] = "SiraNo";
            drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
            drParam["ParameterValue"] = Convert.ToInt32(id);
            dtParameters.Rows.Add(drParam);

            MyDal.ExecQuery("EXEC ArnSp_Delete_SaticiSiparisKalem ?,?", dtParameters);
        }


        public bool DeleteTeklif(string FatNo)
        {
            try
            {
                MyDal.ExecQuery("DELETE FROM Arn_Tb_TeklifKalemler WHERE FatNo = '" + FatNo + "'", null);
                MyDal.ExecQuery("DELETE FROM Arn_Tb_Cariler WHERE FatNo = '" + FatNo + "'", null);
                MyDal.ExecQuery("DELETE FROM Arn_Tb_Teklifler WHERE TeklifNo = '" + FatNo + "'", null);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public int DeleteGrup(string Grup)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Grup";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = Grup;
                dtParameters.Rows.Add(drParam);
                return MyDal.ExecQuery("DELETE Arn_Tb_GrupTanimlari WHERE Grup = ?", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public int DeleteRapor(string RaporAdi)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "RaporAdi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = RaporAdi;
                dtParameters.Rows.Add(drParam);
                return MyDal.ExecQuery("DELETE Arn_Tb_RaporTanimlari WHERE RaporAdi = ?", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion


        #region GET METHODLARI

        public DataTable GetRapor(string RaporAdi)
        {
            try
            {
                return MyDal.GetDataFromTable("SELECT * FROM " + RaporAdi, null);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<Cariler> GetCariler()
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Cariler", null);
                List<Cariler> cariler = new List<Cariler>();
                cariler = ConvertDataTable<Cariler>(dt);

                return cariler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public Cariler GetMuhtelifCari(string FatNo)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Tb_Cariler WHERE FatNo='" + FatNo + "'", null);
                Cariler cari = new Cariler();
                cari.CariKod = dt.Rows[0]["CariKod"].ToString();
                cari.CariIsim = dt.Rows[0]["CariAd"].ToString();
                cari.Adres = dt.Rows[0]["Adres"].ToString();
                cari.VergiDairesi = dt.Rows[0]["VergiDairesi"].ToString();
                cari.VergiNumarasi = dt.Rows[0]["VergiNo"].ToString();
                cari.TcNo = dt.Rows[0]["TCNo"].ToString();
                cari.EMail = dt.Rows[0]["EPosta"].ToString();
                cari.TeslimatAdresi = dt.Rows[0]["TeslimatAdresi"].ToString();
                cari.IlgiliKisi = dt.Rows[0]["KisiAdi"].ToString();

                return cari;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public Cariler GetCari(string CariKod)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Cariler WHERE CariKod='" + CariKod + "'", null);
                Cariler cari = new Cariler();

                if (dt.Rows.Count > 0)
                {
                    cari.CariKod = dt.Rows[0]["CariKod"].ToString();
                    cari.CariIsim = dt.Rows[0]["CariIsim"].ToString();
                    cari.Adres = dt.Rows[0]["Adres"].ToString();
                    cari.VergiDairesi = dt.Rows[0]["VergiDairesi"].ToString();
                    cari.VergiNumarasi = dt.Rows[0]["VergiNumarasi"].ToString();
                    cari.TcNo = dt.Rows[0]["TcNo"].ToString();
                }


                return cari;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public bool GetCariIfExist(string CariKod)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Cariler WHERE CariKod='" + CariKod + "'", null);
                Cariler cari = new Cariler();

                return dt.Rows.Count > 0 ? true : false;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public string GetCariPosta(string CariKod, string TeklifNo)
        {
            try
            {
                string Eposta = string.Empty;
                string sorgu = "SELECT * FROM Arn_Tb_Cariler WHERE CariKod = '" + CariKod + "' AND FatNo = '" + TeklifNo + "'";
                DataTable dt = MyDal.GetDataFromTable(sorgu, null);

                if (dt.Rows.Count > 0)
                {
                    Eposta = dt.Rows[0]["EPosta"].ToString();
                }
                else
                {
                    throw new Exception("Mail için Cari bulunmadı");
                }

                return Eposta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public string GetFatNo(string Controller, string Plasiyer)
        {
            DataTable dt;
            if (Controller == "Siparis")
            {
                dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM TBLSIPAMAS WHERE FATIRS_NO LIKE 'S" + Plasiyer + DateTime.Today.Year.ToString() + "%' AND FTIRSIP = '6' ORDER BY FATIRS_NO DESC ", null);
            }
            else if (Controller == "SatSiparis")
            {
                dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM TBLSIPAMAS WHERE FATIRS_NO LIKE 'S" + Plasiyer + DateTime.Today.Year.ToString() + "%' AND FTIRSIP = '7' ORDER BY FATIRS_NO DESC ", null);
            }
            else
            {
                dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM Arn_Tb_Teklifler WHERE TeklifNo LIKE 'T" + Plasiyer + DateTime.Today.Year.ToString() + "%' ORDER BY TeklifNo DESC ", null);

            }

            int index = 1;

            string header;

            if (Controller == "Teklif")
            {
                header = "T" + Plasiyer + DateTime.Today.Year.ToString();
            }
            else
            {
                header = "S" + Plasiyer + DateTime.Today.Year.ToString();
            }

            string No;
            if (dt.Rows.Count > 0)
            {
                if (Controller == "Teklif")
                {
                    No = dt.Rows[0]["TeklifNo"].ToString();
                }
                else
                {
                    No = dt.Rows[0]["FATIRS_NO"].ToString();
                }

                string substring = No.Substring(header.Length, No.Length - header.Length);
                index = Convert.ToInt32(substring.ToString()) + 1;
            }




            return header + index.ToString().PadLeft(15 - header.Length, '0');
        }

        public List<Kullanicilar> GetAllUsers()
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT KUL.*, PLASIYER_ACIKLAMA KullaniciAdi FROM Arn_Tb_Kullanicilar KUL INNER JOIN TBLCARIPLASIYER TP ON UserName = TP.PLASIYER_KODU", null);
                List<Kullanicilar> kullanicilar = new List<Kullanicilar>();
                kullanicilar = ConvertDataTable<Kullanicilar>(dt);

                return kullanicilar;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public Kullanicilar GetUsers(string UserId)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Tb_Kullanicilar WHERE UserName ='" + UserId + "'", null);
                List<Kullanicilar> kullanicilar = new List<Kullanicilar>();
                kullanicilar = ConvertDataTable<Kullanicilar>(dt);
                Kullanicilar Kullanici = kullanicilar[0];

                return Kullanici;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public Kullanicilar GetLogin(string UserName, string Password)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Tb_Kullanicilar WHERE UserName ='" + UserName + "' AND Password = '" + Password + "'", null);
                List<Kullanicilar> kullanicilar = new List<Kullanicilar>();
                kullanicilar = ConvertDataTable<Kullanicilar>(dt);
                Kullanicilar Kullanici;
                if (kullanicilar.Count > 0)
                {
                    Kullanici = kullanicilar[0];
                }
                else
                {
                    Kullanici = null;
                }


                return Kullanici;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<Siparisler> GetSiparisler()
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Sipamas", null);
                List<Siparisler> siparisler = new List<Siparisler>();
                siparisler = ConvertDataTable<Siparisler>(dt);

                return siparisler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public DateTime GetSiparisTarih(string FatNo)
        {
            try
            {
                var dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM TBLSIPAMAS WHERE FATIRS_NO ='" + FatNo + "'", null);
                return Convert.ToDateTime(dt.Rows[0]["TARIH"].ToString());
            }
            catch (Exception exc)
            {

                throw exc;
            }
        }

        public List<Siparisler> GetSaticiSiparisler()
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_SipamasSatici", null);
                List<Siparisler> siparisler = new List<Siparisler>();
                siparisler = ConvertDataTable<Siparisler>(dt);

                return siparisler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<SiparisKalemler> GetSiparisKalemler(string FatNo)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Sipatra WHERE FatNo ='" + FatNo + "'", null);
                List<SiparisKalemler> siparisKalemler = new List<SiparisKalemler>();
                siparisKalemler = ConvertDataTable<SiparisKalemler>(dt);

                return siparisKalemler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public List<SaticiSiparisKalemler> GetSaticiSiparisKalemler(string FatNo)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_SipatraSatici WHERE FatNo ='" + FatNo + "'", null);
                List<SaticiSiparisKalemler> siparisKalemler = new List<SaticiSiparisKalemler>();
                siparisKalemler = ConvertDataTable<SaticiSiparisKalemler>(dt);

                return siparisKalemler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<TeklifKalemler> GetTeklifKalemler(string FatNo)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Tekliftra_v2 WHERE FatNo ='" + FatNo + "'", null);
                List<TeklifKalemler> teklifKalemler = new List<TeklifKalemler>();
                teklifKalemler = ConvertDataTable<TeklifKalemler>(dt);

                return teklifKalemler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public decimal GetKDVOrani(string StokKodu)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT KDV_ORANI FROM TBLSTSABIT WHERE STOK_KODU = '" + StokKodu + "'", null);
                decimal Kdv = Convert.ToDecimal(dt.Rows[0]["KDV_ORANI"].ToString());
                return Kdv;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<Stoklar> GetStoklar(bool IsSatici = false)
        {
            try
            {

                //DataTable dt = MyDal.GetDataFromTable("SELECT STOK_KODU StokKodu,STOK_ADI StokAdi, TG.GRUP_ISIM GrupKodu,SATIS_FIAT1 Fiyat,PAYDA_1 KutuKati FROM TBLSTSABIT ST INNER JOIN TBLSTGRUP TG ON TG.GRUP_KOD = ST.GRUP_KODU ", null);

                //if (IsSatici)
                //{
                //    dt = MyDal.GetDataFromTable("SELECT STOK_KODU StokKodu,STOK_ADI StokAdi, TG.GRUP_ISIM GrupKodu,ALIS_FIAT1 Fiyat,PAYDA_1 KutuKati FROM TBLSTSABIT ST INNER JOIN TBLSTGRUP TG ON TG.GRUP_KOD = ST.GRUP_KODU ", null);
                //}

                DataTable dt = MyDal.GetDataFromTable("SELECT StokKodu, StokAd as StokAdi, Grup_Kodu As GrupKodu, SATIS_FIAT1 Fiyat, KutuKati, Toplam_Bakiye ToplamBakiye, DepoToplamBakiye, SaticiSiparisMiktar,TransitBakiye,Kdv FROM Arn_Vw_Bakiye ", null);
                //DataTable dt = MyDal.GetDataFromTable("SELECT StokKodu, StokAd as StokAdi, Grup_Kodu As GrupKodu, SATIS_FIAT1 Fiyat, KutuKati, Toplam_Bakiye ToplamBakiye FROM Arn_Vw_Bakiye ", null);

                if (IsSatici)
                {
                    dt = MyDal.GetDataFromTable("SELECT StokKodu, StokAd as StokAdi, Grup_Kodu As GrupKodu, ALIS_FIAT1 Fiyat, KutuKati, Toplam_Bakiye ToplamBakiye, DepoToplamBakiye, SaticiSiparisMiktar,TransitBakiye FROM Arn_Vw_Bakiye ", null);
                    //dt = MyDal.GetDataFromTable("SELECT StokKodu, StokAd as StokAdi, Grup_Kodu As GrupKodu, ALIS_FIAT1 Fiyat, KutuKati, Toplam_Bakiye ToplamBakiye FROM Arn_Vw_Bakiye ", null);
                }

                List<Stoklar> stoklar = new List<Stoklar>();
                stoklar = ConvertDataTable<Stoklar>(dt);

                return stoklar;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<TeklifViewModel> GetTeklifler()
        {
            try
            {

                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM Arn_Vw_Teklifmas_v2", dtParameters);
                List<TeklifViewModel> teklifler = new List<TeklifViewModel>();
                //teklifler = ConvertDataTable<TeklifViewModel>(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    TeklifViewModel teklif = new TeklifViewModel();

                    teklif.FatNo = dr["FatNo"].ToString();
                    teklif.Tarih = Convert.ToDateTime(dr["Tarih"].ToString());
                    teklif.GecerlilikTarihi = Convert.ToDateTime(dr["GecerlilikTarihi"].ToString());
                    teklif.CariKod = dr["CariKod"].ToString();
                    teklif.CariIsim = dr["CariIsim"].ToString();
                    teklif.Plasiyer = dr["Plasiyer"].ToString();
                    teklif.PlasiyerAd = dr["PlasiyerAd"].ToString();
                    teklif.SevkAdresi = dr["SevkAdresi"].ToString();
                    teklif.AciklamaSahasi = dr["AciklamaSahasi"].ToString();
                    teklifler.Add(teklif);
                }

                return teklifler;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public TeklifViewModel GetTeklif(string TeklifNo)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM Arn_Vw_Teklifmas_v2 WHERE FatNo = '" + TeklifNo + "'", null);
                //teklifler = ConvertDataTable<TeklifViewModel>(dt);
                TeklifViewModel teklif = new TeklifViewModel();

                teklif.FatNo = dt.Rows[0]["FatNo"].ToString();
                teklif.Tarih = Convert.ToDateTime(dt.Rows[0]["Tarih"].ToString());
                teklif.GecerlilikTarihi = Convert.ToDateTime(dt.Rows[0]["GecerlilikTarihi"].ToString());
                teklif.CariKod = dt.Rows[0]["CariKod"].ToString();
                teklif.CariIsim = dt.Rows[0]["CariIsim"].ToString();
                teklif.Plasiyer = dt.Rows[0]["Plasiyer"].ToString();
                teklif.PlasiyerAd = dt.Rows[0]["PlasiyerAd"].ToString();
                teklif.AciklamaSahasi = dt.Rows[0]["AciklamaSahasi"].ToString();
                return teklif;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public string GetBakiye(string StokKodu)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT Toplam_Bakiye FROM Arn_Vw_Bakiye WHERE StokKodu = '" + StokKodu + "'", null);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["Toplam_Bakiye"].ToString();
                }

                return string.Empty;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<SelectListItem> GetDepolar()
        {
            List<SelectListItem> depoList = new List<SelectListItem>();
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT DEPO_KODU DepoKodu, DEPO_ISMI DepoAdi  FROM TBLSTOKDP WHERE DEPO_KODU != " + Parametreler.BlokeDepo, null);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = dr["DepoKodu"].ToString();
                        item.Text = dr["DepoAdi"].ToString();
                        depoList.Add(item);
                    }
                }

                return depoList;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<SelectListItem> GetPlasiyer()
        {
            List<SelectListItem> plasiyerList = new List<SelectListItem>();
            try
            {
                DataTable dt = MyDal.GetDataFromStoredProcedure("ArnSp_GetPlasiyer", null);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = dr["PlasiyerKodu"].ToString();
                        item.Text = dr["PlasiyerAdi"].ToString();
                        plasiyerList.Add(item);
                    }
                }

                return plasiyerList;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<YaslandirmaViewModel> GetYaslandirma(string StokKodu)
        {
            try
            {
                List<SelectListItem> depolar = GetDepolar();
                foreach (SelectListItem item in depolar)
                {
                    MyDal.ExecQuery("EXEC dbo.[_ArnSp_StokYaslandirmeListesi_Proje] '" + StokKodu + "', '" + item.Value + "'", null);
                }

                DataTable dt = MyDal.GetDataFromTable("SELECT SIRA Sira, TUR D_S, STOK_KODU StokKodu,STHAR_TARIH Tarih,STHAR_GCMIK Miktar, STHAR_NF Fiyat,DEPO_KODU DepoKodu FROM _Arn_TbStokYaslandirma_Proje where stok_kodu='" + StokKodu + "'", null);
                //DataTable dt = MyDal.GetDataFromTable("SELECT STOK_KODU StokKodu,STHAR_TARIH Tarih,STHAR_GCMIK Miktar, STHAR_NF Fiyat,DEPO_KODU DepoKodu FROM _Arn_TbStokYaslandirma_Proje where stok_kodu='" + StokKodu + "'", null);
                List<YaslandirmaViewModel> yaslandirmalar = new List<YaslandirmaViewModel>();
                yaslandirmalar = ConvertDataTable<YaslandirmaViewModel>(dt);

                return yaslandirmalar;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public DataTable GetGrupKodlari()
        {

            return MyDal.GetDataFromTable("SELECT GRUP_ISIM GrupAdi FROM TBLSTGRUP WHERE GRUP_ISIM  NOT IN (SELECT Grup FROM Arn_Tb_GrupTanimlari)", null);
        }

        public DataTable GetTanimliGrupKodlari()
        {

            return MyDal.GetDataFromTable("SELECT * FROM Arn_Tb_GrupTanimlari", null);
        }

        public DataTable GetTanimliRaporlar()
        {

            return MyDal.GetDataFromTable("SELECT * FROM Arn_Tb_RaporTanimlari", null);
        }


        public bool GrupCheck(string GrupKodu)
        {
            try
            {

                DataTable dt = MyDal.GetDataFromTable("SELECT IsChecked FROM Arn_Tb_GrupTanimlari where IsChecked=1 and   Grup='" + GrupKodu + "'", null);

                return dt.Rows.Count > 0;

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<BlokeCozViewModel> GetBlokeler()
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT * FROM ArnVw_Blokeli_Siparisler", null);

                List<BlokeCozViewModel> blokeCozList = new List<BlokeCozViewModel>();
                blokeCozList = ConvertDataTable<BlokeCozViewModel>(dt);

                int i = 0;
                foreach (BlokeCozViewModel bloke in blokeCozList)
                {
                    blokeCozList[i].ID = i;
                    i++;
                }
                return blokeCozList;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public decimal GetBlokeliMiktar(string FisNo, int id)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "SiparisNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = FisNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Sira";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                drParam["ParameterValue"] = id;
                dtParameters.Rows.Add(drParam);

                DataTable dt = MyDal.GetDataFromTable("SELECT Miktar FROM ArnVw_Blokeli_Siparisler WHERE SiparisNo = ? AND Sira = ?", dtParameters);

                if (dt.Rows.Count > 0)
                {
                    return Convert.ToDecimal(dt.Rows[0]["Miktar"].ToString());
                }
                else
                {
                    return 0;
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }



        #endregion


        #region INSERT & UPDATE METHODLARI


        public void InsertKalemSQL(string FatNo, DateTime Tarih, int Tip, string StokKodu, int DepoKodu, decimal Miktar, decimal MaliyetFiyat, decimal Fiyat, decimal IskontoOran)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "StokKodu";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = StokKodu;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "FisNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = FatNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Miktar";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = Miktar;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Tarih";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Date;
                drParam["ParameterValue"] = Tarih;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "MaliyetFiyat";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = MaliyetFiyat;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Fiyat";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = Fiyat;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "DepoKodu";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                drParam["ParameterValue"] = DepoKodu;
                dtParameters.Rows.Add(drParam);


                if (Tip == 0)
                {
                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "BlokeDepoKodu";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                    drParam["ParameterValue"] = Parametreler.BlokeDepo;
                    dtParameters.Rows.Add(drParam);
                }


                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Iskonto";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = IskontoOran;
                dtParameters.Rows.Add(drParam);

                if (Tip == 0)
                {
                    MyDal.ExecQuery("EXEC ArnSp_Insert_BlokeEkle ?,?,?,?,?,?,?,?,?", dtParameters);
                }
                else if (Tip == 2)
                {
                    MyDal.ExecQuery("EXEC ArnSp_Insert_OnFatura ?,?,?,?,?,?,?,?", dtParameters);
                }
                else
                {
                    MyDal.ExecQuery("EXEC ArnSp_Insert_Transit ?,?,?,?,?,?,?,?", dtParameters);
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        public void InsertKalemSQL_Satici(string FatNo, DateTime Tarih, string StokKodu, int DepoKodu, decimal Miktar, decimal Fiyat)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "StokKodu";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = StokKodu;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "FisNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = FatNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Miktar";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = Miktar;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Tarih";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Date;
                drParam["ParameterValue"] = Tarih;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Fiyat";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = Fiyat;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "DepoKodu";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                drParam["ParameterValue"] = DepoKodu;
                dtParameters.Rows.Add(drParam);

                MyDal.ExecQuery("EXEC ArnSp_Insert_SaticiSipatra ?,?,?,?,?,?", dtParameters);


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public bool InsertSevkEmri(List<BlokeCozViewModel> Blokeliler)
        {
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM TBLSEVKMAS WHERE BELGENO LIKE 'A" + DateTime.Today.Year.ToString() + "%' ORDER BY TARIH DESC,BELGENO DESC ", null);

                int index = 1;

                string header = "A" + DateTime.Today.Year.ToString();


                string No;
                if (dt.Rows.Count > 0)
                {
                    No = dt.Rows[0]["BELGENO"].ToString();


                    string substring = No.Substring(header.Length, No.Length - header.Length);
                    index = Convert.ToInt32(substring.ToString()) + 1;
                }




                string FatNo = header + index.ToString().PadLeft(15 - header.Length, '0');

                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "SubeKodu";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                drParam["ParameterValue"] = Parametreler.NetBranch;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "BelgeNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = FatNo;
                dtParameters.Rows.Add(drParam);



                MyDal.ExecQuery("ArnSp_Insert_Sevk ?,?", dtParameters);

                foreach (BlokeCozViewModel bloke in Blokeliler)
                {
                    dtParameters.Clear();
                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "SubeKodu";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                    drParam["ParameterValue"] = Parametreler.NetBranch;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "BelgeNo";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = FatNo;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "SipNo";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = bloke.SiparisNo;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "SipSira";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                    drParam["ParameterValue"] = bloke.Sira;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "CariKod";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = bloke.CariKodu;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "Sira";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = bloke.ID;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "Miktar";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = bloke.BlokeliMiktar;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "StokKodu";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = bloke.StokKodu;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "DepoKodu";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                    drParam["ParameterValue"] = Parametreler.BlokeDepo;
                    dtParameters.Rows.Add(drParam);

                    MyDal.ExecQuery("ArnSp_Insert_SevkTra ?,?,?,?,?,?,?,?,?", dtParameters);
                }


                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        public bool InsertKullanici(Kullanicilar kullanici)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "UserName";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = kullanici.UserName;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Password";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = kullanici.Password;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "IskontoOran";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                drParam["ParameterValue"] = kullanici.IskontoOran;
                dtParameters.Rows.Add(drParam);

                MyDal.ExecQuery("ArnSp_Insert_Kullanici ?,?,?", dtParameters);

                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }



        public bool InsertTeklif(Teklifler teklif, List<KalemViewModel> kalemler)
        {

            try
            {

                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "TeklifNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.FatNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Tarih";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Date;
                drParam["ParameterValue"] = teklif.Tarih;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "GecerlilikTarihi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Date;
                drParam["ParameterValue"] = teklif.GecerlilikTarihi;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "PlasiyerKodu";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Plasiyer;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "AciklamaSahasi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.AciklamaSahasi;
                dtParameters.Rows.Add(drParam);

                int resultTeklif = MyDal.ExecQuery("ArnSp_Insert_Teklifler ?,?,?,?,?", dtParameters);

                if (resultTeklif != 1)
                {
                    throw new Exception("Tekliflerin Üstü Eklenirken hata oluştu");
                }

                dtParameters.Clear();
                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "FatNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.FatNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "CariKod";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.CariKod;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "CariAd";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.CariIsim;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Adres";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.Adres;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "VergiDairesi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.VergiDairesi;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "VergiNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.VergiNumarasi;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "TCNo";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.TcNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "EPosta";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.EMail;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "TeslimatAdresi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.TeslimatAdresi;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "KisiAdi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = teklif.Cari.IlgiliKisi;
                dtParameters.Rows.Add(drParam);


                int resultCari = MyDal.ExecQuery("ArnSp_Insert_Cariler ?,?,?,?,?,?,?,?,?,?", dtParameters);

                foreach (KalemViewModel kalem in kalemler)
                {
                    dtParameters.Clear();
                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "FatNo";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = teklif.FatNo;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "Sira";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Integer;
                    drParam["ParameterValue"] = kalem.Sira;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "StokKodu";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = kalem.StokKodu;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "StokIsim";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = kalem.StokIsim;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "DepoKodu";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = kalem.DepoKodu;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "DepoAdi";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                    drParam["ParameterValue"] = kalem.DepoAdi;
                    dtParameters.Rows.Add(drParam);


                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "Miktar";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.Miktar;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "Fiyat";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.Fiyat;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "MaliyetFiyat";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.MaliyetFiyat;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "ToplamTutar";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.ToplamTutar;
                    dtParameters.Rows.Add(drParam);


                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "ToplamMaliyetTutar";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.ToplamMaliyetTutar;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "IskontoOran";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.IskontoOran;
                    dtParameters.Rows.Add(drParam);

                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "IskontoluTutar";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.IskontoluTutar;
                    dtParameters.Rows.Add(drParam);


                    drParam = dtParameters.NewRow();
                    drParam["ParameterName"] = "Bakiye";
                    drParam["ParameterType"] = System.Data.OleDb.OleDbType.Decimal;
                    drParam["ParameterValue"] = kalem.Bakiye;
                    dtParameters.Rows.Add(drParam);


                    int resultKalem = MyDal.ExecQuery("ArnSp_Insert_TeklifKalemler ?,?,?,?,?,?,?,?,?,?,?,?,?,?", dtParameters);
                }


                return true;

            }
            catch (Exception exc)
            {
                throw exc;
            }


        }


        public bool UpdateTeklif(Teklifler teklif, List<KalemViewModel> kalemler)
        {
            try
            {
                bool res = DeleteTeklif(teklif.FatNo);
                bool result = InsertTeklif(teklif, kalemler);
                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }



        public int InsertGrup(string Grup, bool IsChecked)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Grup";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = Grup;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "IsChecked";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Boolean;
                drParam["ParameterValue"] = IsChecked;
                dtParameters.Rows.Add(drParam);
                return MyDal.ExecQuery("INSERT INTO [dbo].[Arn_Tb_GrupTanimlari] (Grup, IsChecked) VALUES (?, ?)", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public int UpdateGrup(string Grup, bool IsChecked)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "IsChecked";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.Boolean;
                drParam["ParameterValue"] = IsChecked;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "Grup";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = Grup;
                dtParameters.Rows.Add(drParam);
                return MyDal.ExecQuery("UPDATE Arn_Tb_GrupTanimlari SET IsChecked = ? WHERE Grup = ?", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public int UpdateHareketTur(string FatNo, string id, bool IsSatici)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "FISNO";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = FatNo;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "SIRA";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = id;
                dtParameters.Rows.Add(drParam);


                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "STHAR_FTIRSIP";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                if (IsSatici)
                {
                    drParam["ParameterValue"] = '7';
                }
                else
                {
                    drParam["ParameterValue"] = '6';
                }

                dtParameters.Rows.Add(drParam);

                return MyDal.ExecQuery("UPDATE TBLSIPATRA SET STHAR_HTUR = 'K' WHERE FISNO = ? AND SIRA = ? AND STHAR_FTIRSIP = ?", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public int InsertRapor(string RaporAdi, string ViewAdi)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "RaporAdi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = RaporAdi;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "ViewAdi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = ViewAdi;
                dtParameters.Rows.Add(drParam);
                return MyDal.ExecQuery("INSERT INTO [dbo].[Arn_Tb_RaporTanimlari] (RaporAdi, ViewAdi) VALUES (?, ?)", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public int UpdateRapor(string RaporAdi, string ViewAdi)
        {
            try
            {
                dtParameters.Clear();
                DataRow drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "ViewAdi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = ViewAdi;
                dtParameters.Rows.Add(drParam);

                drParam = dtParameters.NewRow();
                drParam["ParameterName"] = "RaporAdi";
                drParam["ParameterType"] = System.Data.OleDb.OleDbType.VarChar;
                drParam["ParameterValue"] = RaporAdi;
                dtParameters.Rows.Add(drParam);
                return MyDal.ExecQuery("UPDATE Arn_Tb_RaporTanimlari SET ViewAdi = ? WHERE RaporAdi = ?", dtParameters);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        #endregion


        #region REST METHODLARI

        public TResult<ItemSlips> MusteriSiparis(Siparisler siparis, List<KalemViewModel> kalemler)
        {
            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"

            });

            if (string.IsNullOrEmpty(siparis.AciklamaSahasi))
            {
                siparis.AciklamaSahasi = " ";

            }

            if (siparis.AciklamaSahasi.Length < 300)
            {
                siparis.AciklamaSahasi = siparis.AciklamaSahasi.PadRight(300, ' ');
            }

            if (string.IsNullOrEmpty(siparis.Cari.EMail))
            {
                siparis.Cari.EMail = " ";
            }

            if (string.IsNullOrEmpty(siparis.Cari.IlgiliKisi))
            {
                siparis.Cari.IlgiliKisi = " ";
            }
            if (string.IsNullOrEmpty(siparis.Cari.TeslimatAdresi))
            {
                siparis.Cari.TeslimatAdresi = " ";
            }
            if (siparis.Cari.TeslimatAdresi.Length < 200)
            {
                siparis.Cari.TeslimatAdresi = siparis.Cari.TeslimatAdresi.PadRight(200, ' ');
            }

            if (restApioAuth.oAuth2loginResult.IsSuccessStatusCode)
            {
                ItemSlipsManager _manager = new ItemSlipsManager(restApioAuth);
                ItemSlips fatura = new ItemSlips()
                {
                    OtoVadeGunGetir = false,
                    FaturaTip = JTFaturaTip.ftSSip,
                    FatUst = new ItemSlipsHeader()
                    {
                        FATIRS_NO = siparis.FatNo,
                        SIPARIS_TEST = siparis.Tarih,
                        CariKod = siparis.Cari.CariKod,
                        Tarih = siparis.Tarih,
                        ODEMEGUNU = siparis.OdemeGun,
                        ODEMETARIHI = siparis.Tarih.AddDays(siparis.OdemeGun),

                        FiiliTarih = siparis.Tarih,
                        //ODEMETARIHI = DateTime.Now,
                        PLA_KODU = siparis.Plasiyer,
                        Proje_Kodu = "1",
                        EKACK1 = siparis.AciklamaSahasi.Substring(0, 100),
                        EKACK2 = siparis.AciklamaSahasi.Substring(100, 100),
                        EKACK3 = siparis.AciklamaSahasi.Substring(200, 100),
                        EKACK4 = siparis.Cari.EMail,
                        EKACK5 = siparis.Cari.IlgiliKisi,
                        EKACK6 = siparis.Cari.TeslimatAdresi.Substring(0, 100),
                        EKACK7 = siparis.Cari.TeslimatAdresi.Substring(100, 100),
                        EKACK8 = Enum.GetName(typeof(SipTip), siparis.SipTip),
                        EKACK9 = siparis.TeklifNo,
                        KDV_DAHILMI = false


                    },
                };
                fatura.Kalems = new List<ItemSlipLines>();

                foreach (KalemViewModel kalemView in kalemler)
                {

                    fatura.Kalems.Add(new ItemSlipLines
                    {
                        StokKodu = kalemView.StokKodu,
                        STra_GCMIK = Convert.ToDouble(kalemView.Miktar),
                        DEPO_KODU = Convert.ToInt32(kalemView.DepoKodu),
                        STra_NF = Convert.ToDouble(kalemView.Fiyat),
                        STra_BF = Convert.ToDouble(kalemView.Fiyat),
                        STra_SatIsk = Convert.ToDouble(kalemView.IskontoOran),
                        //SatirBaziAciks = strings,                   
                        //Ekalan = siparis.FatNo + kalemView.Sira.ToString(),
                        Ekalan1 = kalemView.Tip == FatType.Bloke ? "E" : "H",
                        Ekalan = kalemView.MaliyetFiyat.ToString(),
                        ProjeKodu = "1" //uu,15.06.2021

                    });

                    //fatura.Kalems[fatura.Kalems.Count - 1].SatirBaziAciks = new List<string>();
                    //fatura.Kalems[fatura.Kalems.Count - 1].SatirBaziAciks.Add(kalemView.MaliyetFiyat.ToString());


                }//kalemler dongu sonu
                var result = _manager.PostInternal(fatura);



                return result;
            }
            else
            {
                TResult<ItemSlips> res = new TResult<ItemSlips>();
                res.IsSuccessful = false;

                res.ErrorDesc = restApioAuth.oAuth2loginResult.error_description;

                return res;
                //throw new Exception("REST API'ye bağlanırken hata oluştu." + Environment.NewLine + restApioAuth.oAuth2loginResult.error + Environment.NewLine + restApioAuth.oAuth2loginResult.error_description);
            }



        }

        public void DeleteFat(string fatNo, string Tip, string CariKod)
        {
            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"

            });

            ItemSlipsManager _manager = new ItemSlipsManager(restApioAuth);


            var Result = _manager.DeleteInternalById(Tip + ";" + fatNo + ";" + CariKod);
        }


        public TResult<ItemSlips> MusteriSiparisDAT(Siparisler siparis, List<KalemViewModel> kalemler)
        {
            var blokes = kalemler.Where(t => t.Tip == FatType.Bloke);


            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"

            });
            if (restApioAuth.oAuth2loginResult.IsSuccessStatusCode)
            {
                ItemSlipsManager _manager = new ItemSlipsManager(restApioAuth);
                ItemSlips fatura = new ItemSlips()
                {
                    FaturaTip = JTFaturaTip.ftLokalDepo,
                    FatUst = new ItemSlipsHeader()
                    {
                        FATIRS_NO = siparis.FatNo,
                        //AMBHARTUR = JTAmbarHarTur.htUretim,
                        //CikisYeri = JTCikisYeri.cyMasrafMer,
                        CariKod = siparis.Cari.CariKod,
                        Tarih = siparis.Tarih,
                        ODEMEGUNU = siparis.OdemeGun,
                        FiiliTarih = DateTime.Now,
                        //ODEMETARIHI = DateTime.Now,
                        PLA_KODU = siparis.Plasiyer,
                        TIPI = JTFaturaTipi.ft_Bos,
                        Proje_Kodu = "1",
                        KDV_DAHILMI = false


                    },
                };
                fatura.Kalems = new List<ItemSlipLines>();
                foreach (KalemViewModel kalemView in blokes)
                {
                    fatura.Kalems.Add(new ItemSlipLines
                    {
                        StokKodu = kalemView.StokKodu,
                        STra_GCMIK = Convert.ToDouble(kalemView.Miktar),
                        DEPO_KODU = Convert.ToInt32(kalemView.DepoKodu),
                        STra_NF = Convert.ToDouble(kalemView.Fiyat),
                        STra_BF = Convert.ToDouble(kalemView.MaliyetFiyat),
                        Gir_Depo_Kodu = Parametreler.BlokeDepo,
                        Olcubr = 1,
                        Ekalan = siparis.FatNo + kalemView.Sira.ToString(),
                    });
                }
                var result = _manager.PostInternal(fatura);



                return result;
            }
            else
            {
                TResult<ItemSlips> res = new TResult<ItemSlips>();
                res.IsSuccessful = false;

                res.ErrorDesc = restApioAuth.oAuth2loginResult.error_description;

                return res;
                //throw new Exception("REST API'ye bağlanırken hata oluştu." + Environment.NewLine + restApioAuth.oAuth2loginResult.error + Environment.NewLine + restApioAuth.oAuth2loginResult.error_description);
            }
        }

        public TResult<ItemSlips> DATKapatma(List<SiparisKalemler> kalemler, decimal miktar)
        {

            string FisNo = string.Empty;
            int DepoKodu;
            try
            {
                DataTable dt = MyDal.GetDataFromTable("SELECT TOP 1 * FROM TBLSTHAR WHERE FISNO LIKE 'L00%' ORDER BY FISNO DESC", null);

                if (dt.Rows.Count > 0)
                {
                    FisNo = dt.Rows[0]["FISNO"].ToString();
                    FisNo = 'L' + (Convert.ToInt32(FisNo.Substring(1, 14)) + 1).ToString().PadLeft(14, '0');
                }
                else
                {
                    FisNo = 'L' + (dt.Rows.Count + 1).ToString().PadLeft(14, '0');
                }

                DataTable dtDepo = MyDal.GetDataFromTable("SELECT DEPO_KODU FROM TBLSIPATRA WHERE FISNO = '" + kalemler[0].FatNo + "' AND SIRA = " + kalemler[0].Sira + " AND STHAR_FTIRSIP = '6'", null);

                if (dtDepo.Rows.Count > 0)
                {
                    DepoKodu = Convert.ToInt32(dtDepo.Rows[0]["DEPO_KODU"].ToString());
                }
                else
                {
                    throw new Exception("DAT Kapatma yapılırken kalem bulunamadı.");
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }


            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"

            });
            if (restApioAuth.oAuth2loginResult.IsSuccessStatusCode)
            {
                ItemSlipsManager _manager = new ItemSlipsManager(restApioAuth);
                ItemSlips fatura = new ItemSlips()
                {
                    FaturaTip = JTFaturaTip.ftLokalDepo,
                    FatUst = new ItemSlipsHeader()
                    {

                        FATIRS_NO = FisNo,
                        //AMBHARTUR = JTAmbarHarTur.htUretim,
                        //CikisYeri = JTCikisYeri.cyMasrafMer,
                        CariKod = kalemler[0].Cari_Kod,
                        Tarih = DateTime.Now.Date,
                        //ODEMEGUNU = siparis.OdemeGun,
                        FiiliTarih = DateTime.Now,
                        //ODEMETARIHI = DateTime.Now,
                        //PLA_KODU = siparis.Plasiyer,
                        TIPI = JTFaturaTipi.ft_Bos,
                        Proje_Kodu = "1",
                        EKACK10 = kalemler[0].FatNo,
                        KDV_DAHILMI = false,


                    },
                };
                fatura.Kalems = new List<ItemSlipLines>();
                foreach (SiparisKalemler kalemView in kalemler)
                {
                    fatura.Kalems.Add(new ItemSlipLines
                    {
                        StokKodu = kalemView.StokKodu,
                        STra_GCMIK = Convert.ToDouble(miktar),
                        DEPO_KODU = Convert.ToInt32(Parametreler.BlokeDepo),
                        Gir_Depo_Kodu = DepoKodu,
                        Olcubr = 1,
                        Ekalan = kalemView.FatNo + kalemView.Sira.ToString()

                    });
                }
                var result = _manager.PostInternal(fatura);



                return result;
            }
            else
            {
                TResult<ItemSlips> res = new TResult<ItemSlips>();
                res.IsSuccessful = false;

                res.ErrorDesc = restApioAuth.oAuth2loginResult.error_description;

                return res;
                //throw new Exception("REST API'ye bağlanırken hata oluştu." + Environment.NewLine + restApioAuth.oAuth2loginResult.error + Environment.NewLine + restApioAuth.oAuth2loginResult.error_description);
            }
        }

        public TResult<ItemSlips> MusteriSiparisAmbar(Siparisler siparis, List<KalemViewModel> kalemler)
        {
            var ambars = kalemler.Where(t => t.Tip == FatType.OnFatura);


            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"

            });
            if (restApioAuth.oAuth2loginResult.IsSuccessStatusCode)
            {
                ItemSlipsManager _manager = new ItemSlipsManager(restApioAuth);
                ItemSlips fatura = new ItemSlips()
                {
                    FaturaTip = JTFaturaTip.ftAmbarG,
                    FatUst = new ItemSlipsHeader()
                    {
                        FATIRS_NO = siparis.FatNo,
                        AMBHARTUR = JTAmbarHarTur.htDepolar,
                        CikisYeri = JTCikisYeri.cySerbest,
                        Sube_Kodu = Parametreler.NetBranch,
                        CariKod = siparis.Cari.CariKod,
                        Tarih = siparis.Tarih,
                        FiiliTarih = DateTime.Now,
                        TIPI = JTFaturaTipi.ft_Bos,
                        //ODEMETARIHI = DateTime.Now,
                        PLA_KODU = siparis.Plasiyer,
                        Proje_Kodu = "1",
                        KDV_DAHILMI = false


                    },
                };
                fatura.Kalems = new List<ItemSlipLines>();
                foreach (KalemViewModel kalemView in ambars)
                {
                    fatura.Kalems.Add(new ItemSlipLines
                    {
                        StokKodu = kalemView.StokKodu,
                        STra_GCMIK = Convert.ToDouble(kalemView.Miktar),
                        DEPO_KODU = Convert.ToInt32(kalemView.DepoKodu),
                        STra_NF = Convert.ToDouble(kalemView.Fiyat),
                        Ekalan = siparis.FatNo + kalemView.Sira.ToString()
                    });
                }
                var result = _manager.PostInternal(fatura);



                return result;
            }
            else
            {
                TResult<ItemSlips> res = new TResult<ItemSlips>();
                res.IsSuccessful = false;

                res.ErrorDesc = restApioAuth.oAuth2loginResult.error_description;

                return res;
                //throw new Exception("REST API'ye bağlanırken hata oluştu." + Environment.NewLine + restApioAuth.oAuth2loginResult.error + Environment.NewLine + restApioAuth.oAuth2loginResult.error_description);
            }
        }

        public TResult<ARPs> CariKart(Cariler cari)
        {
            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"
            });

            if (restApioAuth.oAuth2loginResult.IsSuccessStatusCode)
            {
                var _ARPsManager = new ARPsManager(restApioAuth);
                var _dummyArps2 = new ARPs()
                {
                    CariTemelBilgi = new ARPsPrimInfo()
                    {
                        CARI_KOD = cari.CariKod,
                        CARI_ISIM = cari.CariIsim,
                        CARI_TIP = "S",
                        Sube_Kodu = 1,
                        VERGI_DAIRESI = cari.VergiDairesi,
                        VERGI_NUMARASI = cari.VergiNumarasi,
                        EMAIL = cari.EMail,
                        CARI_ADRES = cari.Adres,
                        CM_RAP_TARIH = DateTime.Now.Date
                    },

                    CariEkBilgi = new ARPsSuppInfo()
                    {
                        CARI_KOD = cari.CariKod,

                        TcKimlikNo = cari.TcNo,
                    },
                    IsletmelerdeOrtak = true,
                    SubelerdeOrtak = true
                };

                //_ARPsManager.DeleteInternalById(_dummyArps2.CariTemelBilgi.CARI_KOD);
                var result = _ARPsManager.PostInternal(_dummyArps2);
                //var subeOrtak = result.Data.SubelerdeOrtak.GetValueOrDefault();
                return result;
            }
            else
            {
                TResult<ARPs> res = new TResult<ARPs>();
                res.IsSuccessful = false;
                res.ErrorDesc = "Rest Api'ye bağlanırken hata oluştu";

                return res;
            }

        }

        public TResult<ItemSlips> SaticiSiparis(Siparisler siparis, List<KalemViewModel> kalemler)
        {
            oAuth2 restApioAuth = new oAuth2(Parametreler.RESTAPI);

            restApioAuth.Login(new JLogin()
            {
                BranchCode = Parametreler.NetBranch,
                NetsisUser = Parametreler.NetUser,
                NetsisPassword = Parametreler.NetPassword,
                DbType = JNVTTipi.vtMSSQL,
                DbName = Parametreler.VTAdi,
                DbPassword = "",
                DbUser = "TEMELSET"
            });

            if (string.IsNullOrEmpty(siparis.AciklamaSahasi))
            {
                siparis.AciklamaSahasi = " ";

            }

            if (siparis.AciklamaSahasi.Length < 300)
            {
                siparis.AciklamaSahasi = siparis.AciklamaSahasi.PadRight(300, ' ');
            }

            if (string.IsNullOrEmpty(siparis.Cari.EMail))
            {
                siparis.Cari.EMail = " ";
            }
            if (string.IsNullOrEmpty(siparis.Cari.IlgiliKisi))
            {
                siparis.Cari.IlgiliKisi = " ";
            }
            if (string.IsNullOrEmpty(siparis.Cari.TeslimatAdresi))
            {
                siparis.Cari.TeslimatAdresi = " ";
            }
            if (siparis.Cari.TeslimatAdresi.Length < 200)
            {
                siparis.Cari.TeslimatAdresi = siparis.Cari.TeslimatAdresi.PadRight(200, ' ');
            }

            if (restApioAuth.oAuth2loginResult.IsSuccessStatusCode)
            {
                ItemSlipsManager _manager = new ItemSlipsManager(restApioAuth);
                ItemSlips fatura = new ItemSlips()
                {
                    OtoVadeGunGetir = false,
                    FaturaTip = JTFaturaTip.ftASip,
                    FatUst = new ItemSlipsHeader()
                    {
                        FATIRS_NO = siparis.FatNo,
                        //AMBHARTUR = JTAmbarHarTur.htUretim,
                        //CikisYeri = JTCikisYeri.cyMasrafMer,
                        // FAT UST TESTAR
                        SIPARIS_TEST = DateTime.Now,
                        CariKod = siparis.Cari.CariKod,
                        Tarih = siparis.Tarih,
                        ODEMEGUNU = siparis.OdemeGun,
                        ODEMETARIHI = DateTime.Now.AddDays(siparis.OdemeGun),
                        FiiliTarih = DateTime.Now,
                        //ODEMETARIHI = DateTime.Now,
                        PLA_KODU = siparis.Plasiyer,
                        Proje_Kodu = "1",
                        EKACK1 = siparis.AciklamaSahasi.Substring(0, 100),
                        EKACK2 = siparis.AciklamaSahasi.Substring(100, 100),
                        EKACK3 = siparis.AciklamaSahasi.Substring(200, 100),
                        EKACK4 = siparis.Cari.EMail,
                        EKACK5 = siparis.Cari.IlgiliKisi,
                        EKACK6 = siparis.Cari.TeslimatAdresi.Substring(0, 100),
                        EKACK7 = siparis.Cari.TeslimatAdresi.Substring(100, 100),
                        KDV_DAHILMI = false



                    },
                };
                fatura.Kalems = new List<ItemSlipLines>();
                foreach (KalemViewModel kalemView in kalemler)
                {
                    fatura.Kalems.Add(new ItemSlipLines
                    {
                        StokKodu = kalemView.StokKodu,
                        STra_GCMIK = Convert.ToDouble(kalemView.Miktar),
                        DEPO_KODU = Convert.ToInt32(kalemView.DepoKodu),
                        STra_NF = Convert.ToDouble(kalemView.Fiyat),
                        STra_BF = Convert.ToDouble(kalemView.Fiyat)
                        //KalemGenIskOran1 = Convert.ToDouble(kalemView.IskontoOran)
                    });
                }
                var result = _manager.PostInternal(fatura);
                return result;
            }
            else
            {
                TResult<ItemSlips> res = new TResult<ItemSlips>();
                res.IsSuccessful = false;

                res.ErrorDesc = restApioAuth.oAuth2loginResult.error_description;

                return res;
                //throw new Exception("REST API'ye bağlanırken hata oluştu." + Environment.NewLine + restApioAuth.oAuth2loginResult.error + Environment.NewLine + restApioAuth.oAuth2loginResult.error_description);
            }
        }



        #endregion


        #region Dinamic DataTable to List<T>

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {

                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        if (dr[column.ColumnName].GetType() != typeof(DBNull))
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        else
                            continue;
                }
            }
            return obj;
        }




        #endregion
    }
}