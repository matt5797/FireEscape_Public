
/*
 * 외부 정보 시스템과의 상호작용
 */

namespace FireEscape.Object
{
    interface IInformationInterface
    {
        /// <summary>
        /// LevelObject의 이름과 추가 교육 정보를 반환
        /// </summary>
        public string GetInfo();

        /// <summary>
        /// LevelObject의 이름과 추가 교육 정보를 세팅
        /// </summary>
        public void SetInfo(string name);
    }
    
}
