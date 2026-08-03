using UnityEngine;

namespace SeriousGameBPD.Data
{
    [CreateAssetMenu(menuName = "SeriousGame BPD/Glossary Entry", fileName = "GlossaryEntry")]
    public class GlossaryEntry : ScriptableObject
    {
        public string title;
        [TextArea(3, 10)] public string body;
        public EducationalTag relatedTag;
    }
}
