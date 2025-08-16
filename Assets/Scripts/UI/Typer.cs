
using System;
using System.Text.RegularExpressions;
using UnityEngine;

using Cysharp.Threading.Tasks;
using TMPro;

using GameSystem;

namespace UI
{
    public class Typer : MonoBehaviour
    {
        public interface IListener
        {
            void End();
        }
        
        [SerializeField] private TextMeshProUGUI tmpTMP = null;

        private IListener _iListener = null;
        private float _typingSpeed = 0.05f;
        private bool _isEnd = false;

        public void Initialize(IListener iListener)
        {
            _iListener = iListener;
        }
        
        public async UniTask TypeTextAsync(string text)
        {
            _isEnd = false;
            tmpTMP?.SetText(string.Empty);
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            
             string currentText = "";
             int realCharIndex = 0;

            // 정규식을 통해 태그와 텍스트 분리
            MatchCollection matches = Regex.Matches(text, @"(<.*?>|[^<])");

            while (realCharIndex < matches.Count)
            {
                string part = matches[realCharIndex].Value;
                currentText += part;

                // 태그는 한 번에 추가하고, 텍스트만 기다림
                if (!Regex.IsMatch(part, @"<.*?>"))
                {
                    // tmpTMP.text = currentText;
                    tmpTMP?.SetText(currentText);
                    await UniTask.Delay(TimeSpan.FromSeconds(_typingSpeed));
                }

                realCharIndex++;
                
                if (_isEnd)
                    return;
            }

            tmpTMP?.SetText(text);
            _iListener?.End();
        }

        public void End(string text)
        {
            _isEnd = true;
            tmpTMP?.SetText(text);
        }
    }
}

