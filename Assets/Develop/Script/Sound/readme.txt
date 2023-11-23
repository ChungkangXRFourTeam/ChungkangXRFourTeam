Assets/Develop/Data/SoundTable/Resources/SoundTable 폴더에 각종 분류에 따른 sound table이 있음.
git conflit 방지와 유지보수를 위해 sound table을 나누어놨음.
sound table을 sound table set 에 등록하여 사용해야함

GameSetting.cs 에서 Init 하고 있음. 싱글톤으로 동작함.

에셋을 table에 등록하는 작업을 마쳤다면 언제든지 간편하게 사운드를 가져오고, 플레이할 수 있음.
예제 코드는 SoundExample.cs를 참고.

Schedule 함수 사용할 때, 여러 설정값 들을 반영하여 실행시키고 싶다면 SoundCommand.cs에서 확장가능.

11.23 추가

인게임 볼륨에 맞추어 소리 크기를 조절 할 수 있는 기능을 추가 하였음.
Volume 값은 인게임에서 조절하고 PlayerPref로 저장함.
실시간 저장이 아닌, 볼륨을 조절하고 패널에 나왔을 때 저장하도록 되어있음.
해당 사용 예시도 기존 SoundExample.cs를 수정하였으니 참고