# NI 카드를 이용한 PWM 생성 및 측정 프로젝트

## 설명
이 프로젝트는 National Instruments (NI) 하드웨어와 Measurement & Automation Explorer (MAX)를 사용하여 PWM(Pulse Width Modulation) 신호를 생성하고 측정하는 방법을 보여줍니다. 이 시스템은 사용자가 입력한 주기, 듀티 사이클, 전압 값에 따라 PWM 신호를 생성하고, 아날로그 입력(AI) 채널을 통해 측정된 PWM 신호를 실시간으로 그래프에 표시합니다. 또한, 특정 주기의 데이터를 캡처하여 자세히 분석할 수 있습니다.

## 기능
- 사용자가 설정한 주기와 듀티 사이클에 따라 PWM 신호 생성 (AO)
- 설정된 전압 수준으로 PWM 신호 생성 (High Voltage 및 Low Voltage)
- 주기 및 듀티 사이클의 범위 확인 (주기: 100ms ~ 2s, 듀티: 10% ~ 90%)
- 생성된 PWM 신호를 실시간 그래프로 표시 (Continuous Graph)
- 현재 그래프의 데이터를 캡처하여 별도 그래프로 표시 (Capture Graph)
- 오실로스코프를 사용하여 생성된 PWM 신호의 시각적 비교

## UI
### [Operation]

<img width="500" alt="2" src= "https://github.com/user-attachments/assets/1bda0d3e-10f8-4706-9f25-ae2cbfafee9e">

### [Coverage error handling]

<img width="500" alt="2" src= "https://github.com/user-attachments/assets/111de6fc-041e-4f76-a774-c4f0567e6576">

## HardWare Setting
<img width="500" alt="2" src= "https://github.com/user-attachments/assets/1a160ba0-8296-4879-b0b0-7dd76b732fe6">

<img width="500" alt="2" src= "https://github.com/user-attachments/assets/ec5136ba-c9e1-4e02-afd9-3699ffb34f77">

## 설치 방법
1. 이 저장소를 클론합니다:
   ```bash
   git clone https://github.com/your-username/ni-pwm-project.git
   ```

2. Visual Studio에서 프로젝트를 엽니다.
3. 프로젝트를 빌드하고 실행합니다.

## 사용 방법
1. 주기, 듀티 사이클, High/Low 전압을 설정합니다.
2. `Apply` 버튼을 눌러 설정을 적용합니다.
3. 스위치를 켜서 PWM 신호를 생성합니다.
4. `Capture` 버튼을 눌러 현재 그래프의 데이터를 캡처하여 별도 그래프에 표시합니다.
5. 오실로스코프를 사용하여 실시간 신호를 확인하고, 그래프와 비교합니다.

## 참고 문서
- [NI 공식 문서](https://www.ni.com)
- [Measurement & Automation Explorer (MAX) 사용법](https://www.ni.com/ko-kr/support/downloads/software-products/download.ni-measurement-automation-explorer-(max).html)
- [PWM의 이해와 활용](https://your-document-link)

### 추가 정보
- 이 프로젝트는 C# 및 .NET Framework를 사용하여 개발되었습니다.
- NI DAQmx 라이브러리를 통해 NI 하드웨어와 통신합니다.
- 모든 설정 값은 UI를 통해 실시간으로 조정 가능하며, 사용자 친화적인 인터페이스를 제공합니다.

이와 같은 README 파일을 통해 프로젝트의 주요 기능과 설치 방법을 명확하게 전달할 수 있습니다. 사용자는 제공된 설명에 따라 프로젝트를 설치하고, 제공된 예제 코드를 기반으로 NI 하드웨어를 제어하고 데이터를 시각화하는 방법을 쉽게 이해할 수 있습니다.
