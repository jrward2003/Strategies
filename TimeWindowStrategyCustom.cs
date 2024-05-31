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
	public class TimeWindowStrategyCustom : Strategy
	{
		private double highestHigh = 0;
		private double lowestLow = 0;
		
		private Series<double> range_high;
		private Series<double> range_low;
		
		private Series<int> bias;
		
		private Series<bool> opp_close;
		private Series<bool> took_hl;
		private Series<bool> is_long;
		private Series<bool> is_short;
		
		private Series<bool> t_prev;
		private Series<bool> t_take;
		private Series<bool> t_trade;	
		
		//Time frames to trade in per day
		private int[] prev_starts = new int[] {100, 60000, 80000, 90000};
		private int[] prev_ends = new int[] {20000, 90000, 110000, 130000};
		private int[] take_starts = new int[] {21500, 91500, 111500, 131500};
		private int[] take_ends = new int[] {40000, 110000, 130000, 150000};
		private int[] trade_starts = new int[] {100, 80000, 100000, 120000};
		private int[] trade_ends = new int[] {160000, 160000, 160000, 160000};
		private int tradeZone = 0;	
		private bool inTheZone = false;
		private DateTime startDateTime;
		private DateTime endDateTime;
		
		private List<int> timeBars = new List<int>();
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "TimeWindowStrategyCustom";
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
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Minute, 3);
				
				
				range_high = new Series<double>(this);
				range_low = new Series<double>(this);
				
				bias = new Series<int>(this);
				
				opp_close = new Series<bool>(this);
				took_hl = new Series<bool>(this);
				is_long = new Series<bool>(this);
				is_short = new Series<bool>(this);
				
				t_prev = new Series<bool>(this);
				t_take = new Series<bool>(this);
				t_trade = new Series<bool>(this);				
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom strategy logic here.
			getHighLow();
		}
		private bool check_time(int T1, int T2)
		{
			bool result  = false;
			int T = ToTime(Time[0]);
			if(T1 > T2)
			{
				result = T >= T1 || T <= T2; //T1 = 220000, T2 = 020000
				//result = true;
			}
			else
			{
				result = T >= T1 && T <= T2;
				//result = true;
			}
			return result;
			
		}		
		private void getHighLow()
		{
			if(Times[0][0].Hour >= 0 && Times[0][0].Hour <= 2)
				tradeZone = 0;
			if(Times[0][0].Hour >= 9 && Times[0][0].Hour <= 11)
				tradeZone = 1;
			if(Times[0][0].Hour >= 11 && Times[0][0].Hour <= 13)
				tradeZone = 2;			
			if(Times[0][0].Hour >= 13 && Times[0][0].Hour <= 15)
				tradeZone = 3;			
			
			t_prev[0] = check_time(prev_starts[tradeZone], prev_ends[tradeZone]);
			bool draw = false;
			
			//If in the price timeframe set Start
			if(t_prev[0])
			{
				//inTheZone = true;
				draw = true;
				//timeBars.Add(CurrentBar);
			    //startDateTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, StartHour, StartMinute, 0);
			    //endDateTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, EndHour, EndMinute, 0);				
				
			}
			if(draw)
			{

				//Draw.Line(this, "Test High", startBarsAgo, yvariable, endBarsAgo, endYValue, Brushes.AliceBlue);
			//startDateTime = new DateTime(
			
			
			//Draw.Line(this, Convert.ToString(CurrentBar) + " RangeHigh", 20, range_high[0], 0, range_high[0], Brushes.Yellow);	
			//Draw.Line(this, Convert.ToString(CurrentBar) + " RangeLow", 20, range_low[0], 0, range_low[0], Brushes.Yellow);			
			}
			
		}
	}
}
