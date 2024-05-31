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
	public class FirstStrategyUnlocked : Strategy
	{
		private SMA smaFast;
		private SMA smaSlow;
			
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "FirstStrategyUnlocked";
				Calculate									= Calculate.OnBarClose;
				//Calculate									= Calculate.OnEachTick;
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
				BarsRequiredToTrade							= 22;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				//StopLossTicks				   = 12;
				//ProfitTargetTicks			   = 100;
				MoneyLost					= 100.00;
				Fast = 11; //6
				Slow = 60; //40
				DefaultOrderAmount = 1;
				
			}
			else if (State == State.Configure)
			{
				
        // Adds a 5-minute Bars object to the strategy and is automatically assigned
        // a Bars object index of 1 since the primary data the strategy is run against
        // set by the UI takes the index of 0.				
				
				
				//1 minute timeframe
				AddDataSeries(Data.BarsPeriodType.Minute, 3); //BarsArray[1]
				
				//3 minute timeframe
				AddDataSeries(Data.BarsPeriodType.Minute, 5); //BarsArray[2]  //Enter Position
				
				//SetParabolicStop(CalculationMode.Ticks, 12);
				//SetProfitTarget(CalculationMode.Ticks, 10);
				//SetTrailStop(CalculationMode.Ticks, 10);
				//SetStopLoss(CalculationMode.Ticks, StopLossTicks);
				//SetProfitTarget(CalculationMode.Ticks, ProfitTargetTicks);								
				
			}
			else if(State == State.DataLoaded)
			{
				//SMA1 = SMA(Close, 5);
				//SMA1.Plots[0].Brush = Brushes.Olive;
				//AddChartIndicator(SMA1);
				
				smaFast = SMA(Fast);
				smaSlow = SMA(Slow);
				
				smaFast.Plots[0].Brush = Brushes.Aqua;
				smaSlow.Plots[0].Brush = Brushes.SeaGreen;
				
				AddChartIndicator(smaFast);
				AddChartIndicator(smaSlow);
				
			}
		}

		protected override void OnBarUpdate()
		{
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
				//mySMABuyStrategy();
				
				if(CrossAbove(smaFast, smaSlow, 1))
				{
					//if(Position.MarketPosition == MarketPosition.Short)
					//	ExitShort();
					//if(BarsSinceExitExecution() > 1)
						EnterLong(DefaultOrderAmount, 1, "Def Buy Order");
					
				}
				else if(CrossBelow(smaFast, smaSlow, 1))
				{
					//if(Position.MarketPosition == MarketPosition.Long)
					//	ExitLong();
					//if(BarsSinceExitExecution() > 1)
						EnterShort(DefaultOrderAmount, 1, "Def Short Order");
					
				}
				if( IsRising(smaFast) && (Position.MarketPosition == MarketPosition.Short))
				{
					ExitShort();	
					
				}
				if( IsFalling(smaFast) && (Position.MarketPosition == MarketPosition.Long))
				{
					ExitLong();	
					
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
		
		#endregion

	}
}
