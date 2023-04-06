using IDensity.AddClasses;
using IDensity.Core.Models.Trends;
using IDensity.DataAccess;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDensity.ViewModels
{
    class MainTrendViewModel: PropertyChangedBase
    {
        public MainTrendViewModel()
        {
            PlotModel = new PlotModel();
            SetAppTrend();
        }

        LinearAxis valueAxis;
        LineSeries[] series = Enumerable.Range(0, 10).Select(i => new LineSeries()).ToArray();

        private PlotModel _plotModel;

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { Set(ref _plotModel, value); }
        }

        void SetAppTrend()
        {
            PlotModel.PlotAreaBackground = OxyColors.DarkGray;
            PlotModel.PlotType = PlotType.XY;
            PlotModel.TextColor = OxyColors.WhiteSmoke;
            PlotModel.IsLegendVisible = true;
            PlotModel.Background = OxyColors.DarkSlateGray;
             
            var dateAxis = new DateTimeAxis(){Position = AxisPosition.Bottom, Key="date",  MaximumRange = 0.001 , MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            PlotModel.Axes.Add(dateAxis);
            valueAxis = new LinearAxis() { MajorGridlineStyle = LineStyle.Solid, Key="x1", MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            valueAxis.
            PlotModel.Axes.Add(valueAxis);
            for (int i = 0; i < 10; i++)
            {
                PlotModel.Series.Add(series[i]);
            }

        }

        public void Add(TimePoint point)
        {
            series[0].Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.time), point.y1));
            valueAxis.Reset();
            PlotModel.InvalidatePlot(false);
        }

    }
}
