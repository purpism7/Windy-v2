using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameSystem
{
    public class DayNightCycle : MonoBehaviour
    {
        [SerializeField] private Light2D light2d = null;
        [SerializeField] private TextMeshProUGUI timeTMP = null;
        
        private float _dayLenght = 60f * 10f * 1f; // 하루의 길이(초).
        private float _dayIntensity = 1f;
        private float _nightIntensity = 0.4f;

        private Color _dayColor = Color.white;
        private Color _nightColor = new Color(30 / 255f, 130 / 255f, 255 / 255f);
        
        private float _timeOfDay = 0.55f; // 현재 시간 (0 - 1 범위로, 0은 낮 시작, 1은 다시 밤 시작)
        private int _hours = 0;
        
        public bool IsNight
        {
            get { return _hours >= 18 && _hours < 24 || _hours >= 0 && _hours < 6; }
        }

        public void ChainUpdate()
        {
            if (light2d == null)
                return;

            _timeOfDay += Time.deltaTime / _dayLenght;
            if (_timeOfDay > 1f)
                _timeOfDay = 0;

            UpdateLighting();
            UpdateTime();
        }

        private void UpdateLighting()
        {
            if (light2d == null)
                return;
            
            // float time = 0;
            // if (_timeOfDay < 0.5f) // 낮
            //     // time = _timeOfDay * 2f;
            // else // 밤
                // time = (_timeOfDay - 0.5f) * 2;

            ChangeToDay();
            ChangeToNight();
        }

        private void ChangeToDay()
        {
            float start = 0.45f;
            float transitionRange = 0.05f;
            
            if (_timeOfDay > start && _timeOfDay < start + transitionRange)
            {
                var time = (_timeOfDay - start) / 0.1f;
                
                LerpLight(_nightIntensity, _dayIntensity, _nightColor, _dayColor, time);
            }
        }

        private void ChangeToNight()
        {
            var start = 0.95f;
            var transitionRange = 0.05f;
            float time = 0;
            
            if (_timeOfDay > start || _timeOfDay < transitionRange)
            {
                if (_timeOfDay >= start)
                {
                    // 0.9 ~ 1.0 → 0.0 ~ 0.5
                    time = (_timeOfDay - start) / transitionRange * 0.5f;
                }
                else if (_timeOfDay <= transitionRange)
                {
                    // 0.0 ~ 0.1 → 0.5 ~ 1.0
                    time = 0.5f + (_timeOfDay / transitionRange * 0.5f);
                }
                
                LerpLight(_dayIntensity, _nightIntensity, _dayColor, _nightColor, time);
            }
        }

        private void LerpLight(float startIntensity, float endIntensity, Color startColor, Color endColor, float time)
        {
            light2d.intensity = Mathf.Lerp(startIntensity, endIntensity, time); 
            light2d.color = Color.Lerp(startColor, endColor, time); 
        }

        private void UpdateTime()
        {
            string dayNight = IsNight ? "Night" : "Day";

            float time = _timeOfDay * 24f;
            _hours = Mathf.FloorToInt(time);
            string hoursText = _hours < 10 ? $"0{_hours}" : $"{_hours}";
            
            int minutes = (int)((time - _hours) * 60);
            string minutesText = minutes < 10 ? $"0{minutes}" : $"{minutes}";
            
            int seconds = (int)(((time - _hours) * 60 - minutes) * 60f);
            string secondsText = seconds < 10 ? $"0{seconds}" : $"{seconds}";
            
            timeTMP?.SetText( $"{dayNight} {hoursText}:{minutesText}:{secondsText}");
        }
    }
}    

