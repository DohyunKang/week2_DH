using NationalInstruments.Analysis;
using NationalInstruments.Analysis.Conversion;
using NationalInstruments.Analysis.Dsp;
using NationalInstruments.Analysis.Dsp.Filters;
using NationalInstruments.Analysis.Math;
using NationalInstruments.Analysis.Monitoring;
using NationalInstruments.Analysis.SignalGeneration;
using NationalInstruments.Analysis.SpectralMeasurements;
using NationalInstruments;
using NationalInstruments.UI;
using NationalInstruments.DAQmx;
using NationalInstruments.NI4882;
using NationalInstruments.VisaNS;
using NationalInstruments.NetworkVariable;
using NationalInstruments.NetworkVariable.WindowsForms;
using NationalInstruments.Tdms;
using NationalInstruments.UI.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        // 멤버 변수 선언
        private Task aoTask;  // AO 작업을 위한 멤버 변수 선언
        private Task aiTask;  // AI 작업을 위한 멤버 변수 선언
        private AnalogSingleChannelWriter writer;
        private AnalogSingleChannelReader reader;
        private double pwmPeriod;
        private double pwmDutyCycle;
        private double highVoltage;
        private double lowVoltage;
        private System.Timers.Timer graphUpdateTimer;
        private bool isPwmRunning = false;

        // UI 라벨 컨트롤
        private Label lblPeriod;
        private Label lblFrequency;
        private Label lblDuty;

        private double tempDutyCycle;
        private double tempHighVoltage;
        private double tempLowVoltage;
        private double tempPeriod;

        public Form1()
        {
            InitializeComponent();
        }

        private void SetUpAnalogOutput(string channelName, double highVoltage)
        {
            try
            {
                // AO 작업 생성
                aoTask = new Task();

                // 아날로그 출력 채널 생성, 여기서 highVoltage는 최대 전압 범위로 사용됨
                aoTask.AOChannels.CreateVoltageChannel(channelName, "AO Channel", 0, highVoltage, AOVoltageUnits.Volts);

                // AO 스트림에 대한 Writer 설정
                writer = new AnalogSingleChannelWriter(aoTask.Stream);

                // 설정한 전압(highVoltage)을 AO로 출력
                writer.WriteSingleSample(true, highVoltage);

                MessageBox.Show("아날로그 출력 채널 '{channelName}'에서 {highVoltage}V 출력 중", "출력 확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DaqException ex)
            {
                // 예외 발생 시 메시지 박스를 통해 오류 메시지 표시
                MessageBox.Show("아날로그 출력 설정 중 오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // 애플리케이션 종료
            }
        }

        private void SetUpAnalogInput(string channelName)
        {
            try
            {
                aiTask = new Task(); // AI 작업 생성
                aiTask.AIChannels.CreateVoltageChannel(channelName, "AI Channel", AITerminalConfiguration.Rse, 0, 10, AIVoltageUnits.Volts);
                reader = new AnalogSingleChannelReader(aiTask.Stream); // AI 스트림에 대한 Reader 설정
            }
            catch (DaqException ex)
            {
                // 예외 발생 시 메시지 박스를 통해 오류 메시지 표시
                MessageBox.Show("아날로그 입력 설정 중 오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // 애플리케이션 종료
            }
        }

        public double ReadAnalogInput(string channelName)
        {
            try
            {
                if (aiTask == null)
                {
                    aiTask = new Task(); // AI 작업 생성
                    aiTask.AIChannels.CreateVoltageChannel(channelName, "AI Channel", AITerminalConfiguration.Rse, 0, 10, AIVoltageUnits.Volts);
                    reader = new AnalogSingleChannelReader(aiTask.Stream); // AI 스트림에 대한 Reader 설정
                }
                return reader.ReadSingleSample(); // 단일 샘플 읽기
            }
            catch (DaqException ex)
            {
                // 예외 발생 시 예외 메시지를 출력하고 애플리케이션 종료
                MessageBox.Show("아날로그 입력 읽기 중 오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // 애플리케이션 종료
            }

            return 0.0; // 기본값 반환 (예외 발생 시 실행되지 않음)
        }

        private void switch1_StateChanged(object sender, ActionEventArgs e)
        {
            if (switch1.Value) // switch1이 켜졌을 때
            {
                SetUpAnalogOutput("Dev1/ao0", highVoltage); // AO 채널 설정
                SetUpAnalogInput("Dev1/ai0"); // AI 채널 설정

                // PWM 신호 생성 및 출력
                if (!isPwmRunning)
                {
                    isPwmRunning = true;
                    graphUpdateTimer = new System.Timers.Timer(1/pwmPeriod); // 타이머 간격을 주파수 속도ms로 설정
                    graphUpdateTimer.Elapsed += OnGraphUpdateTimerElapsed;
                    graphUpdateTimer.Start();
                }

                // 스위치가 켜졌을 때 라벨에 현재 설정된 PWM 값들을 출력 (`String.Format` 사용)
                lblPeriod.Text = String.Format("{0}", pwmPeriod);
                lblFrequency.Text = String.Format("{0}", 1000/pwmPeriod);
                lblDuty.Text = String.Format("{0}", pwmDutyCycle);

            }
            else // switch1이 꺼졌을 때
            {
                if (aoTask != null)
                {
                    writer.WriteSingleSample(true, 0); // AO 출력 전압을 0으로 설정하여 PWM 중지
                }
                if (graphUpdateTimer != null)
                {
                    graphUpdateTimer.Stop();
                    graphUpdateTimer.Dispose();
                    isPwmRunning = false;
                }

                // 그래프 초기화
                if (ContinuousWfg != null)
                {
                    ContinuousWfg.ClearData(); // 기존 그래프 데이터 초기화
                }
            }
        }

        private void OnGraphUpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // PWM 주기와 듀티 사이클에 따른 신호 생성
            double pwmOn = pwmPeriod * (pwmDutyCycle / 100.0); // 듀티 사이클에 따른 ON 시간 계산
            double pwmOff = pwmPeriod - pwmOn; // OFF 시간은 주기에서 ON 시간을 뺀 값

            double currentTime = DateTime.Now.TimeOfDay.TotalMilliseconds % pwmPeriod; // 현재 시간(ms)을 주기와 모듈로 연산
            double outputVoltage;

            if (currentTime < pwmOn)
            {
                outputVoltage = highVoltage; // ON 상태에서 High Voltage 출력
            }
            else
            {
                outputVoltage = lowVoltage; // OFF 상태에서 Low Voltage 출력
            }

            writer.WriteSingleSample(true, outputVoltage); // AO에 전압 출력

            double aiValue = ReadAnalogInput("Dev1/ai0"); // AI 채널에서 값 읽기
            UpdateGraph(aiValue); // 그래프 업데이트
        }

        private void UpdateGraph(double value)
        {
            // 스레드 안전성을 위한 객체 상태 확인
            if (ContinuousWfg == null || ContinuousWfg.IsDisposed)
            {
                return; // 그래프 객체가 해제된 경우 함수 종료
            }

            if (InvokeRequired)
            {
                try
                {
                    // Invoke 호출을 시도하고, 예외 발생 시 무시
                    Invoke(new Action<double>(UpdateGraph), value);
                }
                catch (ObjectDisposedException)
                {
                    // 그래프가 해제된 경우 예외 무시
                }
                return;
            }

            // 실제 그래프 업데이트
            ContinuousWfg.PlotYAppend(value); // 그래프에 읽은 값 추가
        }

        private void PeriodEdit_AfterChangeValue(object sender, AfterChangeNumericValueEventArgs e)
        {
            tempPeriod = e.NewValue;  // 주기 값 업데이트
        }

        private void DutyEdit_AfterChangeValue(object sender, AfterChangeNumericValueEventArgs e)
        {
            tempDutyCycle = e.NewValue;
        }

        private void HighEdit_AfterChangeValue(object sender, AfterChangeNumericValueEventArgs e)
        {
            tempHighVoltage = e.NewValue;
        }

        private void LowEdit_AfterChangeValue(object sender, AfterChangeNumericValueEventArgs e)
        {
            tempLowVoltage = e.NewValue;
        }

        private void CaptureCurrentWaveform()
        {
            try
            {
                // ContinuousWfg에서 현재 보여지는 데이터 가져오기
                var continuousData = ContinuousWfg.Plots[0].GetYData(); // 첫 번째 플롯의 Y 데이터 가져오기

                // CaptureWfg에 데이터를 복사하여 플롯
                CaptureWfg.PlotY(continuousData);

                // 메시지 박스 표시 (캡처 완료)
                MessageBox.Show("현재 파형이 캡처되었습니다.", "캡처 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // 예외 처리
                MessageBox.Show("파형 캡처 중 오류가 발생했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            // ContinuousWfg의 데이터를 CaptureWfg에 복사
            CaptureCurrentWaveform();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            // Apply 버튼이 눌렸을 때 실제로 값을 적용
            pwmPeriod = tempPeriod;
            pwmDutyCycle = tempDutyCycle;
            highVoltage = tempHighVoltage;
            lowVoltage = tempLowVoltage;

            if (switch1.Value)
            {
                // 설정된 값을 라벨에 표시
                lblPeriod.Text = String.Format("{0}", pwmPeriod);
                lblFrequency.Text = String.Format("{0}", 1000 / pwmPeriod);
                lblDuty.Text = String.Format("{0}", pwmDutyCycle);
            }
        }

        private void ContinuousWfg_PlotDataChanged(object sender, XYPlotDataChangedEventArgs e)
        {
        }

        private void CaptureWfg_PlotDataChanged(object sender, XYPlotDataChangedEventArgs e)
        {
        }
    }
}