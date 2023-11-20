Assets/Develop/Data/SoundTable/Resources/SoundTable 폴더에 각종 분류에 따른 sound table이 있음.
git conflit 방지와 유지보수를 위해 sound table을 나누어놨음.
sound table을 sound table set 에 등록하여 사용해야함

GameSetting.cs 에서 Init 하고 있음. 싱글톤으로 동작함.

에셋을 table에 등록하는 작업을 마쳤다면 언제든지 간편하게 사운드를 가져오고, 플레이할 수 있음.
예제 코드는 SoundExample.cs를 참고.

Schedule 함수 사용할 때, 여러 설정값 들을 반영하여 실행시키고 싶다면 SoundCommand.cs에서 확장가능.
 