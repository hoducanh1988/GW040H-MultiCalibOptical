using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MultiCalibOpticalBoB_Ver1.Function
{
    public class connectionstatus : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        bool _iqs1700status;
        public bool IQS1700STATUS {
            get { return _iqs1700status; }
            set {
                _iqs1700status = value;
                OnPropertyChanged(nameof(IQS1700STATUS));
            }
        }
        bool _iqs9100bstatus;
        public bool IQS9100BSTATUS {
            get { return _iqs9100bstatus; }
            set {
                _iqs9100bstatus = value;
                OnPropertyChanged(nameof(IQS9100BSTATUS));
            }
        }
        bool _dcax86100dstatus;
        public bool DCAX86100DSTATUS {
            get { return _dcax86100dstatus; }
            set {
                _dcax86100dstatus = value;
                OnPropertyChanged(nameof(DCAX86100DSTATUS));
            }
        }
        bool _ontstatus;
        public bool ONTSTATUS {
            get { return _ontstatus; }
            set {
                _ontstatus = value;
                OnPropertyChanged(nameof(ONTSTATUS));
            }
        }
        bool _sqlstatus;
        public bool SQLSTATUS {
            get { return _sqlstatus; }
            set {
                _sqlstatus = value;
                OnPropertyChanged(nameof(SQLSTATUS));
            }
        }
    }

    public class manualtest : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _iqs610plog;
        public string IQS610PLOG {
            get { return _iqs610plog; }
            set {
                _iqs610plog = value;
                OnPropertyChanged(nameof(IQS610PLOG));
            }
        }
        string _dcax86100dlog;
        public string DCAX86100DLOG {
            get { return _dcax86100dlog; }
            set {
                _dcax86100dlog = value;
                OnPropertyChanged(nameof(DCAX86100DLOG));
            }
        }
        string _ontlog;
        public string ONTLOG {
            get { return _ontlog; }
            set {
                _ontlog = value;
                OnPropertyChanged(nameof(ONTLOG));
            }
        }
    }

    public class defaultsetting : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //BOSA
        #region BOSA
        public int BOSASNLEN {
            get { return Properties.Settings.Default.BosaSNLen; }
            set {
                Properties.Settings.Default.BosaSNLen = value;
                OnPropertyChanged(nameof(BOSASNLEN));
            }
        }
        public string BOSAREPORT {
            get { return Properties.Settings.Default.BosaReportFile; }
            set {
                Properties.Settings.Default.BosaReportFile = value;
                OnPropertyChanged(nameof(BOSAREPORT));
            }
        }
        #endregion

        //DUT
        #region DUT
        public string ONTTYPE {
            get { return Properties.Settings.Default.OntType; }
            set {
                Properties.Settings.Default.OntType = value;
                OnPropertyChanged(nameof(ONTTYPE));
            }
        }
        public string ONTLOGINUSER {
            get { return Properties.Settings.Default.OntLoginUser; }
            set {
                Properties.Settings.Default.OntLoginUser = value;
                OnPropertyChanged(nameof(ONTLOGINUSER));
            }
        }
        public string ONTLOGINPASS {
            get { return Properties.Settings.Default.OntLoginPass; }
            set {
                Properties.Settings.Default.OntLoginPass = value;
                OnPropertyChanged(nameof(ONTLOGINPASS));
            }
        }
        #endregion

        //ER Instrument
        #region ER
        public string ERINSTRNAME {
            get { return Properties.Settings.Default.ERInstrName; }
            set {
                Properties.Settings.Default.ERInstrName = value;
                OnPropertyChanged(nameof(ERINSTRNAME));
            }
        }
        public string ERINSTRGPIB {
            get { return Properties.Settings.Default.ERInstrGPIB; }
            set {
                Properties.Settings.Default.ERInstrGPIB = value;
                OnPropertyChanged(nameof(ERINSTRGPIB));
            }
        }
        public int ERINSTRPORT {
            get { return Properties.Settings.Default.ERInstrPort; }
            set {
                Properties.Settings.Default.ERInstrPort = value;
                OnPropertyChanged(nameof(ERINSTRPORT));
            }
        }
        #endregion

        //Power Instrument
        #region Power
        public string EXFOIP {
            get { return Properties.Settings.Default.EXFOIP; }
            set {
                Properties.Settings.Default.EXFOIP = value;
                OnPropertyChanged(nameof(EXFOIP));
            }
        }
        public int EXFOPORT {
            get { return Properties.Settings.Default.EXFOPort; }
            set {
                Properties.Settings.Default.EXFOPort = value;
                OnPropertyChanged(nameof(EXFOPORT));
            }
        }
        public string PWINSTRNAME {
            get { return Properties.Settings.Default.PWInstrName; }
            set {
                Properties.Settings.Default.PWInstrName = value;
                OnPropertyChanged(nameof(PWINSTRNAME));
            }
        }
        public int PWINSTRPORT {
            get { return Properties.Settings.Default.PWInstrPort; }
            set {
                Properties.Settings.Default.PWInstrPort = value;
                OnPropertyChanged(nameof(PWINSTRPORT));
            }
        }
        public string SWINSTRNAME {
            get { return Properties.Settings.Default.SWInstrName; }
            set {
                Properties.Settings.Default.SWInstrName = value;
                OnPropertyChanged(nameof(SWINSTRNAME));
            }
        }
        public int SWINSTRPORT {
            get { return Properties.Settings.Default.SWInstrPort; }
            set {
                Properties.Settings.Default.SWInstrPort = value;
                OnPropertyChanged(nameof(SWINSTRPORT));
            }
        }
        #endregion

        //Sql Server
        #region Sql
        public string SQLNAME {
            get { return Properties.Settings.Default.SqlName; }
            set {
                Properties.Settings.Default.SqlName = value;
                OnPropertyChanged(nameof(SQLNAME));
            }
        }
        public string SQLUSER {
            get { return Properties.Settings.Default.SqlUser; }
            set {
                Properties.Settings.Default.SqlUser = value;
                OnPropertyChanged(nameof(SQLUSER));
            }
        }
        public string SQLPASS {
            get { return Properties.Settings.Default.SqlPass; }
            set {
                Properties.Settings.Default.SqlPass = value;
                OnPropertyChanged(nameof(SQLPASS));
            }
        }
        public string SQLDB {
            get { return Properties.Settings.Default.SqlDb; }
            set {
                Properties.Settings.Default.SqlDb = value;
                OnPropertyChanged(nameof(SQLDB));
            }
        }
        #endregion

        //USB debugger
        #region Usb Debugger
        public string USBDEBUG1 {
            get { return Properties.Settings.Default.usbDebug1; }
            set {
                Properties.Settings.Default.usbDebug1 = value;
                OnPropertyChanged(nameof(USBDEBUG1));
            }
        }
        public string USBDEBUG2 {
            get { return Properties.Settings.Default.usbDebug2; }
            set {
                Properties.Settings.Default.usbDebug2 = value;
                OnPropertyChanged(nameof(USBDEBUG2));
            }
        }
        public string USBDEBUG3 {
            get { return Properties.Settings.Default.usbDebug3; }
            set {
                Properties.Settings.Default.usbDebug3 = value;
                OnPropertyChanged(nameof(USBDEBUG3));
            }
        }
        public string USBDEBUG4 {
            get { return Properties.Settings.Default.usbDebug4; }
            set {
                Properties.Settings.Default.usbDebug4 = value;
                OnPropertyChanged(nameof(USBDEBUG4));
            }
        }
        #endregion

        //Config test
        #region Config test
        public bool ENABLETUNINGPOWER {
            get { return Properties.Settings.Default.enabletuningpower; }
            set {
                Properties.Settings.Default.enabletuningpower = value;
                OnPropertyChanged(nameof(ENABLETUNINGPOWER));
            }
        }
        public bool ENABLETUNINGER {
            get { return Properties.Settings.Default.enabletuninger; }
            set {
                Properties.Settings.Default.enabletuninger = value;
                OnPropertyChanged(nameof(ENABLETUNINGER));
            }
        }
        public bool ENABLETXDDMI {
            get { return Properties.Settings.Default.enabletxddmi; }
            set {
                Properties.Settings.Default.enabletxddmi = value;
                OnPropertyChanged(nameof(ENABLETXDDMI));
            }
        }
        public bool ENABLESIGNALOFF {
            get { return Properties.Settings.Default.enablesignaloff; }
            set {
                Properties.Settings.Default.enablesignaloff = value;
                OnPropertyChanged(nameof(ENABLESIGNALOFF));
            }
        }
        public bool ENABLEWRITEFLASH {
            get { return Properties.Settings.Default.enablewriteflash; }
            set {
                Properties.Settings.Default.enablewriteflash = value;
                OnPropertyChanged(nameof(ENABLEWRITEFLASH));
            }
        }
        public bool ENABLEVERIFYSIGNAL {
            get { return Properties.Settings.Default.enableverifysignal; }
            set {
                Properties.Settings.Default.enableverifysignal = value;
                OnPropertyChanged(nameof(ENABLEVERIFYSIGNAL));
            }
        }

        #endregion

        //Attenuation
        #region Attenuation
        public string POWERCABLEATTENUATION {
            get { return Properties.Settings.Default.powerCableAtt; }
            set {
                Properties.Settings.Default.powerCableAtt = value;
                OnPropertyChanged(nameof(POWERCABLEATTENUATION));
            }
        }
        public string ERCABLEATTENUATION {
            get { return Properties.Settings.Default.erCableAtt; }
            set {
                Properties.Settings.Default.erCableAtt = value;
                OnPropertyChanged(nameof(ERCABLEATTENUATION));
            }
        }
        #endregion

        public void Save() {
            Properties.Settings.Default.Save();
        }
    }

    public class testinginfo : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _ontindex;
        public string ONTINDEX {
            get { return _ontindex; }
            set {
                _ontindex = value;
                OnPropertyChanged(nameof(ONTINDEX));
            }
        }
        string _comport;
        public string COMPORT {
            get { return _comport; }
            set {
                _comport = value;
                OnPropertyChanged(nameof(COMPORT));
            }
        }
        string _macaddress;
        public string MACADDRESS {
            get { return _macaddress; }
            set {
                _macaddress = value;
                OnPropertyChanged(nameof(MACADDRESS));
            }
        }
        string _bosaserial;
        public string BOSASERIAL {
            get { return _bosaserial; }
            set {
                _bosaserial = value;
                OnPropertyChanged(nameof(BOSASERIAL));
            }
        }
        string _tuningpowerresult;
        public string TUNINGPOWERRESULT {
            get { return _tuningpowerresult; }
            set {
                _tuningpowerresult = value;
                OnPropertyChanged(nameof(TUNINGPOWERRESULT));
            }
        }
        string _tuningerresult;
        public string TUNINGERRESULT {
            get { return _tuningerresult; }
            set {
                _tuningerresult = value;
                OnPropertyChanged(nameof(TUNINGERRESULT));
            }
        }
        string _txddmiresult;
        public string TXDDMIRESULT {
            get { return _txddmiresult; }
            set {
                _txddmiresult = value;
                OnPropertyChanged(nameof(TXDDMIRESULT));
            }
        }
        string _signaloffresult;
        public string SIGNALOFFRESULT {
            get { return _signaloffresult; }
            set {
                _signaloffresult = value;
                OnPropertyChanged(nameof(SIGNALOFFRESULT));
            }
        }
        string _writeflashresult;
        public string WRITEFLASHRESULT {
            get { return _writeflashresult; }
            set {
                _writeflashresult = value;
                OnPropertyChanged(nameof(WRITEFLASHRESULT));
            }
        }
        string _verifysignalresult;
        public string VERIFYSIGNALRESULT {
            get { return _verifysignalresult; }
            set {
                _verifysignalresult = value;
                OnPropertyChanged(nameof(VERIFYSIGNALRESULT));
            }
        }
        string _totalresult;
        public string TOTALRESULT {
            get { return _totalresult; }
            set {
                _totalresult = value;
                OnPropertyChanged(nameof(TOTALRESULT));
            }
        }
        string _systemlog;
        public string SYSTEMLOG {
            get { return _systemlog; }
            set {
                _systemlog = value;
                OnPropertyChanged(nameof(SYSTEMLOG));
            }
        }
        string _errorcode;
        public string ERRORCODE {
            get { return _errorcode; }
            set {
                _errorcode = value;
                OnPropertyChanged(nameof(ERRORCODE));
            }
        }
        string _ontlog;
        public string ONTLOG {
            get { return _ontlog; }
            set {
                _ontlog = value;
                OnPropertyChanged(nameof(ONTLOG));
            }
        }

        public testinginfo() {
            Initialization();
        }

        public void Initialization() {
            this.MACADDRESS = "--";
            this.BOSASERIAL = "--";
            this.TUNINGPOWERRESULT = nameof(Parameters.testStatus.NONE);
            this.TUNINGERRESULT = nameof(Parameters.testStatus.NONE);
            this.TXDDMIRESULT = nameof(Parameters.testStatus.NONE);
            this.SIGNALOFFRESULT = nameof(Parameters.testStatus.NONE);
            this.WRITEFLASHRESULT = nameof(Parameters.testStatus.NONE);
            this.VERIFYSIGNALRESULT = nameof(Parameters.testStatus.NONE);
            this.TOTALRESULT = nameof(Parameters.testStatus.NONE);
            this.SYSTEMLOG = "";
            this.ONTLOG = "";
            this.ERRORCODE = "";
        }
    }

    public class bosainfo {
        public string tbID { get; set; }
        public string BosaSN { get; set; }
        public string Ith { get; set; }
        public string Vbr { get; set; }
    }

    public class variables {

        public variables() {
            Pwr_1 = 0;
            Pwr_2 = 0;
            Pwr_temp = 0;
            ER_temp = 0;
            Slope = 0;
            Iav = 0;
            Iav_1 = 0;
            Iav_2 = 0;
            Iav_1_dac = 0;
            Iav_1_dac_hex = "";
            Iav_2_dac = 0;
            Iav_2_dac_hex = "";
            Imod = 0;
            Imod_DAC = "";
            Imod_DAC_Hex = "";
            Iav_DAC = "";
            Iav_DAC_Hex = "";
            TX_Power_DDMI = "";
            Ith = 0;
        }

        public double Pwr_1 { get; set; }
        public double Pwr_2 { get; set; }
        public double Pwr_temp { get; set; }
        public double ER_temp { get; set; }
        public double Slope { get; set; }
        public double Iav { get; set; }
        public double Iav_1 { get; set; }
        public double Iav_2 { get; set; }
        public double Iav_1_dac { get; set; }
        public string Iav_1_dac_hex { get; set; }
        public double Iav_2_dac { get; set; }
        public string Iav_2_dac_hex { get; set; }
        public double Imod { get; set; }
        public string Imod_DAC { get; set; }
        public string Imod_DAC_Hex { get; set; }
        public string Iav_DAC { get; set; }
        public string Iav_DAC_Hex { get; set; }
        public string TX_Power_DDMI { get; set; }
        public double Ith { get; set; }

        public string LOS_value = "04";

        public string NOT_LOS_value = "7f";
    }

}
