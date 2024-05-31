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
	public class ShortMoneyLowRisk : Strategy
	{
		
		private SMA smaFast;
		private SMA smaSlow;
		private MACD macD;
		private Order myEntryOrder = null;
		private bool entrySubmit = false;	
		private EMA emaSignal;
		private bool takeLess = false;
		private bool madePositive = false;
		private RSI rsi1;
		private RSI rsi2;		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ShortMoneyLowRisk";
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
				Fast					= 20;
				Slow					= 40;
				rsiEntry 				= 46;				
				rsiExit					= 60;
				
				DefaultOrderAmount 		= 1;				
			}
			else if (State == State.DataLoaded)
			{
				smaFast = SMA(Fast);
				smaSlow = SMA(Slow);
				macD = MACD(Fast, Slow, 4);
				emaSignal = EMA(200);
				rsi1 = RSI(Fast,1);	
				rsi2 = RSI(Slow,1);					

				smaFast.Plots[0].Brush = Brushes.Goldenrod;
				smaSlow.Plots[0].Brush = Brushes.SeaGreen;

				//AddChartIndicator(smaFast);
				//AddChartIndicator(smaSlow);
				AddChartIndicator(emaSignal);
				AddChartIndicator(macD);
				AddChartIndicator(rsi1);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBar < BarsRequiredToTrade)
				return;				
			
			if (Time[0].Hour == 15 && Time[0].Minute >= 0)
			{
				if(Position.MarketPosition == MarketPosition.Short)
			   		ExitShort("Time Exit","");
				if(Position.MarketPosition == MarketPosition.Long)
					ExitLong("Time Exit","");
				
				return;
			}	
			
			
			yTKillerSellMACDStrategy();


			 if(Position.MarketPosition != MarketPosition.Flat)
				 allExit();			
			
			
		}
		
		private void yTKillerSellMACDStrategy()
		{
			if((Close[0] < emaSignal[0]) && (CrossAbove(MACD(Slow,Fast,5), 0, 1)) && (rsi1.Default[Slow] < rsiEntry))
			{
				EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(),"MACD Short Entry: " + rsi1.Default[Slow]);
			}
			
		}		
		private void allExit()
		{
			//Was 65
			if(Position.MarketPosition == MarketPosition.Long){
				if(CrossAbove(smaFast, Close, 1) && (rsi1.Default[Fast] > rsiExit))
				{
					//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
					//EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
					ExitLong("JW Long Exit","");
					
				}
			}
			if(Position.MarketPosition == MarketPosition.Short){
				if(CrossBelow(smaSlow, Close, 1) && (rsi1.Default[Slow] < rsiExit))
				{
					//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
					//EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
					ExitShort("JW Short Exit","");
					
				}
			}			
		}		
		
		#region Properties	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Fast", Order=1, GroupName="Parameters")]
		public int Fast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Slow", Order=2, GroupName="Parameters")]
		public int Slow
		{ get; set; }
					
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="RSI Entry", Order=3, GroupName="Parameters")]
		public int rsiEntry
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="RSI Exit", Order=4, GroupName="Parameters")]
		public int rsiExit
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount", GroupName = "Order Amount", Order = 0)]
		public int DefaultOrderAmount
		{ get; set; }			
		
		#endregion
		
			
		
	}
}
