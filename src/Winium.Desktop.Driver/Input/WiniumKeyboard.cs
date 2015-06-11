﻿namespace Winium.Desktop.Driver.Input
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using WindowsInput.Native;

    using OpenQA.Selenium;

    using Winium.Cruciatus;
    using Winium.Cruciatus.Core;
    using Winium.Cruciatus.Settings;

    #endregion

    class WiniumKeyboard
    {
        #region Private Fields
        
        private readonly KeyboardModifiers modifiers = new KeyboardModifiers();

        #endregion

        #region Private Static Fields

        private static volatile WiniumKeyboard instance;

        #endregion

        #region Public Methods and Operators

        public void SendKeys(char[] keysToSend)
        {
            List<KeyEvent> builder = keysToSend.Select(key => new KeyEvent(key)).ToList();

            this.SendKeys(builder);
        }

        public void KeyDown(string keyToPress)
        {
            var key = KeyboardModifiers.GetVirtualKeyCode(keyToPress);
            this.modifiers.Add(keyToPress);
            CruciatusFactory.Keyboard.KeyDown(key);
        }

        public void KeyUp(string keyToRelease)
        {
            var key = KeyboardModifiers.GetVirtualKeyCode(keyToRelease);
            this.modifiers.Remove(keyToRelease);
            CruciatusFactory.Keyboard.KeyUp(key);
        }

        #endregion

        #region Public Static Methods and Operators

        public static WiniumKeyboard GetInstance()
        {
            return instance ?? (instance = new WiniumKeyboard());
        }

        #endregion

        #region Protected Methods and Operators

        protected void ReleaseModifiers()
        {
            var tmp = this.modifiers.ToList();

            foreach (var modifierKey in tmp)
            {
                this.KeyUp(modifierKey);
            }
        }

        #endregion

        #region Private Methods and Operators

        private void SendKeys(List<KeyEvent> events)
        {
            foreach (var keyEvent in events)
            {
                if (keyEvent.IsNewLine())
                {
                    CruciatusFactory.Keyboard.SendEnter();
                }
                else if (keyEvent.IsModifierRelease())
                {
                    this.ReleaseModifiers();
                }
                else if (keyEvent.IsModifier())
                {
                    this.PressOrReleaseModifier(keyEvent.GetKey());
                }
                else
                {
                    this.Type(keyEvent.GetCharacter());
                }
            }
        }

        private void Type(char key)
        {
            String str = Convert.ToString(key);

            if (this.modifiers.Contains(Keys.LeftShift) || this.modifiers.Contains(Keys.Shift))
            {
                str = str.ToUpper();
            }

            CruciatusFactory.Keyboard.SendText(str);
        }

        private void PressOrReleaseModifier(string modifier)
        {
            if (this.modifiers.Contains(modifier))
            {
                this.KeyUp(modifier);
            }
            else
            {
                this.KeyDown(modifier);
            }
        }

        #endregion
    }
}
