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
	public class ESBuySellStrategy : Strategy
	{
		private SMA smaBFast;
		private SMA smaBSlow;
		private SMA smaSFast;
		private SMA smaSSlow;
		
		private MACD macDBuy;
		private MACD macDShort;

		private EMA emaSignal;
		private bool takeLess = false;

		private RSI rsiBuy;
		private RSI rsiShort;			
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ESBuySellStrategy";
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
				bFast					= 20;
				bSlow					= 5;
				sFast					= 2;
				sSlow					= 30;
				
				rsiLongExit				= 42;
				rsiShortExit			= 62;
				
				DefaultOrderAmount 		= 2;	
				exitStrategy 			= true;
				shortProfit				= 200;
				shortStopLoss			= 500;
				longProfit				= 250;
				longStopLoss			= 500;
				
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{
				smaBFast = SMA(bFast);
				smaSFast = SMA(bFast);
				smaBSlow = SMA(sSlow);
				smaSSlow = SMA(sSlow);
				macDBuy = MACD(bFast, bSlow, 4);
				macDShort = MACD(sFast, sSlow, 4);
				emaSignal = EMA(200);
				rsiBuy = RSI(bFast,1);	
				rsiShort = RSI(sFast,1);					

				//smaFast.Plots[0].Brush = Brushes.Goldenrod;
				//smaSlow.Plots[0].Brush = Brushes.SeaGreen;

				AddChartIndicator(emaSignal);
				AddChartIndicator(macDBuy);
				AddChartIndicator(macDShort);
				AddChartIndicator(rsiBuy);
				AddChartIndicator(rsiShort);
				
			}			
		}

		protected override void OnBarUpdate()
		{
			//Add your custom strategy logic here.
			
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
			
			
			 if(Position.MarketPosition == MarketPosition.Flat)
			 {
				 //Price above ema
				 yTKillerMACDStrategy();
			 }
			 
			 if(Position.MarketPosition == MarketPosition.Flat)
			 {
				yTKillerSellMACDStrategy();
			 }	
			 
			 if(Position.MarketPosition != MarketPosition.Flat)
				 allExit();
			 
		}
		


		private void yTKillerMACDStrategy()
		{
			if( CrossAbove(MACD(bFast,bSlow,5), 0, 1) && (rsiBuy.Default[bFast] < 48)) 
			{
				EnterLongLimit(DefaultOrderAmount, GetCurrentAsk(), "MACD Long Entry: " + rsiBuy.Default[bFast]);	
			}		
		}
		private void yTKillerSellMACDStrategy()
		{
			if( CrossBelow(MACD(sSlow,sFast,5), 0, 1) && (rsiShort.Default[sSlow] < 48))
			{
				EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(),"MACD Short Entry: " + rsiShort.Default[sSlow]);
			}
		}		
		
		private void allExit()
		{
			if(Position.MarketPosition == MarketPosition.Long){
				if(CrossAbove(smaBFast, Close, 1) && (rsiBuy.Default[bFast] > rsiLongExit))
				{
					ExitLong("JW Long Exit","");
				}
			}
			if(Position.MarketPosition == MarketPosition.Short){
				if(CrossBelow(smaSSlow, Close, 1) && (rsiShort.Default[sSlow] < rsiShortExit))
				{
					ExitShort("JW Short Exit","");
				}
			}			
		}		
		
		#region Properties
		
		[NinjaScriptProperty]
		[Display(Name="Exit Strategy - Take Profit or Trailing Stop", Description="", Order=201, GroupName="Strategy")]
		public bool exitStrategy
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Take Short Profit($)", Description="Only needed if Exit Strategy is checked", Order=301, GroupName="Strategy")]
		public double shortProfit
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Short Stop Loss($)", Description="Only needed if Exit Strategy is checked", Order=302, GroupName="Strategy")]
		public double shortStopLoss
		{ get; set; }		
		
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Take Long Profit($)", Description="Only needed if Exit Strategy is checked", Order=303, GroupName="Strategy")]
		public double longProfit
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(Name="Stop Long Loss($)", Description="Only needed if Exit Strategy is checked", Order=304, GroupName="Strategy")]
		public double longStopLoss
		{ get; set; }		
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Buy Fast", Order=1, GroupName="Parameters")]
		public int bFast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Buy Slow", Order=2, GroupName="Parameters")]
		public int bSlow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Sell Fast", Order=3, GroupName="Parameters")]
		public int sFast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Sell Slow", Order=4, GroupName="Parameters")]
		public int sSlow
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="RSI Buy Exit", Order=5, GroupName="Parameters")]
		public int rsiLongExit
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="RSI Sell Exit", Order=6, GroupName="Parameters")]
		public int rsiShortExit
		{ get; set; }		
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount", GroupName = "Order Amount", Order = 0)]
		public int DefaultOrderAmount
		{ get; set; }				
		
		
		#endregion
		
		
	}
}
