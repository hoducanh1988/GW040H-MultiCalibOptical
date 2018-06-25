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

        double _opacity;
        public double WINDOWOPACITY {
            get { return _opacity; }
            set {
                _opacity = value;
                OnPropertyChanged(nameof(WINDOWOPACITY));
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

        public connectionstatus() {
            IQS1700STATUS = false;
            IQS9100BSTATUS = false;
            DCAX86100DSTATUS = false;
            ONTSTATUS = false;
            SQLSTATUS = false;
            WINDOWOPACITY = 1;
        }
    }

    public class mainwindowinfo : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        string _wtitle = "";
        public string WINDOWTITLE {
            get { return _wtitle; }
            set {
                _wtitle = value;
                OnPropertyChanged(nameof(WINDOWTITLE));
            }
        }

        public mainwindowinfo() {
            WINDOWTITLE = "Tool Multi Calib Optical For Product GW";
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

        //MAC
        #region MAC
        public int MACLEN {
            get { return Properties.Settings.Default.MacLen; }
            set {
                Properties.Settings.Default.MacLen = value;
                OnPropertyChanged(nameof(MACLEN));
            }
        }
        public string MAC6DIGIT {
            get { return Properties.Settings.Default.Mac6digits; }
            set {
                Properties.Settings.Default.Mac6digits = value;
                OnPropertyChanged(nameof(MAC6DIGIT));
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
                GlobalData.connectionManagement.DCAX86100DSTATUS = false;
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
                GlobalData.connectionManagement.IQS9100BSTATUS = false;
                GlobalData.connectionManagement.IQS1700STATUS = false;
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
                GlobalData.connectionManagement.IQS1700STATUS = false;
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
                GlobalData.connectionManagement.IQS9100BSTATUS = false;
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
                GlobalData.connectionManagement.SQLSTATUS = false;
                OnPropertyChanged(nameof(SQLNAME));
            }
        }
        public string SQLUSER {
            get { return Properties.Settings.Default.SqlUser; }
            set {
                Properties.Settings.Default.SqlUser = value;
                GlobalData.connectionManagement.SQLSTATUS = false;
                OnPropertyChanged(nameof(SQLUSER));
            }
        }
        public string SQLPASS {
            get { return Properties.Settings.Default.SqlPass; }
            set {
                Properties.Settings.Default.SqlPass = value;
                GlobalData.connectionManagement.SQLSTATUS = false;
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
                GlobalData.testingDataDut1.ETUNINGPOWER = value;
                GlobalData.testingDataDut2.ETUNINGPOWER = value;
                GlobalData.testingDataDut3.ETUNINGPOWER = value;
                GlobalData.testingDataDut4.ETUNINGPOWER = value;
                OnPropertyChanged(nameof(ENABLETUNINGPOWER));
            }
        }
        public bool ENABLETUNINGER {
            get { return Properties.Settings.Default.enabletuninger; }
            set {
                Properties.Settings.Default.enabletuninger = value;
                GlobalData.testingDataDut1.ETUNINGER = value;
                GlobalData.testingDataDut2.ETUNINGER = value;
                GlobalData.testingDataDut3.ETUNINGER = value;
                GlobalData.testingDataDut4.ETUNINGER = value;
                OnPropertyChanged(nameof(ENABLETUNINGER));
            }
        }
        public bool ENABLETUNINGCROSSING {
            get { return Properties.Settings.Default.enabletuningcrossing; }
            set {
                Properties.Settings.Default.enabletuningcrossing = value;
                GlobalData.testingDataDut1.ETUNINGCROSSING = value;
                GlobalData.testingDataDut2.ETUNINGCROSSING = value;
                GlobalData.testingDataDut3.ETUNINGCROSSING = value;
                GlobalData.testingDataDut4.ETUNINGCROSSING = value;
                OnPropertyChanged(nameof(ENABLETUNINGCROSSING));
            }
        }
        public bool ENABLETXDDMI {
            get { return Properties.Settings.Default.enabletxddmi; }
            set {
                Properties.Settings.Default.enabletxddmi = value;
                GlobalData.testingDataDut1.ETXDDMI = value;
                GlobalData.testingDataDut2.ETXDDMI = value;
                GlobalData.testingDataDut3.ETXDDMI = value;
                GlobalData.testingDataDut4.ETXDDMI = value;
                OnPropertyChanged(nameof(ENABLETXDDMI));
            }
        }
        public bool ENABLESIGNALOFF {
            get { return Properties.Settings.Default.enablesignaloff; }
            set {
                Properties.Settings.Default.enablesignaloff = value;
                GlobalData.testingDataDut1.ESIGNALOFF = value;
                GlobalData.testingDataDut2.ESIGNALOFF = value;
                GlobalData.testingDataDut3.ESIGNALOFF = value;
                GlobalData.testingDataDut4.ESIGNALOFF = value;
                OnPropertyChanged(nameof(ENABLESIGNALOFF));
            }
        }
        public bool ENABLEWRITEFLASH {
            get { return Properties.Settings.Default.enablewriteflash; }
            set {
                Properties.Settings.Default.enablewriteflash = value;
                GlobalData.testingDataDut1.EWRITEFLASH = value;
                GlobalData.testingDataDut2.EWRITEFLASH = value;
                GlobalData.testingDataDut3.EWRITEFLASH = value;
                GlobalData.testingDataDut4.EWRITEFLASH = value;
                OnPropertyChanged(nameof(ENABLEWRITEFLASH));
            }
        }
        public bool ENABLEVERIFYSIGNAL {
            get { return Properties.Settings.Default.enableverifysignal; }
            set {
                Properties.Settings.Default.enableverifysignal = value;
                GlobalData.testingDataDut1.EVERIFYSIGNAL = value;
                GlobalData.testingDataDut2.EVERIFYSIGNAL = value;
                GlobalData.testingDataDut3.EVERIFYSIGNAL = value;
                GlobalData.testingDataDut4.EVERIFYSIGNAL = value;
                OnPropertyChanged(nameof(ENABLEVERIFYSIGNAL));
            }
        }
        public bool ENABLEWRITEMAC {
            get { return Properties.Settings.Default.enablewritemac; }
            set {
                Properties.Settings.Default.enablewritemac = value;
                GlobalData.testingDataDut1.EWRITEMAC = value;
                GlobalData.testingDataDut2.EWRITEMAC = value;
                GlobalData.testingDataDut3.EWRITEMAC = value;
                GlobalData.testingDataDut4.EWRITEMAC = value;
                OnPropertyChanged(nameof(ENABLEWRITEMAC));
            }
        }

        #endregion

        //Attenuation
        #region Attenuation
        //JIG 1
        public string POWERCABLEATTENUATION1 {
            get { return Properties.Settings.Default.powerCableAtt1; }
            set {
                Properties.Settings.Default.powerCableAtt1 = value;
                OnPropertyChanged(nameof(POWERCABLEATTENUATION1));
            }
        }
        public string ERCABLEATTENUATION1 {
            get { return Properties.Settings.Default.erCableAtt1; }
            set {
                Properties.Settings.Default.erCableAtt1 = value;
                OnPropertyChanged(nameof(ERCABLEATTENUATION1));
            }
        }

        //JIG 2
        public string POWERCABLEATTENUATION2 {
            get { return Properties.Settings.Default.powerCableAtt2; }
            set {
                Properties.Settings.Default.powerCableAtt2 = value;
                OnPropertyChanged(nameof(POWERCABLEATTENUATION2));
            }
        }
        public string ERCABLEATTENUATION2 {
            get { return Properties.Settings.Default.erCableAtt2; }
            set {
                Properties.Settings.Default.erCableAtt2 = value;
                OnPropertyChanged(nameof(ERCABLEATTENUATION2));
            }
        }

        //JIG 3
        public string POWERCABLEATTENUATION3 {
            get { return Properties.Settings.Default.powerCableAtt3; }
            set {
                Properties.Settings.Default.powerCableAtt3 = value;
                OnPropertyChanged(nameof(POWERCABLEATTENUATION3));
            }
        }
        public string ERCABLEATTENUATION3 {
            get { return Properties.Settings.Default.erCableAtt3; }
            set {
                Properties.Settings.Default.erCableAtt3 = value;
                OnPropertyChanged(nameof(ERCABLEATTENUATION3));
            }
        }

        //JIG 4
        public string POWERCABLEATTENUATION4 {
            get { return Properties.Settings.Default.powerCableAtt4; }
            set {
                Properties.Settings.Default.powerCableAtt4 = value;
                OnPropertyChanged(nameof(POWERCABLEATTENUATION4));
            }
        }
        public string ERCABLEATTENUATION4 {
            get { return Properties.Settings.Default.erCableAtt4; }
            set {
                Properties.Settings.Default.erCableAtt4 = value;
                OnPropertyChanged(nameof(ERCABLEATTENUATION4));
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

        #region enable tuning

        bool _etuningpower;
        public bool ETUNINGPOWER {
            get { return _etuningpower; }
            set {
                _etuningpower = value;
                OnPropertyChanged(nameof(ETUNINGPOWER));
            }
        }
        bool _etuninger;
        public bool ETUNINGER {
            get { return _etuninger; }
            set {
                _etuninger = value;
                OnPropertyChanged(nameof(ETUNINGER));
            }
        }
        bool _etuningcrossing;
        public bool ETUNINGCROSSING {
            get { return _etuningcrossing; }
            set {
                _etuningcrossing = value;
                OnPropertyChanged(nameof(ETUNINGCROSSING));
            }
        }
        bool _etxddmi;
        public bool ETXDDMI {
            get { return _etxddmi; }
            set {
                _etxddmi = value;
                OnPropertyChanged(nameof(ETXDDMI));
            }
        }
        bool _esignaloff;
        public bool ESIGNALOFF {
            get { return _esignaloff; }
            set {
                _esignaloff = value;
                OnPropertyChanged(nameof(ESIGNALOFF));
            }
        }
        bool _ewriteflash;
        public bool EWRITEFLASH {
            get { return _ewriteflash; }
            set {
                _ewriteflash = value;
                OnPropertyChanged(nameof(EWRITEFLASH));
            }
        }
        bool _everifysignal;
        public bool EVERIFYSIGNAL {
            get { return _everifysignal; }
            set {
                _everifysignal = value;
                OnPropertyChanged(nameof(EVERIFYSIGNAL));
            }
        }
        bool _ewritemac;
        public bool EWRITEMAC {
            get { return _ewritemac; }
            set {
                _ewritemac = value;
                OnPropertyChanged(nameof(EWRITEMAC));
            }
        }
        #endregion


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
        string _gpon;
        public string GPON {
            get { return _gpon; }
            set {
                _gpon = value;
                OnPropertyChanged(nameof(GPON));
            }
        }
        string _wps;
        public string WPS {
            get { return _wps; }
            set {
                _wps = value;
                OnPropertyChanged(nameof(WPS));
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
        string _tuningcrossingresult;
        public string TUNINGCROSSINGRESULT {
            get { return _tuningcrossingresult; }
            set {
                _tuningcrossingresult = value;
                OnPropertyChanged(nameof(TUNINGCROSSINGRESULT));
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
        string _writemac;
        public string WRITEMACRESULT {
            get { return _writemac; }
            set {
                _writemac = value;
                OnPropertyChanged(nameof(WRITEMACRESULT));
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
        string _buttoncontent;
        public string BUTTONCONTENT {
            get { return _buttoncontent; }
            set {
                _buttoncontent = value;
                OnPropertyChanged(nameof(BUTTONCONTENT));
            }
        }
        bool _buttonenable;
        public bool BUTTONENABLE {
            get { return _buttonenable; }
            set {
                _buttonenable = value;
                OnPropertyChanged(nameof(BUTTONENABLE));
            }
        }

        int _autovalue;
        public int AUTOVALUE {
            get { return _autovalue; }
            set {
                _autovalue = value;
                AUTOPROGRESS = string.Format("{0}/{1}", AUTOVALUE, AUTOMAX);
                OnPropertyChanged(nameof(AUTOVALUE));
            }
        }
        int _automax;
        public int AUTOMAX {
            get { return _automax; }
            set {
                _automax = value;
                AUTOPROGRESS = string.Format("{0}/{1}", AUTOVALUE, AUTOMAX);
                OnPropertyChanged(nameof(AUTOMAX));
            }
        }
        string _autoprogress;
        public string AUTOPROGRESS {
            get { return _autoprogress; }
            set {
                _autoprogress = value;
                OnPropertyChanged(nameof(AUTOPROGRESS));
            }
        }
        string _autoerrorrate;
        public string AUTOERRORRATE {
            get { return _autoerrorrate; }
            set {
                _autoerrorrate = value;
                OnPropertyChanged(nameof(AUTOERRORRATE));
            }
        }
        string _totaltime;
        public string TOTALTIME {
            get { return _totaltime; }
            set {
                _totaltime = value;
                OnPropertyChanged(nameof(TOTALTIME));
            }
        }


        public testinginfo() {
            Initialization();
        }

        public void Initialization() {
            this.TOTALTIME = "0";
            this.AUTOPROGRESS = "";
            this.AUTOERRORRATE = "";
            this.MACADDRESS = "--";
            this.BOSASERIAL = "--";
            this.TUNINGPOWERRESULT = nameof(Parameters.testStatus.NONE);
            this.TUNINGERRESULT = nameof(Parameters.testStatus.NONE);
            this.TUNINGCROSSINGRESULT = nameof(Parameters.testStatus.NONE);
            this.TXDDMIRESULT = nameof(Parameters.testStatus.NONE);
            this.SIGNALOFFRESULT = nameof(Parameters.testStatus.NONE);
            this.WRITEFLASHRESULT = nameof(Parameters.testStatus.NONE);
            this.VERIFYSIGNALRESULT = nameof(Parameters.testStatus.NONE);
            this.WRITEMACRESULT = nameof(Parameters.testStatus.NONE);
            this.TOTALRESULT = nameof(Parameters.testStatus.NONE);
            this.SYSTEMLOG = "";
            this.ONTLOG = "";
            this.ERRORCODE = "";
            this.BUTTONCONTENT = "START";
            this.BUTTONENABLE = true;

            this.ETUNINGPOWER = GlobalData.initSetting.ENABLETUNINGPOWER;
            this.ETUNINGER = GlobalData.initSetting.ENABLETUNINGER;
            this.ETUNINGCROSSING = GlobalData.initSetting.ENABLETUNINGCROSSING;
            this.ETXDDMI = GlobalData.initSetting.ENABLETXDDMI;
            this.ESIGNALOFF = GlobalData.initSetting.ENABLESIGNALOFF;
            this.EWRITEFLASH = GlobalData.initSetting.ENABLEWRITEFLASH;
            this.EVERIFYSIGNAL = GlobalData.initSetting.ENABLEVERIFYSIGNAL;
            this.EWRITEMAC = GlobalData.initSetting.ENABLEWRITEMAC;
        }

        public void Initialization_Auto() {
            this.TOTALTIME = "0";
            this.MACADDRESS = "--";
            this.TUNINGPOWERRESULT = nameof(Parameters.testStatus.NONE);
            this.TUNINGERRESULT = nameof(Parameters.testStatus.NONE);
            this.TUNINGCROSSINGRESULT = nameof(Parameters.testStatus.NONE);
            this.TXDDMIRESULT = nameof(Parameters.testStatus.NONE);
            this.SIGNALOFFRESULT = nameof(Parameters.testStatus.NONE);
            this.WRITEFLASHRESULT = nameof(Parameters.testStatus.NONE);
            this.VERIFYSIGNALRESULT = nameof(Parameters.testStatus.NONE);
            this.WRITEMACRESULT = nameof(Parameters.testStatus.NONE);
            this.TOTALRESULT = nameof(Parameters.testStatus.NONE);
            this.SYSTEMLOG = "";
            this.ONTLOG = "";
            this.ERRORCODE = "";
            this.BUTTONCONTENT = "START";
            this.BUTTONENABLE = true;

            this.ETUNINGPOWER = GlobalData.initSetting.ENABLETUNINGPOWER;
            this.ETUNINGER = GlobalData.initSetting.ENABLETUNINGER;
            this.ETUNINGCROSSING = GlobalData.initSetting.ENABLETUNINGCROSSING;
            this.ETXDDMI = GlobalData.initSetting.ENABLETXDDMI;
            this.ESIGNALOFF = GlobalData.initSetting.ENABLESIGNALOFF;
            this.EWRITEFLASH = GlobalData.initSetting.ENABLEWRITEFLASH;
            this.EVERIFYSIGNAL = GlobalData.initSetting.ENABLEVERIFYSIGNAL;
            this.EWRITEMAC = GlobalData.initSetting.ENABLEWRITEMAC;
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

    public class logfileinfo {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string MemorySize { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
    }

}
