#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class YTCrossoverUnlocked : Strategy
	{
		private WMA WMA1;
		private WMA WMA2;
		private EMA EMA1;
		private WMA WMA3;
		private WMA WMA4;
		private EMA EMA2;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "YTCrossoverUnlocked";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				StopLoss					= 12;
				TakeProfit3					= 48;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				WMA1				= WMA(Close, 5);
				WMA2				= WMA(Close, 20);
				EMA1				= EMA(Close, 200);
				WMA3				= WMA(Close, 5);
				WMA4				= WMA(Close, 20);
				EMA2				= EMA(Close, 200);
				WMA1.Plots[0].Brush = Brushes.Moccasin;
				WMA2.Plots[0].Brush = Brushes.Goldenrod;
				EMA1.Plots[0].Brush = Brushes.Red;
				WMA3.Plots[0].Brush = Brushes.OrangeRed;
				WMA4.Plots[0].Brush = Brushes.Sienna;
				EMA2.Plots[0].Brush = Brushes.RosyBrown;
				AddChartIndicator(WMA1);
				AddChartIndicator(WMA2);
				AddChartIndicator(EMA1);
				AddChartIndicator(WMA3);
				AddChartIndicator(WMA4);
				AddChartIndicator(EMA2);
				//SetProfitTarget(@"WMALong", CalculationMode.Ticks, TakeProfit3);
				//SetStopLoss(@"WMAShort", CalculationMode.Ticks, StopLoss, true);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((CrossAbove(WMA1, WMA2, 1))
				 && (GetCurrentBid(0) > EMA1[0]))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), @"WMALong");
			}
			
			 // Set 2
			if ((CrossBelow(WMA3, WMA4, 1))
				 && (GetCurrentBid(0) < EMA2[0]))
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), @"WMAShort");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopLoss", Order=1, GroupName="Parameters")]
		public int StopLoss
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TakeProfit3", Order=2, GroupName="Parameters")]
		public int TakeProfit3
		{ get; set; }
		#endregion

	}
}
