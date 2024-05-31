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
	public class MyMACDCrossAndOut : Strategy
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
		
		private double ATRStop;
		private double ATRPrice;
		private double myATR;
		//private double ATRMulti;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "MyMACDCrossAndOut";
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
				BarsRequiredToTrade							= 50;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				Fast					= 20;
				Slow					= 40;
				rsiExit					= 45;
				
				DefaultOrderAmount 		= 2;
				ATRMulti				= 1;
				ATRStop					= 1;
				ATRPrice				= 1.00;	
				myATR					= 1;	
				exitStrategy 			= true;
				shortProfit				= 200;
				shortStopLoss			= 500;
				longProfit				= 250;
				longStopLoss			= 500;
			}
			//else if (State == State.Configure)
			//{
				//AddDataSeries("MES JUN24", Data.BarsPeriodType.Minute, 3, Data.MarketDataType.Last);
			//}
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
			
			//customBuySell();
			//myBuySignal();
			//stopWhenLimitReached();
			
			
			//sellStrategy();
			//yTKillerMACDStrategy();
			//buyStrategy();
			
			
			
			 //if((Close[0] > emaSignal[0]) && Position.MarketPosition == MarketPosition.Flat)
			 if(Position.MarketPosition == MarketPosition.Flat)
			 {
				 //Price above ema
				//ytBuyStrategy();
				 yTKillerMACDStrategy();
				 //buyStrategy();
				 //sellStrategy();
			 }
			 
			 
			 if(Position.MarketPosition == MarketPosition.Flat)
			 {
				//yTSellStrategy();
				//yTKillerSellMACDStrategy(); still in use
				 //sellStrategy();
				 //buyStrategy();
			 }
			 

			 if(Position.MarketPosition != MarketPosition.Flat)
				 allExit();
		}
		private void sellStrategy()
		{
			if(CrossAbove(Close, smaSlow,1) && (rsi1.Default[2] > rsiExit))
			{
				//EnterLong(DefaultOrderAmount, 1, "Def Buy Order V2");
				//EnterLongStopMarket(DefaultOrderAmount, High[0], "Buy Above RSI-" + rsi1.Default[Fast]);
				EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(), "RSI- " + rsi1.Default[2]);
			}			
			
		}
		private void buyStrategy()
		{
			if(CrossBelow(Close, smaFast,1) && (rsi1.Default[Fast] > 47))
			{
				//EnterLong(DefaultOrderAmount, 1, "Def Buy Order V2");
				//EnterLongStopMarket(DefaultOrderAmount, High[0], "Buy Above RSI-" + rsi1.Default[Fast]);
				EnterLongLimit(DefaultOrderAmount, GetCurrentAsk(), "Buy Above RSI-" + rsi1.Default[2]);
			}			
		}
		
		private void yTKillerMACDStrategy()
		{
			if( CrossAbove(MACD(Fast,Slow,5), 0, 1) && (rsi1.Default[Fast] < 48)) 
			{
				EnterLongLimit(DefaultOrderAmount, GetCurrentAsk(), "MACD Long Entry: " + rsi1.Default[Fast]);
				
			}
			
			/*
			if( CrossAbove(MACD(Fast,Slow,5), 0, 1))
			{
				ExitLong();	
			}*/
			
		}
		private void yTKillerSellMACDStrategy()
		{
			if( CrossBelow(MACD(Slow,Fast,5), 0, 1) && (rsi1.Default[Slow] < 48))
			{
				EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(),"MACD Short Entry: " + rsi1.Default[Slow]);
			}
			
		}
		private void allExit()
		{
			//Was 65
			if(Position.MarketPosition == MarketPosition.Long){
				if(CrossAbove(smaFast, Close, 1) && (rsi1.Default[Fast] > 65))
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
		
		private void ytBuyStrategy()	
		{
			//MACD(20,40,5)
			//if( (Close[0] > emaSignal[0]) && CrossAbove(MACD(20,40,5), 0, 1) && (ParabolicSAR(.02,.2,.02).Close[0] > Close[0]))
			if( (Close[0] > emaSignal[0]) && CrossAbove(MACD(Fast,Slow,5), 0, 1))
			{
				//EnterLong("MACD Long Entry");
				EnterLongLimit(DefaultOrderAmount, GetCurrentAsk(), "MACD Long Entry");
				//SetProfitTarget("MACD Long Entry", CalculationMode.Currency, longProfit); // 20/4 = 5 - $250 target
				ATRPrice = GetCurrentAsk();
				//ATRPrice = Close[0] - (myATR * ATRMulti);
				//ATRStop = ATRPrice;
				Print("Buy In: " + ATRPrice);
				ATRStop = Close[0] - (myATR * ATRMulti);
				
				//Print("Average: " + Position.AveragePrice);
				//
				//Print("Buy In Stop: " + ATRStop);
			
			}
				if(exitStrategy)
				{
					//Print("Profit Long");
					yTLongExitOption1();
					//SetProfitTarget("MACD Short Entry", CalculationMode.Ticks, 16); // 20/4 = 5 - $250 target
					//SetStopLoss("MACD Short Entry", CalculationMode.Currency, 500, false);
				}
				else
				{
					//Print("Trail Long");
					yTLongExitOption2();
				}	
			
		}
		private void yTLongExitOption1()
		{
			if(Position.MarketPosition == MarketPosition.Long)
			{
				SetProfitTarget("MACD Long Entry", CalculationMode.Currency, longProfit); // 20/4 = 5 - $250 target
				SetStopLoss("MACD Long Entry", CalculationMode.Currency, longStopLoss, false);
			}		
			
		}
		private void yTLongExitOption2()
		{
			if(Position.MarketPosition == MarketPosition.Long)
			{
				ATRPrice = Close[0] - (myATR * ATRMulti);
				//ATRPrice = Close[0] - (myATR * ATRMulti);
			}			

			if((Position.MarketPosition == MarketPosition.Long) && ((Close[0] > Close[1]) && (ATRPrice > ATRStop)) )
			{			
				Print("ATRPrice: " + ATRPrice + " > ATRStop: " + ATRStop);
				ATRStop = ATRPrice;
			}			
			
			if((Position.MarketPosition == MarketPosition.Long))
			{
				ExitLongStopMarket(DefaultOrderAmount, ATRStop, "MACD Long Entry", "");
			}
			
			if((Position.MarketPosition == MarketPosition.Long) && (Close[0] < ATRStop))
			{
				ExitLong();
			}	
			
			
			/*
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
			*/
			
			
			
			
			
			
			
			
			
		}		
		
		private void yTSellStrategy()	
		{

			if( (Close[0] < emaSignal[0]) && CrossBelow(MACD(Fast,Slow,5), 0, 1))
			{
				EnterShortLimit(DefaultOrderAmount, GetCurrentAsk(),"MACD Short Entry");
				ATRPrice = Close[0] + (myATR * ATRMulti);
				ATRStop = ATRPrice;
			}
			
			if(exitStrategy)
			{
				yTSellExitOption1();
				//SetProfitTarget("MACD Short Entry", CalculationMode.Ticks, 16); // 20/4 = 5 - $250 target
				//SetStopLoss("MACD Short Entry", CalculationMode.Currency, 500, false);
			}
			else
			{
				yTSellExitOption2();
			}
			
						
		}		
		private void yTSellExitOption1()
		{
			if(Position.MarketPosition == MarketPosition.Short)
			{
				
				//Print("The Take Less time under 200: " + Time[0].ToString() + " Current Price: " + Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency, Close[0]));
				//SetProfitTarget("MACD Short Entry", CalculationMode.Ticks, 16); // 20/4 = 5 - $250 target
				SetProfitTarget("MACD Short Entry", CalculationMode.Currency, shortProfit);
				SetStopLoss("MACD Short Entry", CalculationMode.Currency, shortStopLoss, false);
			}		
			
		}
		private void yTSellExitOption2()
		{
			if(Position.MarketPosition == MarketPosition.Short)
			{
				ATRPrice = Close[0] + (myATR * ATRMulti);	
			}			

			if((Position.MarketPosition == MarketPosition.Short) && ((Close[0]  < Close[1]) && (ATRPrice < ATRStop)) )
			{			
				ATRStop = ATRPrice;
			}			
			
			if((Position.MarketPosition == MarketPosition.Short) && (Close[0] != 0))
			{
				ExitShortStopMarket(DefaultOrderAmount, ATRStop, "MACD Short Entry", "");
			}
			
			if((Position.MarketPosition == MarketPosition.Short) && (Close[0] > ATRStop))
			{
				ExitShort();
			}			
			
		}
		
		private void myBuySignal()
		{
			
			if (CurrentBar < BarsRequiredToTrade)
				return;

			if ((Close[0] > Open[0]) && CrossAbove(smaFast, smaSlow, 1))
			{
				Print("Code has entered If statement. buying long: " + Close[0] + " Open: " + Open[0]);
				EnterLong();
			}
			else if ((Close[0] < Open[0]) &&  CrossBelow(smaFast, smaSlow, 1))
			{
				EnterShort();			
			}
			
			
			
			
		}
		private void customBuySell()
		{
			/* This print will show every time the OnBarUpdate() method is called. When strategy is stopped it will
			no longer print. */
			Print("OnBarUpdate(): " + Time[0]);
			
			if (Close[0] > Open[0] && entrySubmit == false)
			{
				/* Submits a Long Limit order at the current bar's low. This order has liveUntilCancelled set to true
				so it does not require resubmission on every new bar to keep it alive. */ 
				EnterLongLimit(0, true, 1, Low[0], "Long Limit");
				
				// Set our bool to true to prevent resubmission of our entry order
				entrySubmit = true;
			}
			
			// After 5 bars have passed since entry, exit the position
			if (BarsSinceEntryExecution() >= 5)
			{
				// Submit our exit order
				ExitLong();
			}
			
			// After our position is closed we can reset our bool to allow for entries again
			if (Position.MarketPosition == MarketPosition.Flat && entrySubmit)
			{
				// Reset our bool to false to allow for submission of our entry order
				entrySubmit = false;
			}			
		}
		
		private void stopWhenLimitReached()
		{
			// After our strategy has a PnL greater than $1000 or less than -$400 we will stop our strategy
			if (SystemPerformance.AllTrades.TradesPerformance.Currency.CumProfit > 50000 
				|| SystemPerformance.AllTrades.TradesPerformance.Currency.CumProfit < -400)
			{
				/* A custom method designed to close all open positions and cancel all working orders will be called.
				This will ensure we do not have an unmanaged position left after we halt our strategy. */
				StopStrategy();
				
				// Halt further processing of our strategy
				return;
			}			
			
		}
        protected override void OnOrderUpdate(Order order, double limitPrice, double stopPrice, int quantity, int filled, double averageFillPrice, OrderState orderState, DateTime time, ErrorCode error, string nativeError)
        {
            if (myEntryOrder != null && order.Name == "Long limit")
            {
                // Assign entryOrder in OnOrderUpdate() to ensure the assignment occurs when expected.
                // This is more reliable than assigning Order objects in OnBarUpdate, as the assignment is not gauranteed to be complete if it is referenced immediately after submitting
                myEntryOrder = order;
            }
        }		
        private void StopStrategy()
		{
			// If our Long Limit order is still active we will need to cancel it.
			CancelOrder(myEntryOrder);
			
			// If we have a position we will need to close the position
			if (Position.MarketPosition == MarketPosition.Long)
				ExitLong();			
			else if (Position.MarketPosition == MarketPosition.Short)
				ExitShort();
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
		[Display(Name="RSI Exit", Order=3, GroupName="Parameters")]
		public int rsiExit
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Default Order Amount", GroupName = "Order Amount", Order = 0)]
		public int DefaultOrderAmount
		{ get; set; }			

	
		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="ATRMulti", Order=4, GroupName="Parameters")]
		public double ATRMulti
		{ get; set; }		
		
		
		#endregion

	}
}
