using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;

namespace UI.Part
{
    public class SpeechBubble : PartWorld<SpeechBubble.Param>, InputManager.IKeyListener, Typer.IListener
    {
        public new class Param : PartWorld<SpeechBubble.Param>.Param
        {
            public IListener IListener { get; private set; } = null;
            public string Text { get; private set; } = string.Empty;
            public bool IsAppearEffect { get; private set; } = false;

            public Param SetIListener(IListener iListener)
            {
                IListener = iListener;
                return this;
            }
            
            public Param SetText(string text)
            {
                Text = text;
                return this;
            }

            public Param SetIsAppearEffect(bool isAppearEffect)
            {
                IsAppearEffect = isAppearEffect;
                return this;
            }
        }

        public interface IListener
        {
            void End();
        }
        
        [SerializeField] private Typer typer = null;

        private bool _isEnd = false;

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            
            typer?.Initialize(this);

            await UniTask.CompletedTask;
        }

        public override async UniTask BeforeActivateAsync()
        {
            _isEnd = false;
            
            typer?.TypeTextAsync(_param.Text);
            Manager.Get<IInput>()?.AddListener(this);

            if (_param.IsAppearEffect) 
                AppearEffectAsync().Forget();

            await UniTask.CompletedTask;
        }

        public override UniTask AfterActivateAsync()
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            Manager.Get<IInput>()?.RemoveListener(this);
        }
        
        #region InputManager.IListener

        void InputManager.IKeyListener.OnKey(KeyCode keyCode)
        {
            
        }

        void InputManager.IKeyListener.OnKeyDown(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Space)
            {
                if (_isEnd)
                {
                    _param?.IListener?.End();
                    return;
                }
                
                _isEnd = true;
                typer?.End(_param.Text);
            }
        }
        #endregion
        
        #region Typer.IListener

        void Typer.IListener.End()
        {
            _isEnd = true;
        }
        #endregion
    }
}

