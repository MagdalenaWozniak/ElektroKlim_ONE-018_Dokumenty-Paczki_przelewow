using cdn_api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Przelewy.BO;
using static Przelewy.pacz;

namespace Przelewy
{
    public partial class Paczki : Form
    {
        string connection_string = "";
        List<ListaDok> doki;
        private static int sesja = 0;
        private static int XLWersjaAPI = 20220;
        private static int XLTrybWsadowy = 1;
        private static int XLWinieta = -1;
        private static int XLUtworzWlasnaSesje = 1;
        private static string XLSQLServer = "172.51.1.8";
        private static string XLSQLDatabase = "ElektroKlim";//"ERPXL_PoloMarket"
        private static string XLUser = "ADMIN"; //
        private static string XLPass = "NE!w0rk12";//"Dupa2003!";
        private static int dokID = 0;
        List<ListaPaczek> rodzajePaczek = new List<ListaPaczek>();

        public Paczki(string connectionString, string baza, List<ListaDok> dokumenty)
        {
            try
            {
                InitializeComponent();
                if (dokumenty.Count==0)
                {
                    //MessageBox.Show("dokumenty count: " + dokumenty.Count, "Komunikat",
                      //     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    label2.Enabled = true;
                    label3.Enabled = true;
                    barcode.Enabled = true;
                    checkKaucja.Enabled = true;
                    checkPlat.Enabled = true;
                }


                connection_string = connectionString;
                XLSQLDatabase = baza;
                doki = dokumenty;
               // MessageBox.Show("Dok initialize: " +doki[0].GIDNumer, "Komunikat",
                 //         MessageBoxButtons.OK, MessageBoxIcon.Information);
                string paczkiQuery = "SELECT  [PPN_ID], 'PAP-'+SUBSTRING(CAST(PPN_ROK as varchar), 3,2)+'/'+PPN_Rejestr+'/'+CAST(PPN_Miesiac as varchar)+'/'+CAST(PPN_Numer as varchar) , CAST(DateAdd(day, PPN_DataRealizacji  - 4, '1801-01-01') as date ), PPN_Rejestr, KAR_Waluta FROM [CDN].[PrzelewyNag] join cdn.Rejestry on PPN_Rejestr=Kar_Seria WHERE PPN_Status = 0 ORDER BY PPN_DataRealizacji";
                rodzajePaczek = SelectListRowColumn(paczkiQuery, "cdn.PrzelewyNag");
                string[] rodzaje;
                rodzaje = new string[rodzajePaczek.Count];
                int x = 0;
                foreach (ListaPaczek box in rodzajePaczek)
                {
                    rodzaje[x] = box.Nazwa + "\t \t" + box.Data.ToShortDateString()+ "\t \t"+box.Waluta;
                    x++;
                }
                pacz.Items.AddRange(rodzaje);
                //dokumenty.Clear();
            }
            catch(Exception e)
            {
                MessageBox.Show("Błąd inicjalizacji formu: " + e.Message, "Komunikat",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            

        }

        private  List<ListaPaczek> SelectListRowColumn(string query, string srcTableName)
        {
            List<string> str = new List<string>();
            List<ListaPaczek> myList = new List<ListaPaczek>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(row[column].ToString());

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                        myList.Add(new ListaPaczek() { ID = Convert.ToInt32(str[0]), Nazwa = str[1], Data=Convert.ToDateTime(str[2]) , Rejestr=str[3], Waluta=str[4]});
                        str.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return myList;
        }


        private void button_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(pacz.SelectedItem) == "" )
            {
                MessageBox.Show("Proszę wybrać paczkę ", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else if(checkKaucja.Checked==false && checkPlat.Checked==false )
            {
                MessageBox.Show("Proszę wybrać płatności lub kaucje ", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if(label3.Enabled==true && (label3.Text.Contains("Błąd") || label3.Text==""))
            {
                MessageBox.Show("Nie znaleziono faktury", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (label3.Enabled == false)
                {

                    //MessageBox.Show("doki count: "+doki.Count, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    foreach (ListaDok dok in doki)
                    {
                        
                      //  MessageBox.Show("Faktura: "+dok.GIDNumer, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dodajDokumenty(dok);
                    }
                }
                else
                {
                    doki.Clear();

                    if (checkKaucja.Checked == true && checkPlat.Checked==false)
                    {
                        string nrDok = "SELECT TrP_GIDTyp, TrP_GIDFirma, TrP_GIDNumer, TrP_GIDLp  FROM cdn. TraPlat join [CDN].[Atrybuty] on TrP_GIDNumer = Atr_ObiNumer and TrP_GIDTyp = Atr_ObiTyp  join cdn.AtrybutyKlasy on Atr_AtkId = AtK_ID "
                                        + " WHERE ATK_Nazwa = 'WF_Barcode' and TrP_GIDNumer=Atr_ObiNumer and TrP_GIDTyp =Atr_ObiTyp and Atr_Wartosc = '" + barcode.Text + "' and TrP_FormaNazwa like '%Kaucja%'";
                        doki = SelectListDok(nrDok, "cdn.TraPlat");
                    }
                    else if (checkPlat.Checked==true && checkKaucja.Checked==false)
                    {
                        string nrDok = "SELECT TrP_GIDTyp, TrP_GIDFirma, TrP_GIDNumer, TrP_GIDLp  FROM cdn. TraPlat join [CDN].[Atrybuty] on TrP_GIDNumer = Atr_ObiNumer and TrP_GIDTyp = Atr_ObiTyp  join cdn.AtrybutyKlasy on Atr_AtkId = AtK_ID "
                                        + " WHERE ATK_Nazwa = 'WF_Barcode' and TrP_GIDNumer=Atr_ObiNumer and TrP_GIDTyp =Atr_ObiTyp and Atr_Wartosc = '" + barcode.Text + "' and TrP_FormaNazwa not like '%Kaucja%'";
                        doki = SelectListDok(nrDok, "cdn.TraPlat");
                    }
                    else
                    {
                        string nrDok = "SELECT TrP_GIDTyp, TrP_GIDFirma, TrP_GIDNumer, TrP_GIDLp  FROM cdn. TraPlat join [CDN].[Atrybuty] on TrP_GIDNumer = Atr_ObiNumer and TrP_GIDTyp = Atr_ObiTyp  join cdn.AtrybutyKlasy on Atr_AtkId = AtK_ID "
                                        + " WHERE ATK_Nazwa = 'WF_Barcode' and TrP_GIDNumer=Atr_ObiNumer and TrP_GIDTyp =Atr_ObiTyp and Atr_Wartosc = '" + barcode.Text + "'";
                        doki = SelectListDok(nrDok, "cdn.TraPlat");
                    }

                    foreach (ListaDok dok in doki)
                    {
                        dodajDokumenty(dok);
                    }
                }

                MessageBox.Show("Zakończono dodawanie dokumentów do paczki: " + rodzajePaczek[pacz.SelectedIndex].Nazwa, "Komunikat",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);

                doki.Clear();
                Application.Exit();
            }



        }

        public int logowanie()
        {
            XLLoginInfo_20220 nowyUzytkownik = new XLLoginInfo_20220();
            nowyUzytkownik.Wersja = XLWersjaAPI;
            nowyUzytkownik.Winieta = XLWinieta;
            nowyUzytkownik.TrybWsadowy = XLTrybWsadowy;
            nowyUzytkownik.UtworzWlasnaSesje = XLUtworzWlasnaSesje;
            nowyUzytkownik.SerwerKlucza = XLSQLServer;
            nowyUzytkownik.Baza = XLSQLDatabase;
            nowyUzytkownik.OpeIdent = XLUser;
            nowyUzytkownik.OpeHaslo = XLPass;
            nowyUzytkownik.PortHTTPSerweraKlucza = 1;
            nowyUzytkownik.RodzajSerweraKlucza = 0;
            nowyUzytkownik.SerwerKlucza = @"EKSRV04\COMARCH::5000235184";
            nowyUzytkownik.ProgramID = "ONE-018 Dokumenty -> Paczki przelewów";

            return cdn_api.cdn_api.XLLogin(nowyUzytkownik, ref sesja);

        }

        public void dodajDokumenty(ListaDok dok)
        {
            try
            {


                string tytulQuery = "  if exists (SELECT * FROM cdn.TraNag WHERE TRN_GIDNumer=" + dok.GIDNumer + " and TrN_GIDTyp=" + dok.GIDTyp + ") SELECT TrN_DokumentObcy FROM cdn.TraNag "
                                    + " WHERE TRN_GIDNumer = " + dok.GIDNumer + " and TrN_GIDTyp = " + dok.GIDTyp + " else if exists(SELECT * FROM cdn.BilansOtwarciaSElem WHERE BOS_TrPNumer = " + dok.GIDNumer + " and BOS_TrPTyp = " + dok.GIDTyp + " and BOS_TrPLp=" + dok.GIDLp + ") SELECT BOS_NumerDokumentu FROM cdn.BilansOtwarciaSElem "
                                    + " WHERE BOS_TrPNumer = " + dok.GIDNumer + " and BOS_TrPTyp = " + dok.GIDTyp + " and BOS_TrPLp="+dok.GIDLp+ " else if exists (SELECT * FROM cdn.MemNag WHERE MeN_GIDNumer=" + dok.GIDNumer + " and MeN_GIDTyp=" + dok.GIDTyp + ") SELECT MeN_NumerDokumentu FROM cdn.MemNag "
                                    + " WHERE MeN_GIDNumer = " + dok.GIDNumer + " and MeN_GIDTyp = " + dok.GIDTyp + " else if exists (SELECT * FROM cdn.Preliminarz WHERE PRL_GIDNumer=" + dok.GIDNumer + " and PRL_GIDTyp=" + dok.GIDTyp + ") SELECT PrL_DokumentObcy FROM cdn.Preliminarz "
                                    + " WHERE PRL_GIDNumer = " + dok.GIDNumer + " and PRL_GIDTyp = " + dok.GIDTyp + "   else SELECT ''";
                string tytul = ConnectToSQL(tytulQuery);

                // MessageBox.Show("Tytuł: " + tytul, "Komunikat",
                  //    MessageBoxButtons.OK, MessageBoxIcon.Warning);


                string istniejeQuery = "if exists(SELECT * FROM [CDN].[Przelewy] join cdn.PrzelewyNag on PPN_ID=PPX_PPNID join cdn.PrzelewyElem on PPX_PPELp=PPE_PPELP and PPN_ID=PPE_PPNID  WHERE [PPX_TRPTyp]=" + dok.GIDTyp + " and [PPX_TRPFirma]=" + dok.GIDFirma + " and [PPX_TRPNumer]=" + dok.GIDNumer + " and [PPX_TRPLp]="+dok.GIDLp+ " and PPN_Status!=10 and PPE_Odrzucona!=1) SELECT 'TAK' else SELECT 'NIE'";
                string istnieje = ConnectToSQL(istniejeQuery);

                string czyZgodneQuery= "if((SELECT KAR_Waluta from cdn.Rejestry where Kar_Seria='"+rodzajePaczek[pacz.SelectedIndex].Rejestr+"')=(SELECT TrP_Waluta from cdn.TraPlat where TrP_GIDNumer = "+dok.GIDNumer+" and TrP_GIDTyp = "+dok.GIDTyp+" and TrP_GIDLp = " + dok.GIDLp + ") ) "
                                        + " SELECT 'TAK' else SELECT 'NIE'";
                string zgodne = ConnectToSQL(czyZgodneQuery);

                string statusQuery = "SELECT TRP_Status FROM cdn.TraPlat WHERE TRP_GIDTyp = " + dok.GIDTyp + " and TrP_GIDNumer = " + dok.GIDNumer + " and TrP_GIDLp = " + dok.GIDLp;
                string status = ConnectToSQL(statusQuery);

                string rozlQuery = "SELECT TRP_Rozliczona FROM cdn.TraPlat WHERE TRP_GIDTyp = " + dok.GIDTyp + " and TrP_GIDNumer = " + dok.GIDNumer + " and TrP_GIDLp = " + dok.GIDLp;
                string rozl = ConnectToSQL(rozlQuery);

                if ((istnieje == "NIE" && status == "0" && rozl != "2") && zgodne == "TAK")
                {

                    string lpElemQuery = "if exists (SELECT * FROM cdn.PrzelewyElem WHERE PPE_PPNID=" + rodzajePaczek[pacz.SelectedIndex].ID + ") SELECT max(PPE_PPELp)  FROM cdn.PrzelewyElem WHERE PPE_PPNID =" + rodzajePaczek[pacz.SelectedIndex].ID + " else SELECT 0";
                    // MessageBox.Show("LpElemQuery: " + lpElemQuery, "Komunikat",
                    //      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    int lpElem = Convert.ToInt32(ConnectToSQL(lpElemQuery)) + 1;

                    string lpQuery = "SELECT max(PPE_Lp)  FROM cdn.PrzelewyElem";
                    int lp = Convert.ToInt32(ConnectToSQL(lpQuery)) + 1;



                    string insertElem = "INSERT INTO [CDN].[PrzelewyElem] ([PPE_PPNID] ,[PPE_PPELp] ,[PPE_Odrzucona]  ,[PPE_Lp] ,[PPE_GrpPPELp]  ,[PPE_Grupa]  ,[PPE_TytulWyrazenie]  ,[PPE_TytulWartosc]  ,[PPE_TytulSeparator] ,[PPE_TytulModyfikowany]  ,[PPE_PodzialGrupy]"
                                        + "  ,[PPE_PodzialIloscZnakow]    ,[PPE_ModyfOpeNumer]  ,[PPE_ModyfData]) VALUES (" + rodzajePaczek[pacz.SelectedIndex].ID + " , " + lpElem + " , 0, " + lpElem + ", 0, 0, '', '" + tytul + "' , '', 0, 0, 0, 1, 81097)";

                    //MessageBox.Show("insertElem: " + insertElem, "Komunikat",
                    //      MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    InsertInto(insertElem);


                    string kwotaQuery = "Select [TrP_Pozostaje] from [CDN].[TraPlat] WHERE TrP_GIDtyp=" + dok.GIDTyp + " and TrP_GIDFirma=" + dok.GIDFirma + " and TrP_GIDNumer=" + dok.GIDNumer + " and TrP_GiDLp=" + dok.GIDLp;
                    //MessageBox.Show("kwotaQuery: " + kwotaQuery, "Komunikat",
                    //       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    string kwota = ConnectToSQL(kwotaQuery);

                    /* string lpQuery = " if exists (SELECT * FROM cdn.Przelewy WHERE PPX_PPNID=" + rodzajePaczek[pacz.SelectedIndex].ID + ") SELECT max(PPX_PPELp)  FROM cdn.Przelewy WHERE PPX_PPNID = " + rodzajePaczek[pacz.SelectedIndex].ID + " else SELECT 0";
                     MessageBox.Show("LpQuery: " + lpQuery, "Komunikat",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     int lp = Convert.ToInt32(ConnectToSQL(lpQuery)) + 1;*/





                    string insertQuery = "INSERT INTO [CDN].[Przelewy] ([PPX_TRPTyp] ,[PPX_TRPFirma]  ,[PPX_TRPNumer] ,[PPX_TRPLp] ,[PPX_KwotaAkc] ,[PPX_KwotaExp]  ,[PPX_ExpID] ,[PPX_ExpLin]  ,[PPX_Status] ,[PPX_PPNID] ,[PPX_PPELp])"
                                      + " VALUES (" + dok.GIDTyp + ", " + dok.GIDFirma + ", " + dok.GIDNumer + " , " + dok.GIDLp + ", " + kwota.Replace(",", ".") + " , " + kwota.Replace(",", ".") + ", 0, 0, 0 , " + rodzajePaczek[pacz.SelectedIndex].ID + ", " + lpElem + ")";

                    //  MessageBox.Show("insertQuery: " + insertQuery, "Komunikat",
                    //         MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    InsertInto(insertQuery);

                    string selectKAR = "SELECT  [KAR_GIDTyp] ,[KAR_GIDFirma] ,[KAR_GIDNumer] ,[KAR_GIDLp] FROM [CDN].[Rejestry] WHERE KAR_Seria = '" + rodzajePaczek[pacz.SelectedIndex].Rejestr + "'";
                    List<ListaDok> rejestr = SelectListDok(selectKAR, "cdn.Rejestry");


                    string updateRejestr = "UPDATE CDN.[TraPlat] set TrP_KARFirma = " + rejestr[0].GIDFirma + ", TrP_KARLp = " + rejestr[0].GIDLp + ", TrP_KARNumer = " + rejestr[0].GIDNumer + ", TrP_KARTyp = " + rejestr[0].GIDTyp + " WHERE TrP_GIDFirma = " + dok.GIDFirma + " and TrP_GIDLp = " + dok.GIDLp
                                            + " and TrP_GIDNumer = " + dok.GIDNumer + " and TrP_GIDTyp = " + dok.GIDTyp;
                    Update(updateRejestr);

                }
                else if (istnieje == "TAK")
                    MessageBox.Show("Dokument " + tytul + " jest już dodany do paczki", "Komunikat",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (status != "0") 
                   MessageBox.Show("Dokument " + tytul + " ma status inny niż 'Do wysłania'", "Komunikat",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (rozl == "2") ;
                //   MessageBox.Show("Dokument " + tytul + " ma status 'Nie rozliczaj'", "Komunikat",
                //      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Dokument " + tytul + " ma niezgodną walutę z paczką " + rodzajePaczek[pacz.SelectedIndex].Nazwa, "Komunikat",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(Exception e)
            {
                MessageBox.Show("Błąd dodawania dokumentu: " + e.Message, "Komunikat",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void barcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                label3.Text = "";
                string nrDok = "SELECT TrP_GIDTyp, TrP_GIDFirma, TrP_GIDNumer, TrP_GIDLp  FROM cdn. TraPlat join [CDN].[Atrybuty] on TrP_GIDNumer = Atr_ObiNumer and TrP_GIDTyp = Atr_ObiTyp  join cdn.AtrybutyKlasy on Atr_AtkId = AtK_ID "
                                    + " WHERE ATK_Nazwa = 'WF_Barcode' and TrP_GIDNumer=Atr_ObiNumer and TrP_GIDTyp =Atr_ObiTyp and Atr_Wartosc = '" + barcode.Text + "'";
                List<ListaDok> docs = SelectListDok(nrDok, "cdn.TraPlat");

                if (docs.Count != 0)
                {
                    string daneQuery = " if exists(SELECT * FROM cdn.TraNag WHERE TRN_GIDNumer = " + docs[0].GIDNumer + " and TrN_GIDTyp = " + docs[0].GIDTyp + ") SELECT CONCAT('Numer: ' , cdn.NumerDokumentu(TrN_GIDTyp, Trn_SpiTyp, TrN_TrNTyp, TrN_TrNNumer, TrN_TrNRok, TrN_TrNSeria, TrN_TrNMiesiac),' ;\nDokument obcy: ', TrN_DokumentObcy,' ;\nKontrahent: ', Knt_Akronim,' ;\nData: ', CAST(DateAdd(day, TrN_Data2 - 4, '1801-01-01') as date), ' " 
                                        + " ;\nKwota: ', (SELECT ISNULL(SUM(TRP_Pozostaje),0) FROM cdn.TraPlat WHERE TRP_GIDNumer = " + docs[0].GIDNumer + " and TrP_GIDTyp = " + docs[0].GIDTyp + ") - (SELECT ISNULL(SUM(TRP_Kwota),0) FROM cdn.TraPlat WHERE TRP_GIDNumer = " + docs[0].GIDNumer + " and TrP_GIDTyp = "+ docs[0].GIDTyp + " and (TrP_FormaNazwa like '%kaucja%' or TRP_Rozliczona=2 ) ),' ',TrN_Waluta, ' ;\nRachunek: ', [RkB_NrRachunku], " 
                                        + "' ;\nMPP: ', (SELECT ISNULL(SUM([TrP_SPKwotaVAT]),0) FROM cdn.TraPlat WHERE TRP_GIDNumer = " + docs[0].GIDNumer + " and TrP_GIDTyp = " + docs[0].GIDTyp + ")  , ' ;\nTermin płatności: ', CAST(DateAdd(day, (SELECT TOP (1) TrP_Termin FROM cdn.TraPlat where TRP_GIDNumer = " + docs[0].GIDNumer + " and TrP_GIDTyp = " + docs[0].GIDTyp + " )- 4, '1801-01-01') as date) ) "
                                        + " FROM cdn.TraNag join cdn.KntKarty on Knt_GIDNumer = TrN_KntNumer and Knt_GIDTyp = TrN_KntTyp join cdn.TraPlat on TrP_GIDNumer = TrN_GIDNumer and TRP_GIDTyp = TrN_GIDTyp join cdn.RachunkiBankowe on TrP_RachBank=Rkb_id"
                                        + " WHERE TRN_GIDNumer = " + docs[0].GIDNumer + " and TrN_GIDTyp = " + docs[0].GIDTyp + " and TRP_GIDLp=" + docs[0].GIDLp + " else if exists(SELECT * FROM cdn.BilansOtwarciaNag WHERE BON_GIDNumer = " + docs[0].GIDNumer + "and BON_GIDTyp = " + docs[0].GIDTyp + ") "
                                        + " SELECT CONCAT('Opis: ' , BON_Opis,' ;\nData: ', CAST(DateAdd(day, BON_DataWprow - 4, '1801-01-01') as date), ' ;\nKwota: ', TrP_Kwota, ' ;\nRachunek: ', [RkB_NrRachunku], ' ;\nMPP: ', [TrP_SPKwotaVAT]) , ' ;\nTermin płatności: ', CAST(DateAdd(day, (SELECT TOP (1) TrP_Termin FROM cdn.TraPlat where TRP_GIDNumer = " + docs[0].GIDNumer + " and TrP_GIDTyp = " + docs[0].GIDTyp + " ) - 4, '1801-01-01') as date) FROM cdn.BilansOtwarciaNag join cdn.TraPlat on TrP_GIDNumer = BON_GIDNumer and TRP_GIDTyp = BON_GIDTyp join cdn.RachunkiBankowe on TrP_RachBank=Rkb_id"
                                        + " WHERE BON_GIDNumer = " + docs[0].GIDNumer + " and BON_GIDTyp = " + docs[0].GIDTyp + " and TRP_GIDLp=" + docs[0].GIDLp + " else SELECT ''";
                    label3.Text = ConnectToSQL(daneQuery);
                }
            }
            catch(Exception w)
            {
                label3.Text = "Błąd pobierania faktury: " + w.Message;
            }




        }

        private void Paczki_Close(object sender, EventArgs e)
        {
           doki.Clear();
            
        }
        private List<ListaDok> SelectListDok(string query, string srcTableName)
        {
            List<string> str = new List<string>();
            List<ListaDok> myList = new List<ListaDok>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(row[column].ToString());

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                        myList.Add(new ListaDok()
                        {
                            GIDTyp = Convert.ToInt16(str[0]),
                            GIDFirma = Convert.ToInt32(str[1]),
                            GIDNumer = Convert.ToInt32(str[2]),
                            GIDLp = Convert.ToInt16(str[3])
                        });
                        str.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return myList;
        }

        public string ConnectToSQL(string query)
        {
            string str = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Query");
                    if (dataSet.Tables.Count == 0)
                    {
                        return "";
                    }
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        str = row[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show( str = str + "Bład SQL:" + ex.ToString() + "\n");
            }
            return str;
        }

        public string InsertInto(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.InsertCommand = new SqlCommand(sql, connection);
                    adapter.InsertCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row inserted !! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }

        public string Update(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.UpdateCommand = new SqlCommand(sql, connection);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row updated !! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
