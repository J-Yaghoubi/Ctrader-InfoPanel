
////////////////////////////////////////////////////////////////////////////////////////
///                                     Info Panel                                   ///
///     (Have ADR, ATR, Spread, Unrealized Net P/L and symbol-info in your Chart)    ///
///                                                                                  ///
///         Publish DATE  17-DEC-2021                                                ///
///         Version  1.0.0                                                           ///
///         By  Seyed Jafar Yaghoubi                                                 ///
///         License  MIT                                                             ///
///         More info https://github.com/J-Yaghoubi/                                 ///
///         Contact  algo3xp3rt@gmail.com                                            ///
///                                                                                  ///
////////////////////////////////////////////////////////////////////////////////////////


using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using System.Text;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class DisplayDWMPipsonCharts : Indicator
    {

        //------------------------------------------------------ USER INPUT  

        [Parameter("ATR Method?", DefaultValue = MovingAverageType.Exponential, Group = "ATR Preset")]
        public MovingAverageType AtrCalcMethod { get; set; }

        [Parameter("M1 ATR, Period ", DefaultValue = 15, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_0 { get; set; }
        [Parameter("M5 ATR, Period ", DefaultValue = 12, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_1 { get; set; }
        [Parameter("M15 ATR, Period ", DefaultValue = 16, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_2 { get; set; }
        [Parameter("H1 ATR, Period ", DefaultValue = 24, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_3 { get; set; }
        [Parameter("H4 ATR, Period ", DefaultValue = 32, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_4 { get; set; }
        [Parameter("D1 ATR, Period ", DefaultValue = 22, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_5 { get; set; }
        [Parameter("Other ATR, Period ", DefaultValue = 7, MinValue = 1, Group = "ATR Preset")]
        public int p_Period_6 { get; set; }

        [Parameter("Show Table?", DefaultValue = true, Group = "Title Settings")]
        public bool ShowTitle { get; set; }
        [Parameter("Show Symbol Info?", DefaultValue = true, Group = "Title Settings")]
        public bool ShowChartInfo { get; set; }
        [Parameter("Show Unr.Net P&L?", DefaultValue = true, Group = "Title Settings")]
        public bool ShowUnrInfo { get; set; }

        [Parameter("Title Color", DefaultValue = "Black", Group = "Title Settings")]
        public string Color_Title { get; set; }
        [Parameter("Values Color", DefaultValue = "Indigo", Group = "Title Settings")]
        public string Color_Values { get; set; }
        [Parameter("Spread Color", DefaultValue = "Maroon", Group = "Title Settings")]
        public string Color_Spread { get; set; }

        //------------------------------------------------------ GLOBAL VARIABLES 

        private Bars D1;
        private AverageTrueRange ADR;
        private AverageTrueRange ATR;

        double Current_ADR, Current_ATR;
        int period;
        StringBuilder Display_TEXT;

        //------------------------------------------------------ INITIALIZE

        protected override void Initialize()
        {

            // Select ATR Period

            if (Chart.TimeFrame == TimeFrame.Minute)
                period = p_Period_0;
            else if (Chart.TimeFrame == TimeFrame.Minute5)
                period = p_Period_1;
            else if (Chart.TimeFrame == TimeFrame.Minute15)
                period = p_Period_2;
            else if (Chart.TimeFrame == TimeFrame.Hour)
                period = p_Period_3;
            else if (Chart.TimeFrame == TimeFrame.Hour4)
                period = p_Period_4;
            else if (Chart.TimeFrame == TimeFrame.Daily)
                period = p_Period_5;
            else
                period = p_Period_6;

            // Load Indicator Variable

            try
            {
                D1 = MarketData.GetBars(TimeFrame.Daily);
                ADR = Indicators.AverageTrueRange(MarketData.GetBars(TimeFrame.Daily), p_Period_5, AtrCalcMethod);
                ATR = Indicators.AverageTrueRange(period, AtrCalcMethod);

            } catch
            {
            }

        }


        //------------------------------------------------------ CALCULATE

        public override void Calculate(int index)
        {

            Display_TEXT = new StringBuilder();
            Current_ADR = (ADR.Result.LastValue / Symbol.PipSize);
            Current_ATR = (ATR.Result.LastValue / Symbol.PipSize);

            // Show Parameter on Chart

            DisplayATR();

            if (ShowTitle == true)
                DisplayTitle();

            if (ShowChartInfo == true)
                DisplayInfo();

            if (ShowUnrInfo == true)
                DisplayUnr();
        }

        //------------------------------------------------------ DISPLAY Parameter ON THE CHART

        // Title      

        private void DisplayTitle()
        {
            Display_TEXT.Append("ADR\t" + "ATR\t" + "Spread");
            Display_TEXT.AppendLine();
            Display_TEXT.Append("-------------------       ---------");
            Display_TEXT.AppendLine();
            Display_TEXT.Append(" ");
            Chart.DrawStaticText("Table", Display_TEXT.ToString(), VerticalAlignment.Bottom, HorizontalAlignment.Right, Color_Title);
        }

        //  Symbol info

        private void DisplayInfo()
        {
            Chart.DrawStaticText("ChartInfo", Chart.SymbolName + "  " + Chart.TimeFrame, VerticalAlignment.Top, HorizontalAlignment.Left, Color_Title);
        }


        //  Unrealized P/L

        private void DisplayUnr()
        {
            var unr = (Account.UnrealizedNetProfit).ToString("f1");
            Chart.DrawStaticText("Unrealized", unr, VerticalAlignment.Top, HorizontalAlignment.Right, Color_Values);
        }

        // ADR, ATR       

        private void DisplayATR()
        {
            var adr = Current_ADR.ToString("f0");
            var atr = Current_ATR.ToString("f1");
            var values = string.Format("{0} \t {1} \t            ", adr, atr);
            var spread = (Symbol.Spread / Symbol.PipSize).ToString("f1");

            Chart.DrawStaticText("Volatility", values, VerticalAlignment.Bottom, HorizontalAlignment.Right, Color_Values);
            Chart.DrawStaticText("spread", spread, VerticalAlignment.Bottom, HorizontalAlignment.Right, Color_Spread);
        }


    }
    //END class DisplayDWMPipsonCharts
}
//END namespace cAlgo


