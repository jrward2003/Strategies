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
	public class MyCustomStrategyV3 : Strategy
	{
		private SMA smaFast;
		private SMA smaSlow;
		
		private RSI rsi1;
		private RSI rsi2;
		private double ATRStop;
		private double ATRPrice;
		private double myATR;
		
		private ATR ATR1;			
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "MyCustomStrategyV3";
				//Calculate									= Calculate.OnBarClose;
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 2;
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
				Fast = 20; //6
				Slow = 40; //40
				DefaultOrderAmount = 2;
				ATRPeriod									= 20;
				ATRMulti									= 2;
				ATRStop										= 1;
				ATRPrice									= 1.00;	
				myATR										= 1;

			}
			else if (State == State.Configure)
			{			

			}
			else if(State == State.DataLoaded)
			{				
				smaFast = SMA(Fast);
				smaSlow = SMA(Slow);
				
				rsi1 = RSI(Fast,1);	
				rsi2 = RSI(Slow,1);	
				
				smaFast.Plots[0].Brush = Brushes.Olive;
				smaSlow.Plots[0].Brush = Brushes.Gray;
				
				AddChartIndicator(smaFast);
				AddChartIndicator(smaSlow);
				
				ATR1 = ATR(Close, Convert.ToInt32(ATRPeriod));
				ATR1.Plots[0].Brush = Brushes.DarkCyan;
				//AddChartIndicator(ATR1);
				AddChartIndicator(rsi1);
			
				
			}			
		}

		protected override void OnBarUpdate()
		{

			//if (BarsInProgress != 0) 
			//	return;

			//if (CurrentBars[0] < 1 && CurrentBars[1])
			//	return;
			if(CurrentBars[0] < BarsRequiredToTrade)
				return;
			
			if (CurrentBars[0] < Slow)
    			return;

			shortCross();
	
		}

		private void shortCross()
		{
			//Print("RSI - " + rsi1.Default[Slow]);
			if((CrossBelow(Close, smaSlow,1)) && (rsi1.Default[Fast] > 53))
			//if((CrossBelow(Close, smaSlow,1)) && (rsi1.Default[Slow] > 32))
			{
				//EnterLong(DefaultOrderAmount, 1, "Def Buy Order V2");
				//EnterLongStopMarket(DefaultOrderAmount, High[0], "Buy Above RSI-" + rsi1.Default[Fast]);
				//EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(), "Sell Above RSI-" + rsi1.Default[Fast]);
				//EnterShort();
				Draw.ArrowDown(this, Convert.ToString(CurrentBar) + " ArrowDown", true, 0, High[0], Brushes.White);
				EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(), "Sell Above RSI-" + rsi1.Default[Fast]);
				//ATRPrice = Convert.ToDouble((ATR1[0] * ATRMulti).ToString("F2")) + Close[0];
				
				//ATRStop = (Position.AveragePrice - (ATRPrice));	
				//ATRStop = (Position.AveragePrice + (ATRPrice));

								
				
				ATRPrice = Close[0] + (myATR * ATRMulti);
				//ATRStop = (Close[0] + (ATRPrice));
				ATRStop = ATRPrice;
				
				
			}
			
			/*
			//if( CrossAbove(smaFast, smaSlow, 1))
			if(CrossAbove(Open, smaFast, 1) && IsRising(smaSlow))
			{
				//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
				//EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
				ExitShort("S- " + rsi1.Default[Slow] + " F- " + rsi1.Default[Fast], "");
				
			}
			*/
			
			
			
					if(Position.MarketPosition == MarketPosition.Short)
					{
						//ATRPrice = (ATR1[0] * ATRMulti);	
						//ATRPrice = Convert.ToDouble((ATR1[0] * ATRMulti).ToString("F2")) + Close[0];
						ATRPrice = Close[0] + (myATR * ATRMulti);
						
					}			
					//if((Position.MarketPosition == MarketPosition.Short) && ((Close[0] + (ATRPrice)) < ATRStop))
					//if((Position.MarketPosition == MarketPosition.Short) && ((Close[0] + (ATRPrice))  < ATRStop))
					if((Position.MarketPosition == MarketPosition.Short) && ((Close[0]  < Close[1]) && (ATRPrice < ATRStop)) )
					{
						//ATRStop = (Close[0] - (ATRPrice));
						//ATRStop = (Close[0] + (ATRPrice));
						
						ATRStop = ATRPrice;
						
					}			
					
					if((Position.MarketPosition == MarketPosition.Short) && (Close[0] != 0))
					{
						//Print("Exit Price - " + Convert.ToDouble(ATRStop.ToString("F2")) + " Price- " + Close[0] );
						ExitShortStopMarket(DefaultOrderAmount, ATRStop, "Exit Short", "");
						SetProfitTarget(CalculationMode.Currency, 15);
					}
					
					if((Position.MarketPosition == MarketPosition.Short) && (Close[0] > ATRStop))
					{
						//Print("Market needed Exit Price - " + Convert.ToDouble(ATRStop.ToString("F2")) + " Price- " + Close[0] );
						ExitShort(DefaultOrderAmount, "Exit Market Short", "");
						SetProfitTarget(CalculationMode.Currency, 10);
						SetStopLoss(CalculationMode.Currency, 8);
					}					

					//if((Position.MarketPosition == MarketPosition.Short) && ((Close[0] - (ATRPrice)) > ATRStop))
					
					
					/*
					if( CrossBelow(Close, smaFast, 1) && IsRising(smaFast) && (Position.MarketPosition == MarketPosition.Short) && (rsi1.Default[Fast] < 47))
					{
						//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
						//EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
						
						ExitShort("JW Exit","");

						
					}	
					*/			
			
			
			
				
			
		}

	
		
		#region Properties
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Fast", GroupName = "SMA Lines", Order = 0)]
		public int Fast
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Slow", GroupName = "SMA Lines", Order = 1)]
		public int Slow
		{ get; set; }	
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount", GroupName = "Order Amount", Order = 0)]
		public int DefaultOrderAmount
		{ get; set; }			
		
		
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ATRPeriod", Order=1, GroupName="Parameters")]
		public int ATRPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="ATRMulti", Order=2, GroupName="Parameters")]
		public double ATRMulti
		{ get; set; }	
		
		#endregion		
	}
}
