using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Web/mobile compatibility layer for the CMU ETC Board API used by Ghost Maid.
// It intentionally keeps the original Board.Input namespace so existing gameplay
// scripts can compile without the Windows-only fun.board package.
namespace Board.Input
{
    public enum BoardContactType
    {
        Finger,
        Glyph
    }

    public enum BoardContactPhase
    {
        Began,
        Moved,
        Stationary,
        Ended,
        Canceled
    }

    public struct BoardContact
    {
        public BoardContactType type;
        public BoardContactPhase phase;
        public Vector2 screenPosition;
        public int glyphId;
        public float orientation;
    }

    public static class BoardInput
    {
        // Glyph IDs match the original ETC Board project:
        // 1 bag, 2 dryer, 3 scrub, 4 magnifier.
        public static int SelectedGlyphId { get; private set; } = 3;
        public static float OrientationRadians { get; private set; } = 0f;
        public static bool IsPointerFallback => true;

        private static bool pointerWasDown;
        private static Vector2 lastPosition;
        private static float suppressPointerUntil;
        private static readonly List<BoardContact> contacts = new List<BoardContact>(1);

        public static void SelectGlyph(int glyphId)
        {
            SelectedGlyphId = Mathf.Clamp(glyphId, 1, 4);
            suppressPointerUntil = Time.unscaledTime + 0.12f;
        }

        public static void Rotate(float deltaDegrees)
        {
            OrientationRadians += deltaDegrees * Mathf.Deg2Rad;
        }

        public static IReadOnlyList<BoardContact> GetActiveContacts()
        {
            return BuildContacts(BoardContactType.Glyph);
        }

        public static IReadOnlyList<BoardContact> GetActiveContacts(BoardContactType requestedType)
        {
            return BuildContacts(requestedType);
        }

        private static IReadOnlyList<BoardContact> BuildContacts(BoardContactType requestedType)
        {
            contacts.Clear();
            HandleKeyboard();

            bool down = false;
            bool pressed = false;
            bool released = false;
            Vector2 position = lastPosition;

            if (Mouse.current != null)
            {
                position = Mouse.current.position.ReadValue();
                down = Mouse.current.leftButton.isPressed;
                pressed = Mouse.current.leftButton.wasPressedThisFrame;
                released = Mouse.current.leftButton.wasReleasedThisFrame;

                float scroll = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    Rotate(Mathf.Sign(scroll) * 8f);
                }
            }

            // Touchscreen fallback. On Web this also makes the adaptation usable on
            // touch-capable laptops/tablets without scene-specific changes.
            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;
                if (touch.press.isPressed || touch.press.wasPressedThisFrame || touch.press.wasReleasedThisFrame)
                {
                    position = touch.position.ReadValue();
                    down = touch.press.isPressed;
                    pressed = touch.press.wasPressedThisFrame;
                    released = touch.press.wasReleasedThisFrame;
                }
            }

            // Don't let the click used to select the on-screen toolbar clean the room too.
            if (IsPointerOverToolbar(position) || Time.unscaledTime < suppressPointerUntil)
            {
                pointerWasDown = down;
                lastPosition = position;
                return contacts;
            }

            BoardContactPhase? phase = null;
            if (pressed || (down && !pointerWasDown))
            {
                phase = BoardContactPhase.Began;
            }
            else if (down)
            {
                phase = Vector2.SqrMagnitude(position - lastPosition) > 0.25f
                    ? BoardContactPhase.Moved
                    : BoardContactPhase.Stationary;
            }
            else if (released || (!down && pointerWasDown))
            {
                phase = BoardContactPhase.Ended;
            }

            if (phase.HasValue)
            {
                contacts.Add(new BoardContact
                {
                    type = requestedType,
                    phase = phase.Value,
                    screenPosition = position,
                    glyphId = requestedType == BoardContactType.Finger ? 0 : SelectedGlyphId,
                    orientation = OrientationRadians
                });
            }

            pointerWasDown = down;
            lastPosition = position;
            return contacts;
        }


        private static bool IsPointerOverToolbar(Vector2 position)
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene != "GameScene" && scene != "TutorialScene") return false;

            float scale = Mathf.Clamp(Screen.width / 1440f, 0.75f, 1.25f);
            float toolbarWidth = 520f * scale;
            float toolbarHeightFromBottom = (62f + 12f) * scale;
            float left = (Screen.width - toolbarWidth) * 0.5f;
            float right = left + toolbarWidth;
            return position.x >= left && position.x <= right && position.y <= toolbarHeightFromBottom;
        }

        private static void HandleKeyboard()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame) SelectGlyph(1);
            if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame) SelectGlyph(2);
            if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame) SelectGlyph(3);
            if (keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame) SelectGlyph(4);

            if (keyboard.qKey.isPressed) Rotate(-90f * Time.unscaledDeltaTime);
            if (keyboard.eKey.isPressed) Rotate(90f * Time.unscaledDeltaTime);
        }
    }

    // Adds a minimal tool picker without requiring edits to the existing Unity scenes.
    // It only appears in TutorialScene and GameScene.
    public sealed class WebToolOverlay : MonoBehaviour
    {
        private const float PanelWidth = 520f;
        private const float PanelHeight = 62f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureOverlay()
        {
            if (Object.FindFirstObjectByType<WebToolOverlay>() != null) return;
            var go = new GameObject("Web Tool Controls");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<WebToolOverlay>();
        }

        private void OnGUI()
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene != "GameScene" && scene != "TutorialScene") return;

            float scale = Mathf.Clamp(Screen.width / 1440f, 0.75f, 1.25f);
            Matrix4x4 oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);

            float logicalWidth = Screen.width / scale;
            float logicalHeight = Screen.height / scale;
            Rect box = new Rect((logicalWidth - PanelWidth) * 0.5f, logicalHeight - PanelHeight - 12f, PanelWidth, PanelHeight);
            GUI.Box(box, "Tools — click & drag to use   •   Q/E or mouse wheel rotates");

            DrawToolButton(new Rect(box.x + 10, box.y + 27, 115, 28), 1, "1  Bag");
            DrawToolButton(new Rect(box.x + 135, box.y + 27, 115, 28), 2, "2  Dryer");
            DrawToolButton(new Rect(box.x + 260, box.y + 27, 115, 28), 3, "3  Scrub");
            DrawToolButton(new Rect(box.x + 385, box.y + 27, 125, 28), 4, "4  Magnify");

            GUI.matrix = oldMatrix;
        }

        private static void DrawToolButton(Rect rect, int glyphId, string label)
        {
            bool selected = BoardInput.SelectedGlyphId == glyphId;
            string text = selected ? "● " + label : label;
            if (GUI.Button(rect, text)) BoardInput.SelectGlyph(glyphId);
        }
    }
}
