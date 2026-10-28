using Hydra;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using static Przelewy.BO;
using System.Text;
using System.Collections.Generic;

[assembly: CallbackAssemblyDescription(Name: "ONE-018 Dokumenty -> Paczki przelewów",
                                           Description: "Dodawanie dokumentów do paczki przelewów",
                                           Author: "OneSupport",
                                           Version: "1.0",
                                           SystemVersion: "2022.0.2",
                                           Created: "11-01-2023")]
namespace Przelewy
{
    [SubscribeProcedure(Procedures.Preliminarz, "Dokumenty -> Paczki przelewów")]
    public class DodajFakture : Callback
    {
        private ClaWindow RapButton;
        private ClaWindow zakladkaTabWewnetrzne;
        private ClaWindow filtrBtn;

        string XLSQLDatabase = "";
        int listID = 0;
        string connectionString = "";
        private List<ListaDok> doki = new List<ListaDok>();
        public override void Init()
        {
            AddSubscription(true, 0, Events.JustAfterWindowOpening, new TakeEventDelegate(OnOpenWindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
        }



        private bool OnOpenWindow(Procedures ProcId, int ControlId, Events Event)
        {

            ClaWindow Parent = GetWindow();
            Rectangle WndBnd = GetWindow().Bounds;
            zakladkaTabWewnetrzne = GetWindow().AllChildren["?PreliminarzTab"];
            RapButton = zakladkaTabWewnetrzne.Children.Add(ControlTypes.button);
            RapButton.Visible = true;
            RapButton.TextRaw = "Paczki";
            RapButton.ToolTipRaw = "Dodaj dokumenty do paczki przelewów";
            RapButton.OnAfterMouseDown += new TakeEventDelegate(BtnOnMouseDown);

            filtrBtn = GetWindow().AllChildren["?Insert"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X - 25, filtrBtn.Bounds.Y, 22, 15);
            listID = GetWindow().AllChildren["?PreliminarzBrowse"].Id;      //tu sprawdzic

            //pobrane z pola w zakładce Raporty
            return true;
        }

        public bool Resize(Procedures ProcId, int ControlId, Events Event)
        {
            filtrBtn = GetWindow().AllChildren["?Insert"];                            //pobiera współrzędne przycisku zapisz
            RapButton.Bounds = new Rectangle(filtrBtn.Bounds.X-25, filtrBtn.Bounds.Y , 22, 15);
            return true;
        }

        private bool BtnOnMouseDown(Procedures ProcId, int ControlId, Events Event)
        {
            try
            {

               // Application.EnableVisualStyles();
               // Application.SetCompatibleTextRenderingDefault(false);

                ADODB._Recordset recordset = Runtime.WindowController.GetQueueMarked((int)ProcId, listID, GetCallbackThread());
                if (recordset != null)
                {
                    // MessageBox.Show(Convert.ToString(listID), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ZbieranieZS(recordset);
                }
                //proceduraZS();
                if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 9) == "ERPXL_E_P")
                {
                    XLSQLDatabase = "ERPXL_E_P";
                    connectionString = @"Data Source = EKSRV04\COMARCH; Initial Catalog = ERPXL_E_P; User ID = ComarchAPI; Password=hT@S+NQZBT";
                }
                else if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 7) == "ERPXL_E")
                {
                    XLSQLDatabase = "ERPXL_E";
                    connectionString = @"Data Source = EKSRV04\COMARCH; Initial Catalog = ERPXL_E; User ID = ComarchAPI; Password=hT@S+NQZBT";
                }

                Application.Run(new Paczki(connectionString, XLSQLDatabase, doki));

            }
            catch(Exception e)
            {
                MessageBox.Show("Bład aplikacji: "+e.Message, "Komunikat",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            return true;
        }


        bool ZbieranieZS(ADODB._Recordset recordset)
        {

            try
            {
                StringBuilder dokumenty = new StringBuilder();
                if (recordset != null)
                {
                    int count = recordset.RecordCount;

                    if (count != 0)
                    {
                        recordset.MoveFirst();

                        while (recordset.EOF == false)
                        {
                            ADODB.Fields fields = recordset.Fields;

                            doki.Add(new ListaDok()
                            {
                                GIDTyp = Convert.ToInt16(fields[0].Value),
                                GIDFirma = Convert.ToInt32(fields[1].Value),
                                GIDNumer = Convert.ToInt32(fields[2].Value),
                                GIDLp = Convert.ToInt16(fields[3].Value),
                            });
                            recordset.MoveNext();
                        }
                    }
                    else
                    {
                       // MessageBox.Show("Nie zaznaczono dokumentów. Operacja anulowana.", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return true;
                    }
                }
                else
                {
                    //MessageBox.Show("Nie zaznaczono dokumentów. Operacja anulowana.", "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania zaznaczonych dokumentów --> " + ex.Message, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }
        #region
        public override void Cleanup()
        {
        }


    }
}

#endregion