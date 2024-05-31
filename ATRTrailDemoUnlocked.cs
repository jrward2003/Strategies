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
	public class ATRTrailDemoUnlocked : Strategy
	{
		private double MyStarStop;
		private double MyStopLow;
		private double ATRStop;
		private double ATRPrice;

		private ATR ATR1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ATRTrailDemoUnlocked";
				Calculate									= Calculate.OnPriceChange;
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
				TrailTicks					= 10;
				TickOffset					= 0;
				ATRPeriod					= 14;
				ATRMulti					= 2;
				MyStarStop					= 1;
				MyStopLow					= 1;
				ATRStop					= 1;
				ATRPrice					= 1;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				ATR1				= ATR(Close, Convert.ToInt32(ATRPeriod));
				ATR1.Plots[0].Brush = Brushes.DarkCyan;
				AddChartIndicator(ATR1);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((State == State.Realtime)
				 && (Position.MarketPosition == MarketPosition.Flat)
				 // Condition group 1
				 && ((BarsSinceExitExecution(0, "", 0) == -1)
				 || (BarsSinceExitExecution(0, "", 0) > 1))
				 && (Close[0] >= High[1]))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
				ATRPrice = (ATR1[0] * ATRMulti) ;
				ATRStop = (Position.AveragePrice - (ATRPrice)) ;
			}
			
			 // Set 2
			if (Position.MarketPosition == MarketPosition.Long)
			{
				ExitLongStopMarket(Convert.ToInt32(DefaultQuantity), ATRStop, "", "");
			}
			
			 // Set 3
			if (Position.MarketPosition == MarketPosition.Long)
			{
				ATRPrice = (ATR1[0] * ATRMulti) ;
			}
			
			 // Set 4
			if ((Position.MarketPosition == MarketPosition.Long)
				 && ((Close[0] + (ATRPrice))  > ATRStop))
			{
				ATRStop = (Close[0] - (ATRPrice)) ;
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TrailTicks", Order=1, GroupName="Parameters")]
		public int TrailTicks
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TickOffset", Order=2, GroupName="Parameters")]
		public int TickOffset
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ATRPeriod", Order=3, GroupName="Parameters")]
		public int ATRPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="ATRMulti", Order=4, GroupName="Parameters")]
		public double ATRMulti
		{ get; set; }
		#endregion

	}
}
