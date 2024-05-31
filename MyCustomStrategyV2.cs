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
	public class MyCustomStrategyV2 : Strategy
	{
		private SMA smaFast;
		private SMA smaSlow;
		private EMA emaFast;
		
		private RSI rsi1;
		private RSI rsi2;
		
		private double ATRStop;
		private double ATRPrice;
		
		private ATR ATR1;		
		private bool canTrade;
		SessionIterator sessionIterator;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "MyCustomStrategyV2";
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
				MoneyLost					= 10000.00;
				Fast = 20; //6
				Slow = 40; //40
				DefaultOrderAmount = 2;	
				
				//HighLow
				TrailTicks					= 10;
				TickOffset					= 5;

				
				ATRPeriod									= 14;
				ATRMulti									= 2;
				ATRStop										= 1;
				ATRPrice									= 1;
				
				
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Minute, 3); //BarsArray[1]
				
				//3 minute timeframe
				AddDataSeries(Data.BarsPeriodType.Minute, 1); //BarsArray[2]  //Enter Position	
				
				//SetParabolicStop("", CalculationMode.Currency, TrailTicks, false, 0.02, 0.2, 0.002);
				
				
			}
			else if(State == State.DataLoaded)
			{				
				smaFast = SMA(Fast);
				smaSlow = SMA(Slow);
				emaFast = EMA(Fast);
				
				rsi1 = RSI(Fast,1);
				rsi2 = RSI(Slow,1);
				
				smaFast.Plots[0].Brush = Brushes.Aqua;
				smaSlow.Plots[0].Brush = Brushes.SeaGreen;
				//emaFast.Plots[0].Brush = Brushes.DarkSlateBlue;
				
				AddChartIndicator(smaFast);
				
				ATR1 = ATR(Close, Convert.ToInt32(ATRPeriod));
				ATR1.Plots[0].Brush = Brushes.DarkCyan;
				AddChartIndicator(rsi1);
				
				//AddChartIndicator(smaSlow);
				//AddChartIndicator(emaFast);
				

				//SetProfitTarget("", CalculationMode.Ticks, 20);
				SetStopLoss("", CalculationMode.Currency,200, false);
				
			}
			else if(State == State.Historical){
				sessionIterator = new SessionIterator(Bars);
				
			}
		}

		protected override void OnBarUpdate()
		{
			//if (BarsInProgress != 0) 
			//	return; 
			//Test for enough bars in each data set
			if(CurrentBars[0] < BarsRequiredToTrade || CurrentBars[1] < BarsRequiredToTrade || CurrentBars[2] < BarsRequiredToTrade)
				//canTrade = false;
				return;
			/*
			if(Bars.IsFirstBarOfSession){
				
				SystemPerformance.AllTrades.TradesPerformance.Currency.CumProfit = 0;
				//sessionIterator.GetNextSession(Time[0], true);
				
				
			}*/
			
			if (Time[0].Hour == 15 && Time[0].Minute >= 0)
			{
				if(Position.MarketPosition == MarketPosition.Short)
			   		ExitShort("Time Exit","");
				if(Position.MarketPosition == MarketPosition.Long)
					ExitLong("Time Exit","");
				
				return;
			}
			
			
			//if (sessionIterator.IsInSession(DateTime.Now.AddHours(1), true, true)){
				//tradestop();
				

			tradestop();
			//getHighLow();
			//mySMABuyStrategy();
			
		}
		private void getHighLow(){
			// store the highest bars ago value
			//int highestBarsAgo = HighestBar(High, Bars.BarsSinceNewTradingDay);
			int highestBarsAgo = HighestBar(High, 10); // 10 Bars ago aka the last 30 minutes
			int lowestBarAgo = LowestBar(Low, 10);
			//evaluate high price from highest bars ago value
			double highestPrice = High[highestBarsAgo];
			double lowestPrice = Low[lowestBarAgo];
			
			//Printed result:  Highest price of the session: 2095.5 - occurred 24 bars ago
			Print(string.Format("Highest price of the session: {0} - occurred {1} bars ago", highestPrice, highestBarsAgo));  
			Print(string.Format("Lowest price of the session: {0} - occurred {1} bars ago", lowestPrice, lowestBarAgo));
				
		}
		private void tradestop()
		{
			//if (BarsInProgress != 0) 
			//	return;

			 // Set 1
			if (SystemPerformance.AllTrades.TradesPerformance.Currency.CumProfit <= -MoneyLost)
				return;
			else
			{
				
				if(BarsInProgress == 2)
				{
				
					
					// This kinda works
					//if(CrossAbove(smaFast, smaSlow, 1) && (rsi1.Default[Fast] > 47))
					if(CrossAbove(smaFast, Close, 1) && (rsi1.Default[Fast] > 53))
					{
						//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
						//EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
						ExitLong("JW Exit","");
						
					}	
					
					
					
					/*
					if(Position.MarketPosition == MarketPosition.Short)
					{
						ExitShortStopMarket(DefaultOrderAmount, ATRStop, "Exit Short", "");
					}
					if(Position.MarketPosition == MarketPosition.Short)
					{
						ATRPrice = (ATR1[0] * ATRMulti);	
					}
					//if((Position.MarketPosition == MarketPosition.Short) && ((Close[0] - (ATRPrice)) > ATRStop))
					if((Position.MarketPosition == MarketPosition.Short) && ((Close[0] + (ATRPrice)) < ATRStop))
					{
						//ATRStop = (Close[0] - (ATRPrice));
						ATRStop = (Close[0] + (ATRPrice));	
					}					
					
					/*
					if( CrossBelow(Close, smaFast, 1) && IsRising(smaFast) && (Position.MarketPosition == MarketPosition.Short) && (rsi1.Default[Fast] < 47))
					{
						//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
						//EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
						
						ExitShort("JW Exit","");

						
					}	
					*/
					/*
					//if( (Position.MarketPosition == MarketPosition.Short) )
					if( IsRising(smaFast) && (Position.MarketPosition == MarketPosition.Short))
					{
						
						ExitShort();	
						
					}
					else if( IsFalling(smaFast) && (Position.MarketPosition == MarketPosition.Long))
					{
						ExitLong();	
					}*/
					
				}				
				
				if(BarsInProgress == 1)
				{
					//Long Trades
					
					//This kinda works
					//if(CrossAbove(smaFast, smaSlow, 1) && (rsi1.Default[Fast] > 47))
					if(CrossBelow(Close, smaFast,1) && (rsi1.Default[Fast] > 47))
					{
						//EnterLong(DefaultOrderAmount, 1, "Def Buy Order V2");
						//EnterLongStopMarket(DefaultOrderAmount, High[0], "Buy Above RSI-" + rsi1.Default[Fast]);
						EnterLongLimit(DefaultOrderAmount, GetCurrentAsk(), "Buy Above RSI-" + rsi1.Default[Fast]);
					}
					
					
					//No Short Solution yet
					/*
					if(CrossBelow(Close, smaFast,1) && CrossBelow(Close, smaSlow,1) && (rsi1.Default[Fast] > 53))
					{
						//EnterLong(DefaultOrderAmount, 1, "Def Buy Order V2");
						//EnterLongStopMarket(DefaultOrderAmount, High[0], "Buy Above RSI-" + rsi1.Default[Fast]);
						EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(), "Sell Above RSI-" + rsi1.Default[Fast]);
						ATRPrice = (ATR1[0] * ATRMulti);
						//ATRStop = (Position.AveragePrice - (ATRPrice));	
						ATRStop = (Position.AveragePrice + (ATRPrice));	
					}	
					*/
					
					
					//Testing
					/*
					else if(CrossBelow(smaFast, smaSlow, 1) && (rsi1.Default[Fast] > 53))
					{
						//EnterShort(DefaultOrderAmount, 1, "Def Short Order V2");
						EnterShortStopMarket(DefaultOrderAmount, Low[0], "Def Below Sell" + rsi1.Default[Fast]);
						
					}*/
					/*if( IsRising(smaFast) && (Position.MarketPosition == MarketPosition.Short))
					{
						ExitShort();	
					}
					if( IsFalling(smaFast) && (Position.MarketPosition == MarketPosition.Long))
					{
						ExitLong();	
					}*/
				}
				
				
				
				 // Set 2
				//if (Position.MarketPosition == MarketPosition.Long)
				//{
				//	ExitLongStopMarket(Convert.ToInt32(DefaultQuantity), MyStopLow, "JW Sell", "");
				//}
							
				
				
			}
		}	
		//Check SMA on 3 minute and 1 minute charts
		//We want to enter on the 3 minute chart and exit on the 1 minute chart
		private void mySMABuyStrategy()
		{
			//Test for enough bars in each data set
			if(CurrentBars[0] < BarsRequiredToTrade || CurrentBars[1] < BarsRequiredToTrade || CurrentBars[2] < BarsRequiredToTrade)
				return;
			
			//myStopLossTry();
			
			/* OnBarUpdate() method will execute this portion of the code when incoming ticks are on the
			secondary bar object. */
			if (BarsInProgress == 1)
			{
				//EnterLong(0, 1, "Enter Long Test");
				/* Checks if the 5 period SMA is decreasing in the secondary bar series (5min) and if it is below the 10
				period SMA in the tertiary bar series (15min). */
				//if (CrossBelow(SMA1, SMA(BarsArray[1],14), 5))
				//Close[0] < Open[0] Red Candle
				//Close[0] > Open[0] Green Candle
				if ( (Close[0] > Open[0]) && CrossAbove(SMA(BarsArray[1],5), SMA(BarsArray[1],10), 1))
				//if (SMA(BarsArray[1],5)[0] < SMA(BarsArray[1],5)[1] && SMA(BarsArray[1],5)[0] < SMA(BarsArray[2], 10)[0])
				{
					/* Exit the long position entered from the 15min bar object on a more granular time period.
					This allows for more control in the management of your positions and can be used to improve
					exit timing of your trades. */
					//EnterLong(0, 1, "Enter Long Option 1"); //Switched
					EnterShort(0, 1, "Enter Short Option 1");
					//ExitLong(0,1,"Exit Long from 5min", "Enter Long from 15min");
				}
				
				if ( (Close[0] < Open[0]) && CrossBelow(SMA(BarsArray[1],5), SMA(BarsArray[1],10), 1))
				{
					//EnterShort();	//Switched
					EnterLong();
				}
				
				if (CrossBelow(SMA(10), SMA(20), 1))
				{
					//EnterShort(0,1, "Enter Short Option 1");	
					//ExitLongLimit(GetCurrentBid()); //Switched
					ExitShortLimit(GetCurrentBid());
				}
				
			}
			else if (BarsInProgress == 2)
			{
				/* OnBarUpdate() method will execute this portion of the code when incoming ticks are on the
				tertiary bar object (15min). */

				// Checks if the 25 period SMA is greater than the 50 period SMA on the 5min.
				//if (SMA(BarsArray[2],9)[0] > SMA(BarsArray[2],19)[0] && (BarsSinceExitExecution(1,"Enter Long Option 1",0) > 1 || BarsSinceExitExecution(1,"Enter Long Option 1",0) == -1))
				if ((Close[0] > Open[0]) && CrossAbove(SMA(BarsArray[2],9), SMA(BarsArray[2],18), 1))
				{
					/* Enter long for 1 contract on the 5min bar object based on the barsInProgress parameter.
					A value of 0=primary bars, 1=secondary bars, 2=tertiary bars */
					
					//EnterLong(0, 1, "Enter Long Option 2"); //Switched
					EnterShort(0, 1, "Enter Short Option 2"); 
					//ExitLongStopMarket(Convert.ToInt32(DefaultQuantity), MyStopLow, "", "");

				}
				
				//Not working as well
				/*
				if ((Close[0] < Open[0]) && CrossBelow(SMA(BarsArray[2],10), SMA(BarsArray[2],20), 1))
				{
						EnterShort();
				}*/
				
				if (CrossBelow(SMA(9), SMA(18), 1))
				{
					//EnterShort(0,1, "Enter Short Option 1");	
					//ExitLongLimit(GetCurrentBid()); //Switched
					ExitShortLimit(GetCurrentBid());
				}
				//else if (SMA(BarsArray[2],15)[0] < SMA(BarsArray[2],10)[0] && (BarsSinceExitExecution(1,"Exit Long from 1min",0) == 2))
				//{
				//	EnterShort(0,1,"Enter Shorty");	
				//}
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
