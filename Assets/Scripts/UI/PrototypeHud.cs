using UnityEngine;
using SeasOfLegends.Combat;

namespace SeasOfLegends.UI
{
    /// <summary>
    /// Immediate-mode prototype HUD. It is intentionally asset-free and can be replaced with a
    /// production UI Toolkit or uGUI presentation while preserving Combatant health contracts.
    /// </summary>
    public sealed class PrototypeHud : MonoBehaviour
    {
        private Combatant player;
        private Combatant enemy;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;

        public void ConfigureForPrototype(Combatant playerCombatant, Combatant enemyCombatant)
        {
            player = playerCombatant;
            enemy = enemyCombatant;
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawBackdrop();
            DrawHealthBar(new Rect(28f, 26f, 310f, 24f), "TIDE WARDEN", player, new Color(0.11f, 0.7f, 0.95f));
            DrawHealthBar(new Rect(Screen.width - 338f, 26f, 310f, 24f), "CRIMSON RAIDER", enemy, new Color(0.91f, 0.19f, 0.22f));
            GUI.Label(new Rect(0f, 72f, Screen.width, 28f), "STARTER ISLAND • DEFEAT THE CRIMSON RAIDER", titleStyle);
            GUI.Label(new Rect(0f, Screen.height - 42f, Screen.width, 28f), "WASD move  •  Mouse look  •  Space jump  •  Shift dash  •  Left click attack  •  Right click block", labelStyle);

            if (enemy != null && enemy.IsDefeated)
                GUI.Label(new Rect(0f, Screen.height * 0.42f, Screen.width, 44f), "ENCOUNTER COMPLETE", titleStyle);
        }

        private void DrawBackdrop()
        {
            GUI.color = new Color(0f, 0.04f, 0.09f, 0.78f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, 104f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0f, Screen.height - 58f, Screen.width, 58f), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private void DrawHealthBar(Rect rect, string label, Combatant combatant, Color color)
        {
            GUI.Label(new Rect(rect.x, rect.y - 22f, rect.width, 20f), label, labelStyle);
            GUI.color = new Color(0f, 0f, 0f, 0.68f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            float normalized = combatant == null ? 0f : Mathf.Clamp01(combatant.CurrentHealth / combatant.MaximumHealth);
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.x + 3f, rect.y + 3f, (rect.width - 6f) * normalized, rect.height - 6f), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private void EnsureStyles()
        {
            if (titleStyle != null) return;
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                normal = { textColor = new Color(0.84f, 0.92f, 0.98f) }
            };
        }
    }
}
