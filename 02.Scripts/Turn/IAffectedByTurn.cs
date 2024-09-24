
/*
 * 턴 행동에 관한 인터페이스
 */
namespace FireEscape.Turn
{
    public interface IAffectedByTurn
    {
        // 턴 시작시 실행할 액션
        public void StartTurn();

        // 턴 동안 실행할 액션
        public void DuringTurn();

        // 턴 종료시 실행할 액션
        public void EndTurn();
   
    }
}
