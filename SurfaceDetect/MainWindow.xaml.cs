using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SurfaceDetect
{
    public partial class MainWindow : Window
    {
        private int _rows = 5;  // 초기 행 수
        private int _columns = 5;  // 초기 열 수
        private double _canvasWidth = 675;  // 고정된 Canvas 너비
        private double _canvasHeight = 400;  // 고정된 Canvas 높이
        private double _cellSize;  // 셀 크기 (동적으로 계산)

        public MainWindow()
        {
            InitializeComponent();
            BatteryCanvas.Width = _canvasWidth;
            BatteryCanvas.Height = _canvasHeight;
            LoadBatteryTray(); // 배터리 배열 초기 로드
        }

        // 배터리 상태 그리기 (행렬로 배터리 표시)
        private void LoadBatteryTray()
        {
            BatteryCanvas.Children.Clear(); // 이전 배터리 도형 삭제

            // 셀 크기 계산 (Canvas의 크기와 행/열에 따라 자동 조정)
            double cellWidth = BatteryCanvas.Width / _columns;
            double cellHeight = BatteryCanvas.Height / _rows;
            _cellSize = Math.Min(cellWidth, cellHeight); // 셀의 크기는 행렬에 맞게 계산

            // 배터리 정보 리스트 생성 (임의로 상태 설정)
            List<Battery> batteryList = new List<Battery>();
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    batteryList.Add(new Battery
                    {
                        ID = row * _columns + col + 1,
                        X = col * (_cellSize + 5), // 각 배터리의 X 위치
                        Y = row * (_cellSize + 5), // 각 배터리의 Y 위치
                        IsDefective = (row + col) % 2 == 0 // 임의로 불량 상태 설정
                    });
                }
            }

            // 배터리 상태에 따라 색상 변경 (불량은 빨강, 정상은 초록)
            foreach (var battery in batteryList)
            {
                Ellipse batteryShape = new Ellipse
                {
                    Width = _cellSize,
                    Height = _cellSize,
                    Fill = battery.IsDefective ? Brushes.Red : Brushes.Green,
                    ToolTip = $"배터리 ID: {battery.ID}"
                };

                Canvas.SetLeft(batteryShape, battery.X);
                Canvas.SetTop(batteryShape, battery.Y);
                BatteryCanvas.Children.Add(batteryShape);
            }

            // 그룹박스 크기 조정 (배터리 캔버스 크기에 맞게)
            AdjustGroupBoxSize();
        }

        // 그룹박스 크기 조정
        private void AdjustGroupBoxSize()
        {
            // 창의 크기를 기준으로 그룹박스 크기 조정
            double maxHeight = this.ActualHeight - 20; // 창의 높이에 여백 추가
            double groupBoxWidth = this.ActualWidth / 2 - 20; // 창의 절반을 그룹박스의 너비로 설정
            double groupBoxHeight = Math.Min(maxHeight, _rows * _cellSize + (_rows - 1) * 5); // 세로 크기: 행렬에 따라

            // 그룹박스 크기 설정
            if (groupBoxWidth < 0) groupBoxWidth = 0;
            if (groupBoxHeight < 0) groupBoxHeight = 0;

            BatteryGroupBox.Width = groupBoxWidth;
            BatteryGroupBox.Height = groupBoxHeight;

            BatteryCanvas.Width = groupBoxWidth;
            BatteryCanvas.Height = groupBoxHeight;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
        }

        // 창 크기 변경 시 그룹박스 크기 업데이트
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustGroupBoxSize(); // 창 크기 변경 시 그룹박스 크기 조정
            LoadBatteryTray(); // 배터리 상태 갱신
        }

        // 배터리 배열 갱신 버튼 클릭 이벤트
        private void UpdateBatteryArrayButton_Click(object sender, RoutedEventArgs e)
        {
            // 행과 열의 개수 받아오기
            if (int.TryParse(RowsTextBox.Text, out int rows) && int.TryParse(ColumnsTextBox.Text, out int columns))
            {
                _rows = rows;
                _columns = columns;
                LoadBatteryTray(); // 갱신된 행렬 크기 기준으로 배터리 배열 갱신
            }
            else
            {
                MessageBox.Show("유효한 숫자를 입력해주세요.");
            }
        }
    }

    // 배터리 클래스
    public class Battery
    {
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsDefective { get; set; }
    }
}