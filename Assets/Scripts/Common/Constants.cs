using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common
{
    public class Constants
    {
        #region AnimatorFlags
        public const string IsMoving = "IsMoving";
        public const string IsDead = "IsDead";
        public const string IsUnderground = "IsUnderground";
        public const string Attack = "Attack";
        public const string ProjectileAttack = "ProjectileAttack";
        public const string Cast = "Cast";
        public const string Pounce = "Pounce";
        public const string TakeDamage = "TakeDamage";
        public const string Heal = "Heal";
        public const string Buff = "Buff";
        public const string DeBuff = "DeBuff";
        #endregion

        #region ScreenCapture
        public const string CapturePath = @"C:\Pictures\MonsterMash";
        public const string Prefix = "Screenshot_";
        public const string FileFormat = ".png";
        public const string DateFormat = "mmddyyyymmssff";
        public const string UI = "ui";
        #endregion
    }
}
