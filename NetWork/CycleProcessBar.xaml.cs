using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetWork
{
    /// <summary>
    /// SuspendedBall.xaml 的交互逻辑
    /// </summary>
    public partial class CycleProcessBar : UserControl
    {
        public CycleProcessBar()
        {
            InitializeComponent();

            useRatioPath = new Path
            {
                StrokeThickness = 3,
                Stroke = new LinearGradientBrush(
                new GradientStopCollection
                {
                    new GradientStop{
                        Color =Color.FromArgb(255,255,0,0),
                        Offset =0,
                    },
                    new GradientStop{
                        Color =Color.FromArgb(255,0,255,0),
                        Offset =0
                    },
                }
                )
                {
                    EndPoint = new Point(0.5, 1),
                    StartPoint = new Point(0.5, 0),
                },
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                {
                     new PathFigure
                    {
                        StartPoint = new Point(60, 20), //起始地址
                        Segments = new PathSegmentCollection
                        {
                           useArc
                        }
                    },
                }
                },
            };

            noUseRatioPath = new Path
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                {
                     new PathFigure
                    {
                        StartPoint = new Point(84, 28), //起始地址
                        Segments = new PathSegmentCollection
                        {
                            noUseArc
                        }
                    },
                }
                },
            };

            Parent.Children.Add(useRatioPath);
            Parent.Children.Add(noUseRatioPath);

        }
        public double CurrentValue1
        {
            set { SetValue(value); }
        }
        public int Size { private get; set; }

        private ArcSegment useArc = new ArcSegment(new Point(84, 28), new Size(40, 40), 0, false, SweepDirection.Clockwise, true);
        private ArcSegment noUseArc = new ArcSegment(new Point(60, 20), new Size(40, 40), 0, true, SweepDirection.Clockwise, true);

        //private LinearGradientBrush 

        private Color[] colors = new Color[12];

        private Path useRatioPath = null;
        private Path noUseRatioPath = null;

        /// <summary>
        /// 设置百分百，输入小数，自动乘100
        /// </summary>
        /// <param name="percentValue"></param>
        private void SetValue(double percentValue)
        {

            /*****************************************
              方形矩阵边长为34，半长为17
              环形半径为14，所以距离边框3个像素
              环形描边3个像素
            ******************************************/
            //double angel = percentValue * 360; //角度



            //double radius = 37; //环形半径

            ////起始点
            //double leftStart = 40;
            //double topStart = 3;

            ////结束点
            //double endLeft = 0;
            //double endTop = 0;



            ////数字显示
            ////lbValue.Content = (percentValue * 100).ToString("0") + "%";

            ///***********************************************
            //* 整个环形进度条使用Path来绘制，采用三角函数来计算
            //* 环形根据角度来分别绘制，以90度划分，方便计算比例
            //***********************************************/

            //bool isLagreCircle = false; //是否优势弧，即大于180度的弧形

            //#region 计算终点
            ////小于90度
            //if (angel <= 90)
            //{
            //    /*****************
            //              *
            //              *   *
            //              * * ra
            //       * * * * * * * * *
            //              *
            //              *
            //              *
            //    ******************/
            //    double ra = (90 - angel) * Math.PI / 180; //弧度
            //    endLeft = leftStart + Math.Cos(ra) * radius; //余弦横坐标
            //    endTop = topStart + radius - Math.Sin(ra) * radius; //正弦纵坐标

            //}

            //else if (angel <= 180)
            //{
            //    /*****************
            //              *
            //              *  
            //              * 
            //       * * * * * * * * *
            //              * * ra
            //              *  *
            //              *   *
            //    ******************/

            //    double ra = (angel - 90) * Math.PI / 180; //弧度
            //    endLeft = leftStart + Math.Cos(ra) * radius; //余弦横坐标
            //    endTop = topStart + radius + Math.Sin(ra) * radius;//正弦纵坐标
            //}

            //else if (angel <= 270)
            //{
            //    myCycleProcessBar2.Visibility = Visibility.Visible;
            //    /*****************
            //              *
            //              *  
            //              * 
            //       * * * * * * * * *
            //            * *
            //           *ra*
            //          *   *
            //    ******************/
            //    isLagreCircle = true; //优势弧
            //    double ra = (angel - 180) * Math.PI / 180;
            //    endLeft = leftStart - Math.Sin(ra) * radius;
            //    endTop = topStart + radius + Math.Cos(ra) * radius;
            //}

            //else if (angel < 360)
            //{
            //    /*****************
            //          *   *
            //           *  *  
            //         ra * * 
            //       * * * * * * * * *
            //              *
            //              *
            //              *
            //    ******************/
            //    isLagreCircle = true; //优势弧
            //    double ra = (angel - 270) * Math.PI / 180;
            //    endLeft = leftStart - Math.Cos(ra) * radius;
            //    endTop = topStart + radius - Math.Sin(ra) * radius;
            //}
            //else
            //{
            //    isLagreCircle = true; //优势弧
            //    endLeft = leftStart - 0.001; //不与起点在同一点，避免重叠绘制出非环形
            //    endTop = topStart;
            //}
            //#endregion

            //lbValue.Content = string.Format("{0:F2},{1:F2}", endLeft, endTop);

            //Point arcEndPt = new Point(endLeft, endTop); //结束点
            //Size arcSize = new Size(radius, radius);
            //SweepDirection direction = SweepDirection.Clockwise; //顺时针弧形
            ////弧形
            //ArcSegment arcsegment = new ArcSegment(arcEndPt, arcSize, 0, isLagreCircle, direction, true);

            ////形状集合
            //PathSegmentCollection pathsegmentCollection = new PathSegmentCollection
            //{
            //    arcsegment
            //};

            ////路径描述
            //PathFigure pathFigure = new PathFigure
            //{
            //    StartPoint = new Point(leftStart, angel < 180 ? topStart : 83), //起始地址
            //    Segments = pathsegmentCollection
            //};

            ////路径描述集合
            //PathFigureCollection pathFigureCollection = new PathFigureCollection
            //{
            //    pathFigure
            //};

            ////复杂形状
            //PathGeometry pathGeometry = new PathGeometry
            //{
            //    Figures = pathFigureCollection
            //};

            //if (angel <= 180)
            //{
            //    //Data赋值
            //    myCycleProcessBar1.Data = pathGeometry;
            //}
            //else
            //{
            //    myCycleProcessBar2.Data = pathGeometry;
            //}
            //达到100%则闭合整个
            //if (angel == 360)
            //    myCycleProcessBar1.Data = Geometry.Parse(myCycleProcessBar1.Data.ToString() + " z");
        }

        private void ComputeArcEnd()
        {
            //Brush brush = new LinearGradientBrush(
            //    new GradientStopCollection
            //    {
            //        new GradientStop{
            //            Color =Color.FromArgb(0,0,0,0),
            //            Offset =0,
            //        },
            //        new GradientStop{
            //            Color =Color.FromArgb(0,0,0,0),
            //            Offset =0
            //        },
            //    }
            //    )
            //{
            //    EndPoint = new Point(0.5, 1),
            //    StartPoint = new Point(0.5, 0),
            //};
        }
    }
}
