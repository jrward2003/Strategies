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
	public class MyCustomStrategyV1 : Strategy
	{
		private SMA smaFastV1;
		private SMA smaSlowV1;		
		private SMA smaFastV2;
		private SMA smaSlowV2;	
		private SMA smaFastV3;
		private SMA smaSlowV3;	
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "MyCustomStrategyV1";
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
				EntriesPerDirection							= 3;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				MoneyLost									= 1000.00;
				FastV1 = 76; //9 //71
				SlowV1 = 61; //25 //15
				FastV2 = 181; //7  //61
				SlowV2 = 46; //40  //20
				FastV3 = 146; //31  //11
				SlowV3 = 121; //50  //60		
				DefaultOrderAmountV1 = 3;	
				DefaultOrderAmountV2 = 3;
				DefaultOrderAmountV3 = 3;
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Minute, 3); //BarsArray[1]
				
				//3 minute timeframe
				AddDataSeries(Data.BarsPeriodType.Minute, 5); //BarsArray[2]  //Enter Position				
			}
			else if(State == State.DataLoaded)
			{				
				smaFastV1 = SMA(FastV1);
				smaSlowV1 = SMA(SlowV1);
				smaFastV2 = SMA(FastV2);
				smaSlowV2 = SMA(SlowV2);
				smaFastV3 = SMA(FastV3);
				smaSlowV3 = SMA(SlowV3);
				
				smaFastV1.Plots[0].Brush = Brushes.Aqua;
				smaSlowV1.Plots[0].Brush = Brushes.SeaGreen;
				smaFastV2.Plots[0].Brush = Brushes.Lavender;
				smaSlowV2.Plots[0].Brush = Brushes.DarkOrange;
				smaFastV3.Plots[0].Brush = Brushes.LightCoral;
				smaSlowV3.Plots[0].Brush = Brushes.Purple;				
				
				AddChartIndicator(smaFastV1);
				AddChartIndicator(smaSlowV1);
				AddChartIndicator(smaFastV2);
				AddChartIndicator(smaSlowV2);
				AddChartIndicator(smaFastV3);
				AddChartIndicator(smaSlowV3);				
				
			}			
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			tradestop();			
			
		}
		private void tradestop()
		{
			if (BarsInProgress != 0) 
				return;

			 // Set 1
			if (SystemPerformance.AllTrades.TradesPerformance.Currency.CumProfit <= -MoneyLost)
				return;
			else
			{		
				if(CrossAbove(smaFastV1, smaSlowV1, 1))
				{
					EnterLong(DefaultOrderAmountV1, "Def Buy Order V1");
				}
				else if(CrossBelow(smaFastV1, smaSlowV1, 1))
				{
					EnterShort(DefaultOrderAmountV1, "Def Short Order V1");
					
				}
				if( IsRising(smaFastV1) && (Position.MarketPosition == MarketPosition.Short))
				{
					ExitShort(DefaultOrderAmountV1, "Close V1 Short", "Def Short Order V1" );	
				}
				if( IsFalling(smaFastV1) && (Position.MarketPosition == MarketPosition.Long))
				{
					ExitLong(DefaultOrderAmountV1, "Close V1 Long", "Def Buy Order V1");	
				}
				
				
				
				if(CrossAbove(smaFastV2, smaSlowV2, 1))
				{
					EnterLong(DefaultOrderAmountV2, "Def Buy Order V2");
				}
				if(CrossBelow(smaFastV2, smaSlowV2, 1))
				{
					EnterShort(DefaultOrderAmountV2, "Def Short Order V2");
					
				}
				/*if(CrossBelow(smaSlowV2, smaFastV2, 1) && (Position.MarketPosition == MarketPosition.Flat))
				{
					EnterShort(DefaultOrderAmountV2, "Short Knock V2");
				}*/
				
				if( IsRising(smaFastV2) && (Position.MarketPosition == MarketPosition.Short))
				{
					ExitShort(DefaultOrderAmountV2, "Close V2 Short", "Def Short Order V2" );	
				}
				if( IsFalling(smaFastV2) && (Position.MarketPosition == MarketPosition.Long))
				{
					ExitLong(DefaultOrderAmountV2, "Close V2 Long", "Def Buy Order V2");	
				}
				
				
				
				if(CrossAbove(smaFastV3, smaSlowV3, 1))
				{
					EnterLong(DefaultOrderAmountV3, "Def Buy Order V3");
				}
				if(CrossBelow(smaFastV3, smaSlowV3, 1))
				{
					EnterShort(DefaultOrderAmountV3, "Def Short Order V3");
					
				}
				if( IsRising(smaFastV3) && (Position.MarketPosition == MarketPosition.Short))
				{
					ExitShort(DefaultOrderAmountV3, "Close V3 Short", "Def Short Order V3" );	
				}
				if( IsFalling(smaFastV3) && (Position.MarketPosition == MarketPosition.Long))
				{
					ExitLong(DefaultOrderAmountV3, "Close V3 Long", "Def Buy Order V3");	
				}				
				
				
				
				
			}
		}	

		#region Properties
		[NinjaScriptProperty]
		[Range(-50, int.MaxValue)]
		[Display(Name="MoneyLost", Description="Money lost already", Order=1, GroupName="Parameters")]
		public double MoneyLost
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "FastV1", GroupName = "SMA Lines", Order = 0)]
		public int FastV1
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SlowV1", GroupName = "SMA Lines", Order = 1)]
		public int SlowV1
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "FastV2", GroupName = "SMA Lines", Order = 2)]
		public int FastV2
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SlowV2", GroupName = "SMA Lines", Order = 3)]
		public int SlowV2
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "FastV3", GroupName = "SMA Lines", Order = 4)]
		public int FastV3
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SlowV3", GroupName = "SMA Lines", Order = 5)]
		public int SlowV3
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount V1", GroupName = "Order Amount", Order = 0)]
		public int DefaultOrderAmountV1
		{ get; set; }			
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount V2", GroupName = "Order Amount", Order = 1)]
		public int DefaultOrderAmountV2
		{ get; set; }
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount V3", GroupName = "Order Amount", Order = 2)]
		public int DefaultOrderAmountV3
		{ get; set; }		
		#endregion		
	}
}
