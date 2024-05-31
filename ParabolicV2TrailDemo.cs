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
	public class ParabolicV2TrailDemo : Strategy
	{
		private double MyStarStop;

		private ParabolicSAR ParabolicSAR1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ParabolicV2TrailDemo";
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
				MyStarStop					= 1;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				ParabolicSAR1				= ParabolicSAR(Close, 0.02, 0.2, 0.02);
				ParabolicSAR1.Plots[0].Brush = Brushes.Goldenrod;
				AddChartIndicator(ParabolicSAR1);
				SetParabolicStop("", CalculationMode.Currency, TrailTicks, false, 0.02, 0.2, 0.002);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (ParabolicSAR1[0] <= Low[0])
				 && (ParabolicSAR1[0] > MyStarStop))
			{
				MyStarStop = ParabolicSAR1[0];
			}
			
			 // Set 2
			if ((State == State.Realtime)
				 && (Position.MarketPosition == MarketPosition.Flat)
				 // Condition group 1
				 && ((BarsSinceExitExecution(0, "", 0) == -1)
				 || (BarsSinceExitExecution(0, "", 0) > 1))
				 && (Close[0] >= High[1]))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
				MyStarStop = (Position.AveragePrice + (-10 * TickSize)) ;
			}
			
			 // Set 3
			if (Position.MarketPosition == MarketPosition.Long)
			{
				ExitLongStopMarket(Convert.ToInt32(DefaultQuantity), MyStarStop, "", "");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TrailTicks", Order=1, GroupName="Parameters")]
		public int TrailTicks
		{ get; set; }
		#endregion

	}
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0"?>
<ScriptProperties xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Calculate>OnPriceChange</Calculate>
  <ConditionalActions>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set MyStarStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:47:58.0054422</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-04-08T13:47:58.0054422</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">ParabolicSAR(0.02, 0.2, 0.02)[0]</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Parabolic SAR</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ParabolicSAR</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Acceleration</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AccelerationMax</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AccelerationStep</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ParabolicSAR</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Parabolic SAR</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>true</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:48:46.8991105</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">ParabolicSAR(Close, 0.02, 0.2, 0.02)[0]</LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>MyStarStop</VariableName>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:43:20.319941</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:43:20.327941</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Parabolic SAR</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ParabolicSAR</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Acceleration</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AccelerationMax</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AccelerationStep</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ParabolicSAR</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Parabolic SAR</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>true</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:44:22.3087668</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Low</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:44:22.3177662</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <Series1>
                  <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>true</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Low</PriceType>
                  <SeriesType>DefaultSeries</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ParabolicSAR(0.02, 0.2, 0.02)[0] &lt;= Low[0]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Parabolic SAR</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ParabolicSAR</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Acceleration</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AccelerationMax</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AccelerationStep</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ParabolicSAR</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Parabolic SAR</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>true</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:46:56.1278007</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>MyStarStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MyStarStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:46:56.1438013</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ParabolicSAR(0.02, 0.2, 0.02)[0] &gt; MyStarStop</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 1</SetName>
      <SetNumber>1</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:23:06.5574679</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-04-08T13:23:06.5574679</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set MyStarStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:36:50.751156</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-04-08T13:36:50.751156</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (-10 * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:37:21.5547561</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">-10</BindingValue>
                    <IsLiteral>true</IsLiteral>
                    <LiveValue xsi:type="xsd:string">-10</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (-10 * TickSize)) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>MyStarStop</VariableName>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Set MyStarStop</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-04-08T13:36:50.751156</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-04-08T13:36:50.751156</VariableDateTime>
          <VariableBool>false</VariableBool>
          <VariableDouble>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (-10 * TickSize)) </BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Average position price</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Position.AveragePrice</Command>
                <Parameters>
                  <string>OffsetBuilder</string>
                </Parameters>
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-04-08T13:37:21.5547561</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <OffsetBuilder>
                <ConditionOffset>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Ticks</OffsetType>
                </ConditionOffset>
                <Offset>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-10</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-10</LiveValue>
                </Offset>
              </OffsetBuilder>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (-10 * TickSize)) </LiveValue>
          </VariableDouble>
        </ActionProperties>
        <ActionType>SetValue</ActionType>
        <UserVariableType>double</UserVariableType>
        <VariableName>MyStarStop</VariableName>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current state</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>State</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:17:39.8894473</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>State</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>State</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>State.{0}</Command>
                  <Parameters>
                    <string>State</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:17:39.8984488</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>State</ReturnType>
                <StartBar>0</StartBar>
                <State>Realtime</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>State = State.Realtime</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:18:46.531634</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:18:46.5436317</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Flat</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since exit</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceExitExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>ExitExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:19:35.1080181</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:19:35.1160179</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since exit</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceExitExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>ExitExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:20:47.1661043</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:20:47.1741024</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:21:42.9474098</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>High</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:21:42.9644063</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <Series1>
                  <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>true</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>High</PriceType>
                  <SeriesType>DefaultSeries</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &gt;= High[1]</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 2</SetName>
      <SetNumber>2</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:41:24.0766432</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">MyStarStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>MyStarStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MyStarStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:41:53.0143069</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">MyStarStop</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-04-08T13:41:24.0766432</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitLongStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Exit long position by a stop order</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-04-08T13:41:24.0766432</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <StopPrice>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">MyStarStop</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>MyStarStop</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>MyStarStop</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-04-08T13:41:53.0143069</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">MyStarStop</LiveValue>
          </StopPrice>
          <Tag>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set Exit long position by a stop order</StringValue>
              </NinjaScriptString>
            </Strings>
          </Tag>
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-04-08T13:41:24.0766432</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>ExitStop</ActionType>
        <Command>
          <Command>ExitLongStopMarket</Command>
          <Parameters>
            <string>quantity</string>
            <string>stopPrice</string>
            <string>signalName</string>
            <string>fromEntrySignal</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:38:23.5860447</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-04-08T13:38:23.5980445</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 3</SetName>
      <SetNumber>3</SetNumber>
    </ConditionalAction>
  </ConditionalActions>
  <CustomSeries />
  <DataSeries />
  <Description>Enter the description for your new custom Strategy here.</Description>
  <DisplayInDataBox>true</DisplayInDataBox>
  <DrawHorizontalGridLines>true</DrawHorizontalGridLines>
  <DrawOnPricePanel>true</DrawOnPricePanel>
  <DrawVerticalGridLines>true</DrawVerticalGridLines>
  <EntriesPerDirection>1</EntriesPerDirection>
  <EntryHandling>AllEntries</EntryHandling>
  <ExitOnSessionClose>true</ExitOnSessionClose>
  <ExitOnSessionCloseSeconds>30</ExitOnSessionCloseSeconds>
  <FillLimitOrdersOnTouch>false</FillLimitOrdersOnTouch>
  <InputParameters>
    <InputParameter>
      <Default>10</Default>
      <Description />
      <Name>TrailTicks</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
  </InputParameters>
  <IsTradingHoursBreakLineVisible>true</IsTradingHoursBreakLineVisible>
  <IsInstantiatedOnEachOptimizationIteration>true</IsInstantiatedOnEachOptimizationIteration>
  <MaximumBarsLookBack>TwoHundredFiftySix</MaximumBarsLookBack>
  <MinimumBarsRequired>20</MinimumBarsRequired>
  <OrderFillResolution>Standard</OrderFillResolution>
  <OrderFillResolutionValue>1</OrderFillResolutionValue>
  <OrderFillResolutionType>Minute</OrderFillResolutionType>
  <OverlayOnPrice>false</OverlayOnPrice>
  <PaintPriceMarkers>true</PaintPriceMarkers>
  <PlotParameters />
  <RealTimeErrorHandling>StopCancelClose</RealTimeErrorHandling>
  <ScaleJustification>Right</ScaleJustification>
  <ScriptType>Strategy</ScriptType>
  <Slippage>0</Slippage>
  <StartBehavior>WaitUntilFlat</StartBehavior>
  <StopsAndTargets>
    <WizardAction>
      <Children />
      <IsExpanded>false</IsExpanded>
      <IsSelected>true</IsSelected>
      <Name>Parabolic stop</Name>
      <OffsetType>Arithmetic</OffsetType>
      <ActionProperties>
        <Acceleration>
          <DefaultValue>0</DefaultValue>
          <IsInt>false</IsInt>
          <BindingValue xsi:type="xsd:string">0.02</BindingValue>
          <IsLiteral>true</IsLiteral>
          <LiveValue xsi:type="xsd:string">0.02</LiveValue>
        </Acceleration>
        <AccelerationMax>
          <DefaultValue>0</DefaultValue>
          <IsInt>false</IsInt>
          <BindingValue xsi:type="xsd:string">.2</BindingValue>
          <IsLiteral>true</IsLiteral>
          <LiveValue xsi:type="xsd:string">.2</LiveValue>
        </AccelerationMax>
        <AccelerationStep>
          <DefaultValue>0</DefaultValue>
          <IsInt>false</IsInt>
          <BindingValue xsi:type="xsd:string">.002</BindingValue>
          <IsLiteral>true</IsLiteral>
          <LiveValue xsi:type="xsd:string">.002</LiveValue>
        </AccelerationStep>
        <DashStyle>Solid</DashStyle>
        <DivideTimePrice>false</DivideTimePrice>
        <Id />
        <File />
        <IsAutoScale>false</IsAutoScale>
        <IsSimulatedStop>false</IsSimulatedStop>
        <IsStop>false</IsStop>
        <LogLevel>Information</LogLevel>
        <Mode>Currency</Mode>
        <OffsetType>Currency</OffsetType>
        <Priority>Medium</Priority>
        <Quantity>
          <DefaultValue>0</DefaultValue>
          <IsInt>true</IsInt>
          <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
          <DynamicValue>
            <Children />
            <IsExpanded>false</IsExpanded>
            <IsSelected>false</IsSelected>
            <Name>Default order quantity</Name>
            <OffsetType>Arithmetic</OffsetType>
            <AssignedCommand>
              <Command>DefaultQuantity</Command>
              <Parameters />
            </AssignedCommand>
            <BarsAgo>0</BarsAgo>
            <CurrencyType>Currency</CurrencyType>
            <Date>2024-04-08T13:31:35.0935305</Date>
            <DayOfWeek>Sunday</DayOfWeek>
            <EndBar>0</EndBar>
            <ForceSeriesIndex>false</ForceSeriesIndex>
            <LookBackPeriod>0</LookBackPeriod>
            <MarketPosition>Long</MarketPosition>
            <Period>0</Period>
            <ReturnType>Number</ReturnType>
            <StartBar>0</StartBar>
            <State>Undefined</State>
            <Time>0001-01-01T00:00:00</Time>
          </DynamicValue>
          <IsLiteral>false</IsLiteral>
          <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
        </Quantity>
        <ServiceName />
        <ScreenshotPath />
        <SoundLocation />
        <Tag>
          <SeparatorCharacter> </SeparatorCharacter>
          <Strings>
            <NinjaScriptString>
              <Index>0</Index>
              <StringValue>Set Parabolic stop</StringValue>
            </NinjaScriptString>
          </Strings>
        </Tag>
        <TextPosition>BottomLeft</TextPosition>
        <Value>
          <DefaultValue>0</DefaultValue>
          <IsInt>false</IsInt>
          <BindingValue xsi:type="xsd:string">TrailTicks</BindingValue>
          <DynamicValue>
            <Children />
            <IsExpanded>false</IsExpanded>
            <IsSelected>true</IsSelected>
            <Name>TrailTicks</Name>
            <OffsetType>Arithmetic</OffsetType>
            <AssignedCommand>
              <Command>TrailTicks</Command>
              <Parameters />
            </AssignedCommand>
            <BarsAgo>0</BarsAgo>
            <CurrencyType>Currency</CurrencyType>
            <Date>2024-04-08T13:34:17.2165741</Date>
            <DayOfWeek>Sunday</DayOfWeek>
            <EndBar>0</EndBar>
            <ForceSeriesIndex>false</ForceSeriesIndex>
            <LookBackPeriod>0</LookBackPeriod>
            <MarketPosition>Long</MarketPosition>
            <Period>0</Period>
            <ReturnType>Number</ReturnType>
            <StartBar>0</StartBar>
            <State>Undefined</State>
            <Time>0001-01-01T00:00:00</Time>
          </DynamicValue>
          <IsLiteral>false</IsLiteral>
          <LiveValue xsi:type="xsd:string">TrailTicks</LiveValue>
        </Value>
        <VariableDateTime>2024-04-08T13:31:35.0935305</VariableDateTime>
        <VariableBool>false</VariableBool>
      </ActionProperties>
      <ActionType>Misc</ActionType>
      <Command>
        <Command>SetParabolicStop</Command>
        <Parameters>
          <string>fromEntrySignal</string>
          <string>mode</string>
          <string>value</string>
          <string>isSimulatedStop</string>
          <string>acceleration</string>
          <string>accelerationMax</string>
          <string>accelerationStep</string>
        </Parameters>
      </Command>
    </WizardAction>
  </StopsAndTargets>
  <StopTargetHandling>PerEntryExecution</StopTargetHandling>
  <TimeInForce>Gtc</TimeInForce>
  <TraceOrders>false</TraceOrders>
  <UseOnAddTradeEvent>false</UseOnAddTradeEvent>
  <UseOnAuthorizeAccountEvent>false</UseOnAuthorizeAccountEvent>
  <UseAccountItemUpdate>false</UseAccountItemUpdate>
  <UseOnCalculatePerformanceValuesEvent>true</UseOnCalculatePerformanceValuesEvent>
  <UseOnConnectionEvent>false</UseOnConnectionEvent>
  <UseOnDataPointEvent>true</UseOnDataPointEvent>
  <UseOnFundamentalDataEvent>false</UseOnFundamentalDataEvent>
  <UseOnExecutionEvent>false</UseOnExecutionEvent>
  <UseOnMouseDown>true</UseOnMouseDown>
  <UseOnMouseMove>true</UseOnMouseMove>
  <UseOnMouseUp>true</UseOnMouseUp>
  <UseOnMarketDataEvent>false</UseOnMarketDataEvent>
  <UseOnMarketDepthEvent>false</UseOnMarketDepthEvent>
  <UseOnMergePerformanceMetricEvent>false</UseOnMergePerformanceMetricEvent>
  <UseOnNextDataPointEvent>true</UseOnNextDataPointEvent>
  <UseOnNextInstrumentEvent>true</UseOnNextInstrumentEvent>
  <UseOnOptimizeEvent>true</UseOnOptimizeEvent>
  <UseOnOrderUpdateEvent>false</UseOnOrderUpdateEvent>
  <UseOnPositionUpdateEvent>false</UseOnPositionUpdateEvent>
  <UseOnRenderEvent>true</UseOnRenderEvent>
  <UseOnRestoreValuesEvent>false</UseOnRestoreValuesEvent>
  <UseOnShareEvent>true</UseOnShareEvent>
  <UseOnWindowCreatedEvent>false</UseOnWindowCreatedEvent>
  <UseOnWindowDestroyedEvent>false</UseOnWindowDestroyedEvent>
  <Variables>
    <InputParameter>
      <Default>1</Default>
      <Name>MyStarStop</Name>
      <Type>double</Type>
    </InputParameter>
  </Variables>
  <Name>ParabolicV2TrailDemo</Name>
</ScriptProperties>
@*/
#endregion
