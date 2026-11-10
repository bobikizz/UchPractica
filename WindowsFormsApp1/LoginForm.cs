using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsFormsApp1
{
    public partial class LoginForm : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\3 курс\практика\Галичанин\WindowsFormsApp1\inostr.accdb;";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // 1. Проверяем, что поля не пустые
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Пожалуйста, заполните логин и пароль!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Создаем подключение к MS Access
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Запрос выбирает ID пользователя и флаг админа, если логин и пароль совпадают
                string query = "SELECT UserID, IsAdmin FROM Users WHERE Username = @user AND PasswordHash = @pass";

                OleDbCommand command = new OleDbCommand(query, connection);

                // Безопасно передаем параметры из текстовых полей, защищая от SQL-инъекций
                command.Parameters.AddWithValue("@user", txtUsername.Text.Trim());
                command.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());

                try
                {
                    connection.Open();

                    // Выполняем запрос и считываем результат
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Если строка нашлась — пользователь существует
                        {
                            int userId = Convert.ToInt32(reader["UserID"]);
                            bool isAdmin = Convert.ToBoolean(reader["IsAdmin"]);

                            MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 3. Распределяем пользователей по ролям
                            if (isAdmin)
                            {
                                MessageBox.Show("Добро пожаловать в панель администратора!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                AdminForm adminForm = new AdminForm();
                                adminForm.Show();
                            }
                            else
                            {
                                MessageBox.Show($"Добро пожаловать, пользователь!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                UserForm userForm = new UserForm(userId);
                                userForm.Show();
                            }

                            this.Hide(); // Скрываем форму авторизации

                        }
                        else
                        {
                            // Если совпадений в базе нет
                            MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // На случай, если забыли закрыть Access или путь к базе указан неверно
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Проверяем, что поля заполнены
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Заполните логин и пароль для регистрации!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // 2. Сначала проверяем, не занят ли такой логин
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @user";
                OleDbCommand checkCommand = new OleDbCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@user", txtUsername.Text.Trim());

                try
                {
                    connection.Open();
                    int userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (userCount > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3. Если логин свободен, добавляем нового пользователя
                    // Поле IsAdmin по умолчанию ставим в False (0)
                    string insertQuery = "INSERT INTO Users (Username, PasswordHash, DateRegistered, IsAdmin) VALUES (@user, @pass, @date, False)";
                    OleDbCommand insertCommand = new OleDbCommand(insertQuery, connection);

                    insertCommand.Parameters.AddWithValue("@user", txtUsername.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@pass", txtPassword.Text.Trim()); // Пока храним обычный текст
                    insertCommand.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Регистрация успешно завершена! Теперь вы можете войти.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при регистрации: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
