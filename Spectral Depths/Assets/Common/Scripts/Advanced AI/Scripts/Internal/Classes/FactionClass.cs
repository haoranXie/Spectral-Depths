namespace EmeraldAI
{
    [System.Serializable]
    public class FactionClass
    {
        public int FactionIndex;
        public RelationTypes RelationType;

        public FactionClass(int m_FactionIndex, int m_RelationType)
        {
            FactionIndex = m_FactionIndex;
            RelationType = (RelationTypes)m_RelationType;
        }
    }
}