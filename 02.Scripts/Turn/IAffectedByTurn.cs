
/*
 * �� �ൿ�� ���� �������̽�
 */
namespace FireEscape.Turn
{
    public interface IAffectedByTurn
    {
        // �� ���۽� ������ �׼�
        public void StartTurn();

        // �� ���� ������ �׼�
        public void DuringTurn();

        // �� ����� ������ �׼�
        public void EndTurn();
   
    }
}
