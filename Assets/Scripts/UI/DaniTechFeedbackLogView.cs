using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EmeToDia.Gameplay
{
    // 아이템 획득, 사용 성공/실패, 상태 변화 확인 메시지를 표시하는 로그 View입니다.
    public sealed class DaniTechFeedbackLogView : MonoBehaviour
    {
        [SerializeField] private Text _logText;
        [SerializeField] private int _maxLineCount = 7;

        private readonly List<string> _logs = new List<string>();

        public void Clear()
        {
            _logs.Clear();
            RefreshLogText();
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            _logs.Add(message);
            while (_logs.Count > _maxLineCount)
            {
                _logs.RemoveAt(0);
            }

            RefreshLogText();
        }

        private void RefreshLogText()
        {
            if (_logText == null)
            {
                return;
            }

            _logText.text = string.Join("\n", _logs);
        }
    }
}
