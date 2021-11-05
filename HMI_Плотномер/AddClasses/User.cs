using System;
using System.Collections.Generic;
using System.Text;

namespace HMI_Плотномер.AddClasses
{
    class User: PropertyChangedBase
    {
        public long Id { get; set; }
        #region Логин пользователя
        private string login;
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get => login; set => Set(ref login, value); }
        #endregion
        #region Имя пользователя
        private string name="name";
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get => name; set => Set(ref name, value); }
        #endregion
        #region Фамилия пользователя
        private string somename="someneme";
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Somename { get => somename; set => Set(ref somename, value); }
        #endregion
        #region Должность пользователя
        private string post="none";
        /// <summary>
        /// Должность пользователя
        /// </summary>
        public string Post { get => post; set => Set(ref post, value); }
        #endregion
        #region Пароль
        private string password="admin";
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get => password; set => Set(ref password, value); }
        #endregion
        #region Уровень доступа
        private string level = "Администратор";
        public string Level { get => level; set => Set(ref level, value); }
        #endregion
    }
}
