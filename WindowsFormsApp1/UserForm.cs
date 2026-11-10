using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class UserForm : Form
    {
        // Динамический путь к базе данных (работает на любом компьютере)
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inostr.accdb;";
        private DataTable dtWords = new DataTable();
        private int currentWordIndex = 0;
        private int currentUserId;

        public UserForm(int userId)
        {
            InitializeComponent();
            this.currentUserId = userId;
            LoadWordsForTraining();
        }

        // 1. ЗАГРУЗКА СЛОВ С ИХ КАТЕГОРИЯМИ ИЗ БАЗЫ ДАННЫХ
        private void LoadWordsForTraining()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Запрос связывает три таблицы, чтобы достать иностранное слово, перевод и категорию
                string query = @"
                    SELECT 
                        W.ForeighWord, 
                        W.Translation, 
                        C.CategoryName
                    FROM (Words W
                    LEFT JOIN WordCategories WC ON W.WordID = WC.WordID)
                    LEFT JOIN Categories C ON WC.CategoryID = C.CategoryID";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);

                try
                {
                    adapter.Fill(dtWords);
                    ShowWord();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки слов для тренировки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 2. ОТОБРАЖЕНИЕ ТЕКУЩЕГО СЛОВА НА ЭКРАНЕ
        private void ShowWord()
        {
            // Проверяем, есть ли слова в базе вообще
            if (dtWords.Rows.Count == 0)
            {
                lblForeignWord.Text = "В словаре пока нет слов!";
                if (lblCategory != null) lblCategory.Text = "";
                if (lblProgress != null) lblProgress.Text = "";
                txtUserAnswer.Enabled = false;
                btnCheck.Enabled = false;
                btnNext.Enabled = false;
                return;
            }

            // ПРОВЕРКА: Если индекс превысил количество строк, значит слова ЗАКОНЧИЛИСЬ
            if (currentWordIndex >= dtWords.Rows.Count)
            {
                lblForeignWord.Text = "🎉 Изучение завершено!";
                lblResult.Text = "Вы успешно проверили все доступные слова!";
                lblResult.ForeColor = Color.Green;

                if (lblCategory != null) lblCategory.Text = "";
                if (lblProgress != null) lblProgress.Text = "Прогресс: 100%";

                // Выключаем элементы ввода, чтобы было понятно, что круг окончен
                txtUserAnswer.Clear();
                txtUserAnswer.Enabled = false;
                btnCheck.Enabled = false;

                // Кнопку "Следующее слово" превращаем в кнопку "Начать сначала"
                btnNext.Enabled = true;
                btnNext.Text = "Повторить круг";
                btnNext.Focus();
                return;
            }

            // Если слова ещё есть — возвращаем стандартный режим кнопки "Следующее"
            btnNext.Text = "Следующее слово";
            txtUserAnswer.Enabled = true;

            // Показываем иностранное слово
            lblForeignWord.Text = dtWords.Rows[currentWordIndex]["ForeighWord"].ToString();

            // Выводим категорию слова (если элемент lblCategory добавлен на форму)
            if (lblCategory != null)
            {
                string category = dtWords.Rows[currentWordIndex]["CategoryName"].ToString();
                lblCategory.Text = !string.IsNullOrWhiteSpace(category) ? $"Тема: {category}" : "Тема: Общая";
            }

            // Обновляем текстовый счётчик прогресса (например: "Слово: 3 из 12")
            if (lblProgress != null)
            {
                lblProgress.Text = $"Слово: {currentWordIndex + 1} из {dtWords.Rows.Count}";
            }

            // Очищаем результаты прошлого слова и ставим фокус на ввод ответа
            txtUserAnswer.Clear();
            lblResult.Text = "";
            btnCheck.Enabled = true;
            btnNext.Enabled = false; // Кнопка "Следующее" заблокирована, пока пользователь не ответит правильно
            txtUserAnswer.Focus();
        }

        // 3. КНОПКА ПРОВЕРИТЬ ОТВЕТ
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserAnswer.Text))
            {
                MessageBox.Show("Введите перевод слова!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string correctTranslation = dtWords.Rows[currentWordIndex]["Translation"].ToString().Trim();
                string userResponse = txtUserAnswer.Text.Trim();

                // Сравниваем перевод пользователя с правильным (игнорируя регистр букв)
                if (string.Equals(correctTranslation, userResponse, StringComparison.OrdinalIgnoreCase))
                {
                    lblResult.Text = "🎉 Правильно!";
                    lblResult.ForeColor = Color.Green;

                    btnCheck.Enabled = false; // Отключаем кнопку проверки
                    btnNext.Enabled = true;   // Разрешаем нажать кнопку "Следующее слово"
                    btnNext.Focus();
                }
                else
                {
                    lblResult.Text = "❌ Неверно, попробуйте еще раз!";
                    lblResult.ForeColor = Color.Red;
                    txtUserAnswer.Focus();
                    txtUserAnswer.SelectAll(); // Выделяем текст, чтобы его было удобно переписать
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке слова: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. КНОПКА СЛЕДУЮЩЕЕ СЛОВО (ИЛИ НАЧАТЬ СНАЧАЛА)
        private void btnNext_Click(object sender, EventArgs e)
        {
            // Если пользователь нажал кнопку, когда круг уже завершился
            if (currentWordIndex >= dtWords.Rows.Count)
            {
                currentWordIndex = 0; // Сбрасываем индекс на самое первое слово
                ShowWord();
                return;
            }

            // Иначе просто переходим к индексу следующего слова
            currentWordIndex++;
            ShowWord();
        }
    }
}
