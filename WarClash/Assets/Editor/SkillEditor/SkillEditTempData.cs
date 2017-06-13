using Logic.Skill;
using UnityEngine;
using UnityEditor;

public class SkillEditTempData
{
    public static object copyItem;
    public static TimeLineGroup editingSkill;
    public static object editingItem;
    public static object editingItemCache;
    public static Texture2D settingTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Image/setting.png");
}
